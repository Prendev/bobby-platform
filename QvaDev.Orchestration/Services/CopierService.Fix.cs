using System;
using System.Linq;
using System.Threading.Tasks;
using QvaDev.Common;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration.Services
{
    public partial class CopierService
	{
		private Task CopyToFixAccount(NewPosition e, Slave slave)
		{
			if (!(slave.Account?.Connector is FixApiIntegration.Connector slaveConnector)) return Task.CompletedTask;
			if (slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) != true) return Task.CompletedTask;
			var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
				? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
				: e.Position.Symbol + (slave.SymbolSuffix ?? "");

			var tasks = slave.FixApiCopiers.Where(s => s.Run).Select(copier => DelayedRun(async () =>
			{
				var quantity = Math.Abs((decimal)e.Position.Lots * copier.CopyRatio);
				quantity = Math.Floor(quantity);
				if (quantity == 0)
				{
					Logger.Warn($"CopierService.CopyToFixAccount {slave} {symbol} quantity is zero!!!");
					return;
				}

				var side = copier.CopyRatio < 0 ? e.Position.Side.Inv() : e.Position.Side;
				if (e.Action == NewPositionActions.Close) side = side.Inv();

				var limitPrice = 0m;
				if (copier.OrderType != FixApiCopier.FixApiOrderTypes.Market)
				{
					var lastTick = slaveConnector.GetLastTick(symbol);
					if (lastTick == null)
					{
						Logger.Warn($"CopierService.CopyToFixAccount {slave} {symbol} no last tick!!!");
						return;
					}
					limitPrice = copier.BasePriceType == FixApiCopier.BasePriceTypes.Master ? e.Position.OpenPrice :
						side == Sides.Buy ? lastTick.Ask : lastTick.Bid;
				}

				if (e.Action == NewPositionActions.Open)
				{
					// Check if there is an open position
					if (copier.FixApiCopierPositions.Any(p => !p.Archived && p.MasterPositionId == e.Position.Id && p.ClosePosition == null)) return;
					var response = await FixAccountOpening(copier, slaveConnector, symbol, side, quantity, limitPrice);
					if (response == null) return;
					PersistOpenPosition(copier, symbol, e.Position.Id, response);
					LogOpen(slave, symbol, response);
				}
				else if (e.Action == NewPositionActions.Close)
				{
					var pos = copier.FixApiCopierPositions
						.FirstOrDefault(p => !p.Archived && p.MasterPositionId == e.Position.Id && p.ClosePosition == null);
					if (pos == null) return;
					var response = await FixAccountClosing(copier, slaveConnector, symbol, side, pos.OpenPosition.Size, limitPrice);
					if (response == null) return;
					PersistClosePosition(copier, pos, response);
					LogClose(slave, symbol, pos.OpenPosition, response);
				}

			}, copier.DelayInMilliseconds));
			return Task.WhenAll(tasks);
		}

		private void PersistOpenPosition(FixApiCopier copier, string symbol, long masterPositionId, OrderResponse response)
		{
			if (!response.IsFilled) return;
			var side = response.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell;

			var newEntity = new FixApiCopierPosition()
			{
				FixApiCopier = copier,
				FixApiCopierId = copier.Id,
				MasterPositionId = masterPositionId,
				OpenPosition = new StratPosition()
				{
					AccountId = copier.Slave.Account.Id,
					Account = copier.Slave.Account,
					AvgPrice = response.AveragePrice ?? 0,
					OpenTime = HiResDatetime.UtcNow,
					Side = side,
					Size = response.FilledQuantity,
					Symbol = symbol
				}
			};
			lock (copier.FixApiCopierPositions) copier.FixApiCopierPositions.Add(newEntity);
		}

		private void PersistClosePosition(FixApiCopier copier, FixApiCopierPosition pos, OrderResponse response)
		{
			if (!response.IsFilled) return;
			var side = response.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell;

			pos.ClosePosition = new StratPosition()
			{
				AccountId = copier.Slave.Account.Id,
				Account = copier.Slave.Account,
				AvgPrice = response.AveragePrice ?? 0,
				OpenTime = HiResDatetime.UtcNow,
				Side = side,
				Size = response.FilledQuantity,
				Symbol = pos.OpenPosition.Symbol
			};
		}

		private void LogOpen(Slave slave, string symbol, OrderResponse open)
		{
			if (open == null) return;
			if (open.FilledQuantity == 0)
				Logger.Warn($"\t{slave}\t{symbol}\t{open.FilledQuantity}\t{open.AveragePrice}");
			else Logger.Info($"\t{slave}\t{symbol}\t{open.FilledQuantity}\t{open.AveragePrice}");
		}

		private void LogClose(Slave slave, string symbol, StratPosition open, OrderResponse close)
		{
			if (open == null || close == null) return;
			var diff = close.IsFilled ? open.AvgPrice - close.AveragePrice : null;
			if (open.Side == StratPosition.Sides.Buy) diff *= -1;

			if (open.Size != close.FilledQuantity)
				Logger.Error($"\t{slave}\t{symbol}\t{open.Size}\t{open.AvgPrice}\t{close.FilledQuantity}\t{close.AveragePrice}\t{diff}");
			else Logger.Info($"\t{slave}\t{symbol}\t{open.Size}\t{open.AvgPrice}\t{close.FilledQuantity}\t{close.AveragePrice}\t{diff}");
		}

		private async Task<OrderResponse> FixAccountOpening(FixApiCopier copier, IFixConnector connector, string symbol, Sides side,
		    decimal quantity, decimal limitPrice)
	    {
		    OrderResponse response;
		    if (copier.OrderType == FixApiCopier.FixApiOrderTypes.Market)
				return await connector.SendMarketOrderRequest(symbol, side, quantity,
				    copier.MarketTimeWindowInMs, copier.MarketMaxRetryCount, copier.MarketRetryPeriodInMs);
			else if (copier.OrderType == FixApiCopier.FixApiOrderTypes.GtcLimit)
				response = await connector.SendGtcLimitOrderRequest(
					symbol, side, quantity, limitPrice, copier.Deviation, copier.LimitDiff,
					copier.TimeWindowInMs, copier.MaxRetryCount, copier.RetryPeriodInMs);
		    else if (copier.OrderType == FixApiCopier.FixApiOrderTypes.Aggressive)
				response = await connector.SendAggressiveOrderRequest(
					symbol, side, quantity, limitPrice, copier.Deviation, copier.LimitDiff,
					copier.TimeWindowInMs, copier.MaxRetryCount, copier.RetryPeriodInMs);
			else return null;

			// Finished if everything is filled or no forced market open
		    var remainingQuantity = response.OrderedQuantity - response.FilledQuantity;
		    if (remainingQuantity == 0 || !copier.FallbackToMarketOrderType) return response;

			// Try to open on market as a fallback
		    var market = await connector.SendMarketOrderRequest(symbol, side, remainingQuantity,
			    copier.FallbackTimeWindowInMs, copier.FallbackMaxRetryCount, copier.FallbackRetryPeriodInMs);
		    if (!market.IsFilled) return response;

			// Recalculate avg price and filled quantity
		    response.AveragePrice =
			    (response.AveragePrice * response.FilledQuantity + market.AveragePrice * market.FilledQuantity) /
			    (response.FilledQuantity + market.FilledQuantity);
		    response.FilledQuantity += market.FilledQuantity;
		    return response;
		}

	    private async Task<OrderResponse> FixAccountClosing(FixApiCopier copier, IFixConnector connector, string symbol, Sides side,
		    decimal quantity, decimal? limitPrice)
		{
			OrderResponse response;
			if (copier.OrderType == FixApiCopier.FixApiOrderTypes.Market || !limitPrice.HasValue)
				return await connector.SendMarketOrderRequest(symbol, side, quantity,
					copier.MarketTimeWindowInMs, copier.MarketMaxRetryCount, copier.MarketRetryPeriodInMs);
			else if (copier.OrderType == FixApiCopier.FixApiOrderTypes.GtcLimit)
			    response = await connector.SendGtcLimitOrderRequest(
					symbol, side, quantity, limitPrice.Value, copier.Deviation, copier.LimitDiff,
					copier.TimeWindowInMs, copier.MaxRetryCount, copier.RetryPeriodInMs);
			else if (copier.OrderType == FixApiCopier.FixApiOrderTypes.Aggressive)
				response = await connector.SendAggressiveOrderRequest(
					symbol, side, quantity, limitPrice.Value, copier.Deviation, copier.LimitDiff,
					copier.TimeWindowInMs, copier.MaxRetryCount, copier.RetryPeriodInMs);
			else return null;

			// Finished if everything is filled
			var remainingQuantity = response.OrderedQuantity - response.FilledQuantity;
			if (remainingQuantity == 0) return response;

			// Fall back to market order type
			var market = await connector.SendMarketOrderRequest(symbol, side, remainingQuantity,
				copier.MarketTimeWindowInMs, copier.MarketMaxRetryCount, copier.MarketRetryPeriodInMs);
			if (!market.IsFilled) return response;

			// Recalculate avg price and filled quantity
			response.AveragePrice =
				(response.AveragePrice * response.FilledQuantity + market.AveragePrice * market.FilledQuantity) /
				(response.FilledQuantity + market.FilledQuantity);
			response.FilledQuantity += market.FilledQuantity;
			return response;
		}
    }
}

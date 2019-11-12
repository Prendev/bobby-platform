using System;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;
using static TradeSystem.Data.Models.StratHubArbQuoteEventArgs;
using OrderTypes = TradeSystem.Data.Models.StratHubArb.StratHubArbOrderTypes;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public partial class HubArbService
	{
		private Task<OrderResponse> SendPosition(StratHubArb arb, Quote quote, Sides side,
			decimal size, OrderTypes orderType)
		{
			var price = side == Sides.Buy ? quote.Ask : quote.Bid;
			return SendPosition(arb, quote.AggAccount.Account, quote.Symbol, side, size, orderType, price);
		}

		private async Task<OrderResponse> SendPosition(StratHubArb arb, Account account, string symbol, Sides side,
			 decimal size, OrderTypes orderType, decimal? price = null)
		{
			try
			{
				var response = new OrderResponse()
				{
					OrderedQuantity = size,
					AveragePrice = null,
					FilledQuantity = 0,
					Side = side
				};

				if (account.Connector is IFixConnector fix)
				{
					if (price == null || orderType == OrderTypes.Market)
						response = await fix.SendMarketOrderRequest(symbol, side, size,
							arb.TimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);

					else if (orderType == OrderTypes.Aggressive)
						response = await fix.SendAggressiveOrderRequest(
							symbol, side, size, price.Value, arb.Deviation, 0,
							arb.TimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);

					else if (orderType == OrderTypes.DelayedAggressive)
						response = await fix.SendDelayedAggressiveOrderRequest(
							symbol, side, size, price.Value, arb.Deviation, 0, arb.Correction,
							arb.DelayTimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);

					else throw new NotImplementedException();
				}
				else if (account.Connector is Mt4Integration.IConnector mt4)
				{
					var resp = mt4.SendMarketOrderRequest(symbol, side, (double) size, 0, null, arb.MaxRetryCount, arb.RetryPeriodInMs);
					if (resp?.Pos?.Lots > 0)
					{
						response.AveragePrice = resp.Pos.OpenPrice;
						response.FilledQuantity = resp.Pos.Lots;
					}
				}
				else throw new NotImplementedException();

				PersistPosition(arb, account, symbol, response);
				return response;
			}
			catch (Exception e)
			{
				Logger.Error("HubArbService.SendPosition exception", e);
				return new OrderResponse()
				{
					OrderedQuantity = size,
					AveragePrice = null,
					FilledQuantity = 0,
					Side = side
				};
			}
		}

		private void PersistPosition(StratHubArb arb, Account account, string symbol, OrderResponse response)
		{
			var side = response.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell;

			var newEntity = new StratHubArbPosition()
			{
				StratHubArb = arb,
				StratHubArbId = arb.Id,
				Position = new StratPosition()
				{
					AccountId = account.Id,
					Account = account,
					AvgPrice = response.AveragePrice ?? 0,
					OpenTime = HiResDatetime.UtcNow,
					Side = side,
					Size = response.FilledQuantity,
					Symbol = symbol
				}
			};
			account.LastOrderTime = newEntity.Position.OpenTime;
			lock (arb.StratHubArbPositions) arb.StratHubArbPositions.Add(newEntity);
			lock (account.StratHubArbPositions) account.StratHubArbPositions.Add(newEntity);
		}
	}
}

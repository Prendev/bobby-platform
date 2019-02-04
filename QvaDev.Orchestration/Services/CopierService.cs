using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QvaDev.Collections;
using QvaDev.Common;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration.Services
{
    public interface ICopierService
    {
        void Start(List<Master> masters);
        void Stop();
    }

    public class CopierService : ICopierService
    {
	    private volatile CancellationTokenSource _cancellation;
	    private CustomThreadPool _copyPool;

        private IEnumerable<Master> _masters;

	    private readonly ConcurrentDictionary<int, FastBlockingCollection<NewPosition>> _masterQueues =
		    new ConcurrentDictionary<int, FastBlockingCollection<NewPosition>>();

        public void Start(List<Master> masters)
		{
			_cancellation?.Dispose();

			_masters = masters;
			_cancellation = new CancellationTokenSource();

			var threadCount = _masters.Sum(m => m.Slaves.Sum(s => s.Copiers.Count + s.FixApiCopiers.Count));
			_copyPool = new CustomThreadPool(threadCount, "CopyPool", _cancellation.Token);

			foreach (var master in _masters)
	        {
		        master.NewPosition -= Master_NewPosition;
		        master.NewPosition += Master_NewPosition;

				var slaves = master.Slaves
			        .Where(s => s.Account.FixApiAccountId.HasValue &&
			                    s.Account.Connector != null &&
								s.Account.Connector.IsConnected);

		        foreach (var slave in slaves)
		        foreach (var symbolMapping in slave.SymbolMappings)
			        slave.Account.Connector.Subscribe(symbolMapping.To);

		        new Thread(() => MasterLoop(master, _cancellation.Token)) { IsBackground = true }.Start();
			}

			Logger.Info("Copiers are started");
        }

        public void Stop()
        {
	        _cancellation?.Cancel(true);
	        Logger.Info("Copiers are stopped");
		}

	    private async void MasterLoop(Master master, CancellationToken token)
		{
			var queue = _masterQueues.GetOrAdd(master.Id, new FastBlockingCollection<NewPosition>());

			while (!token.IsCancellationRequested)
			{
				try
				{
					var newPos = queue.Take(token);

					Logger.Info($"Master {newPos.Action:F} {newPos.Position.Side:F} signal on " +
					          $"{newPos.Position.Symbol} with open time: {newPos.Position.OpenTime:o}");

					var slaves = master.Slaves.Where(s => s.Run && s.Account.ConnectionState == ConnectionStates.Connected);
					await Task.WhenAll(slaves.Select(slave => CopyToAccount(newPos, slave)));
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("CopierService.MasterLoop exception", e);
				}
			}

			_masterQueues.TryRemove(master.Id, out queue);
		}

		private void Master_NewPosition(object sender, NewPosition e)
        {
            if (_cancellation.IsCancellationRequested) return;
	        if (!(sender is Master master)) return;
	        if (!master.Run) return;

	        _masterQueues.GetOrAdd(master.Id, new FastBlockingCollection<NewPosition>()).Add(e);
        }

        private Task CopyToAccount(NewPosition e, Slave slave)
        {
			if (slave.Account.MetaTraderAccountId.HasValue) return CopyToMtAccount(e, slave);
			if (slave.Account.CTraderAccountId.HasValue) return CopyToCtAccount(e, slave);
			if (slave.Account.FixApiAccountId.HasValue) return CopyToFixAccount(e, slave);
	        return Task.FromResult(0);
		}

		private Task CopyToCtAccount(NewPosition e, Slave slave)
        {
	        if (!(slave.Account?.Connector is CTraderIntegration.Connector slaveConnector)) return Task.FromResult(0);

            var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
                ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
                : e.Position.Symbol + (slave.SymbolSuffix ?? "");

            var tasks = slave.Copiers.Where(s => s.Run).Select(copier => DelayedRun(() =>
			{
				var volume = (long)(100 * Math.Abs(e.Position.RealVolume * copier.CopyRatio));
				var side = copier.CopyRatio < 0 ? e.Position.Side.Inv() : e.Position.Side;
				var type = side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;

				if (e.Action == NewPositionActions.Open && copier.OrderType == Copier.CopierOrderTypes.MarketRange)
					slaveConnector.SendMarketRangeOrderRequest(symbol, type, volume, e.Position.OpenPrice,
						copier.SlippageInPips, $"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMs);
				else if (e.Action == NewPositionActions.Open && copier.OrderType == Copier.CopierOrderTypes.Market)
					slaveConnector.SendMarketOrderRequest(symbol, type, volume, $"{slave.Id}-{e.Position.Id}",
						copier.MaxRetryCount, copier.RetryPeriodInMs);
				else if (e.Action == NewPositionActions.Close)
					slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Position.Id}",
						copier.MaxRetryCount, copier.RetryPeriodInMs);
				return Task.CompletedTask;
			}, copier.DelayInMilliseconds));
            return Task.WhenAll(tasks);
        }

        private Task CopyToMtAccount(NewPosition e, Slave slave)
        {
	        if (!(slave.Account?.Connector is Mt4Integration.Connector slaveConnector)) return Task.FromResult(0);

            var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
                ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
                : e.Position.Symbol + (slave.SymbolSuffix ?? "");

	        var tasks = slave.Copiers.Where(s => s.Run).Select(copier => DelayedRun(() =>
	        {
		        // TODO
		        //var lots = Math.Abs(e.Position.RealVolume) / slaveConnector.GetContractSize(symbol) *
		        //           (double) copier.CopyRatio;
		        var lots = Math.Abs(e.Position.Lots * (double) copier.CopyRatio);
		        var side = copier.CopyRatio < 0 ? e.Position.Side.Inv() : e.Position.Side;
		        var comment = $"{slave.Id}-{e.Position.Id}-{copier.Id}";
		        if (e.Action == NewPositionActions.Open)
			        slaveConnector.SendMarketOrderRequest(symbol, side, lots, e.Position.MagicNumber,
				        comment, copier.MaxRetryCount, copier.RetryPeriodInMs);
		        else if (e.Action == NewPositionActions.Close)
			        slaveConnector.SendClosePositionRequests(comment, copier.MaxRetryCount, copier.RetryPeriodInMs);
		        return Task.CompletedTask;
			}, copier.DelayInMilliseconds));
            return Task.WhenAll(tasks);
		}

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
					var response = await FixAccountOpening(copier, slaveConnector, symbol, side, quantity, limitPrice);
					if (response == null) return;
					copier.OrderResponses[e.Position.Id] = response;
					LogOpen(slave, symbol, response);
				}
				else if (e.Action == NewPositionActions.Close)
				{
					if (!copier.OrderResponses.TryGetValue(e.Position.Id, out var openResponse)) return;
					if (!openResponse.IsFilled || openResponse.FilledQuantity == 0) return;
					var response = await FixAccountClosing(copier, slaveConnector, symbol, side, openResponse.FilledQuantity, limitPrice);
					if (response == null) return;
					LogClose(slave, symbol, openResponse, response);
				}

			}, copier.DelayInMilliseconds));
			return Task.WhenAll(tasks);
		}

	    private void LogOpen(Slave slave, string symbol, OrderResponse open)
	    {
		    if (open == null) return;
		    if (open.FilledQuantity == 0)
			    Logger.Warn($"\t{slave}\t{symbol}\t{open.FilledQuantity}\t{open.AveragePrice}");
		    else Logger.Info($"\t{slave}\t{symbol}\t{open.FilledQuantity}\t{open.AveragePrice}");
	    }

	    private void LogClose(Slave slave, string symbol, OrderResponse open, OrderResponse close)
	    {
		    if (open == null || close == null) return;
		    var diff = close.IsFilled ? open.AveragePrice - close.AveragePrice : null;
		    if (open.Side == Sides.Buy) diff *= -1;

			if (open.FilledQuantity != close.FilledQuantity)
				Logger.Error($"\t{slave}\t{symbol}\t{open.FilledQuantity}\t{open.AveragePrice}\t{close.FilledQuantity}\t{close.AveragePrice}\t{diff}");
			else Logger.Info($"\t{slave}\t{symbol}\t{open.FilledQuantity}\t{open.AveragePrice}\t{close.FilledQuantity}\t{close.AveragePrice}\t{diff}");
		}

	    private async Task<OrderResponse> FixAccountOpening(FixApiCopier copier, IFixConnector connector, string symbol, Sides side,
		    decimal quantity, decimal limitPrice)
	    {
		    if (copier.OrderType == FixApiCopier.FixApiOrderTypes.Market)
			    return await connector.SendMarketOrderRequest(symbol, side, quantity,
				    copier.MarketTimeWindowInMs, copier.MarketMaxRetryCount, copier.MarketRetryPeriodInMs);

			// Initiate limit order types
		    OrderResponse response;
			if (copier.OrderType == FixApiCopier.FixApiOrderTypes.GtcLimit)
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

			// Try to open once more on market
		    var market = await connector.SendMarketOrderRequest(symbol, side, remainingQuantity, copier.FallbackTimeWindowInMs, 0, 0);
		    if (!market.IsFilled) return response;

			// Recalculate avg price and filled quantity
		    response.AveragePrice =
			    (response.AveragePrice * response.FilledQuantity + market.AveragePrice * market.FilledQuantity) /
			    (response.FilledQuantity + market.FilledQuantity);
		    response.FilledQuantity += market.FilledQuantity;
		    return response;
		}

	    private async Task<OrderResponse> FixAccountClosing(FixApiCopier copier, IFixConnector connector, string symbol, Sides side,
		    decimal quantity, decimal limitPrice)
		{
			if (copier.OrderType == FixApiCopier.FixApiOrderTypes.Market)
				return await connector.SendMarketOrderRequest(symbol, side, quantity,
					copier.MarketTimeWindowInMs, copier.MarketMaxRetryCount, copier.MarketRetryPeriodInMs);

			// Initiate limit order types
			OrderResponse response;
			if (copier.OrderType == FixApiCopier.FixApiOrderTypes.GtcLimit)
			    response = await connector.SendGtcLimitOrderRequest(
					symbol, side, quantity, limitPrice, copier.Deviation, copier.LimitDiff,
					copier.TimeWindowInMs, copier.MaxRetryCount, copier.RetryPeriodInMs);
			else if (copier.OrderType == FixApiCopier.FixApiOrderTypes.Aggressive)
				response = await connector.SendAggressiveOrderRequest(
					symbol, side, quantity, limitPrice, copier.Deviation, copier.LimitDiff,
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

	    private async Task DelayedRun(Func<Task> action, int millisecondsDelay)
	    {
		    try
			{
				if (millisecondsDelay > 0) await Task.Delay(millisecondsDelay);
				await _copyPool.Run(action);
			}
		    catch (Exception e)
		    {
			    Logger.Error("CopierService.DelayedRun exception", e);
			}
	    }
    }
}

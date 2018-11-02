using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QvaDev.Common;
using QvaDev.Common.Integration;
using QvaDev.Common.Logging;
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

		private readonly ILog _log;
        private IEnumerable<Master> _masters;

	    private readonly ConcurrentDictionary<int, BlockingCollection<NewPositionEventArgs>> _masterQueues =
		    new ConcurrentDictionary<int, BlockingCollection<NewPositionEventArgs>>();

		public CopierService(ILog log)
        {
            _log = log;
        }

        public void Start(List<Master> masters)
		{
			_cancellation?.Dispose();
			_copyPool?.Dispose();

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

            _log.Info("Copiers are started");
        }

        public void Stop()
        {
	        _cancellation?.Cancel(true);
			_log.Info("Copiers are stopped");
		}

	    private async void MasterLoop(Master master, CancellationToken token)
		{
			var queue = _masterQueues.GetOrAdd(master.Id, new BlockingCollection<NewPositionEventArgs>());

			while (!token.IsCancellationRequested)
			{
				try
				{
					var newPos = queue.Take(token);

					_log.Info($"Master {newPos.Action:F} {newPos.Position.Side:F} signal on " +
					          $"{newPos.Position.Symbol} with open time: {newPos.Position.OpenTime:o}");

					var slaves = master.Slaves.Where(s => s.Run);
					await Task.WhenAll(slaves.Select(slave => CopyToAccount(newPos, slave)));
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					_log.Error("CopierService.MasterLoop exception", e);
				}
			}

			queue.Dispose();
			_masterQueues.TryRemove(master.Id, out queue);
		}

		private void Master_NewPosition(object sender, NewPositionEventArgs e)
        {
            if (_cancellation.IsCancellationRequested) return;
	        if (!(sender is Master master)) return;
	        if (!master.Run) return;

	        _masterQueues[master.Id].Add(e);
        }

        private Task CopyToAccount(NewPositionEventArgs e, Slave slave)
        {
            if (slave.Account.MetaTraderAccountId.HasValue) return CopyToMtAccount(e, slave);
            if (slave.Account.CTraderAccountId.HasValue) return CopyToCtAccount(e, slave);
			if (slave.Account.FixApiAccountId.HasValue) return CopyToFixAccount(e, slave);
			return Task.FromResult(0);
		}

		private Task CopyToCtAccount(NewPositionEventArgs e, Slave slave)
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

				if (e.Action == NewPositionEventArgs.Actions.Open && copier.OrderType == Copier.CopierOrderTypes.MarketRange)
                    slaveConnector.SendMarketRangeOrderRequest(symbol, type, volume, e.Position.OpenPrice,
                        copier.SlippageInPips, $"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMs);
                else if (e.Action == NewPositionEventArgs.Actions.Open && copier.OrderType == Copier.CopierOrderTypes.Market)
                    slaveConnector.SendMarketOrderRequest(symbol, type, volume, $"{slave.Id}-{e.Position.Id}",
                        copier.MaxRetryCount, copier.RetryPeriodInMs);
                else if (e.Action == NewPositionEventArgs.Actions.Close)
                    slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Position.Id}",
                        copier.MaxRetryCount, copier.RetryPeriodInMs);

            }, copier.DelayInMilliseconds));
            return Task.WhenAll(tasks);
        }

        private Task CopyToMtAccount(NewPositionEventArgs e, Slave slave)
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
		        if (e.Action == NewPositionEventArgs.Actions.Open)
			        slaveConnector.SendMarketOrderRequest(symbol, side, lots, e.Position.MagicNumber,
				        comment, copier.MaxRetryCount, copier.RetryPeriodInMs);
		        else if (e.Action == NewPositionEventArgs.Actions.Close)
			        slaveConnector.SendClosePositionRequests(comment, copier.MaxRetryCount, copier.RetryPeriodInMs);
	        }, copier.DelayInMilliseconds));
            return Task.WhenAll(tasks);
		}

		private Task CopyToFixAccount(NewPositionEventArgs e, Slave slave)
		{
			if (!(slave.Account?.Connector is FixApiIntegration.Connector slaveConnector)) return Task.FromResult(0);
			if (slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) != true) return Task.FromResult(0);
			var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
				? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
				: e.Position.Symbol + (slave.SymbolSuffix ?? "");

			var tasks = slave.FixApiCopiers.Where(s => s.Run).Select(copier => DelayedRun(async () =>
			{
				var quantity = Math.Abs((decimal)e.Position.Lots * copier.CopyRatio);
				quantity = Math.Floor(quantity);
				if (quantity == 0) return;
				var side = copier.CopyRatio < 0 ? e.Position.Side.Inv() : e.Position.Side;

				if (e.Action == NewPositionEventArgs.Actions.Open && copier.OrderType == FixApiCopier.FixApiOrderTypes.Aggressive)
				{
					var response = await slaveConnector.SendAggressiveOrderRequest(symbol, side, quantity, e.Position.OpenPrice,
						copier.Deviation, copier.TimeWindowInMs, copier.MaxRetryCount, copier.RetryPeriodInMs);
					copier.OrderResponses[e.Position.Id] = response;
				}
				else if (e.Action == NewPositionEventArgs.Actions.Open && copier.OrderType == FixApiCopier.FixApiOrderTypes.Market)
				{
					var response = await slaveConnector.SendMarketOrderRequest(symbol, side, quantity);
					copier.OrderResponses[e.Position.Id] = response;
				}
				else if (e.Action == NewPositionEventArgs.Actions.Close && copier.OrderType == FixApiCopier.FixApiOrderTypes.Aggressive)
				{
					if (!copier.OrderResponses.TryGetValue(e.Position.Id, out OrderResponse openResponse)) return;
					if (!openResponse.IsFilled || openResponse.FilledQuantity == 0) return;
					var closeResponse = await slaveConnector.SendAggressiveOrderRequest(symbol, side.Inv(),
						openResponse.FilledQuantity, e.Position.ClosePrice,
						copier.Deviation, copier.TimeWindowInMs, copier.MaxRetryCount, copier.RetryPeriodInMs);

					var remainingQuantity = closeResponse.OrderedQuantity - closeResponse.FilledQuantity;
					if (remainingQuantity == 0) return;
					await slaveConnector.SendMarketOrderRequest(symbol, side.Inv(), remainingQuantity);
				}
				else if (e.Action == NewPositionEventArgs.Actions.Close && copier.OrderType == FixApiCopier.FixApiOrderTypes.Market)
				{
					if (!copier.OrderResponses.TryGetValue(e.Position.Id, out OrderResponse openResponse)) return;
					if (!openResponse.IsFilled || openResponse.FilledQuantity == 0) return;
					await slaveConnector.SendMarketOrderRequest(symbol, side.Inv(), openResponse.FilledQuantity);
				}

			}, copier.DelayInMilliseconds));
			return Task.WhenAll(tasks);
		}

	    private async Task DelayedRun(Action action, int millisecondsDelay)
	    {
		    if (millisecondsDelay > 0) await Task.Delay(millisecondsDelay);
		    await _copyPool.Run(action);
	    }
    }
}

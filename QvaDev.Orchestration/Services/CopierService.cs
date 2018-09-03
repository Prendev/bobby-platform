using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
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
        private bool _isStarted;
        private readonly ILog _log;
        private IEnumerable<Master> _masters;

        public CopierService(ILog log)
        {
            _log = log;
        }

        public void Start(List<Master> masters)
        {
	        _masters = masters;

	        foreach (var master in _masters)
	        {
		        master.Account.Connector.NewPosition -= Master_NewPosition;
		        master.Account.Connector.NewPosition += Master_NewPosition;

		        var slaves = master.Slaves
			        .Where(s => s.Account.FixApiAccountId.HasValue &&
			                    s.Account.Connector != null &&
								s.Account.Connector.IsConnected);

		        foreach (var slave in slaves)
		        foreach (var symbolMapping in slave.SymbolMappings)
			        slave.Account.Connector.Subscribe(symbolMapping.To);

	        }

	        _isStarted = true;
            _log.Info("Copiers are started");
        }

        public void Stop()
        {
            _isStarted = false;
        }

        private void Master_NewPosition(object sender, NewPositionEventArgs e)
        {
            if (!_isStarted) return;
			Task.Factory.StartNew(() =>
            {
                lock (sender)
                {
                    _log.Info($"Master {e.Action:F} {e.Position.Side:F} signal on " +
                              $"{e.Position.Symbol} with open time: {e.Position.OpenTime:o}");

	                var slaves = _masters.Where(m => m.Run && m.Account.MetaTraderAccountId == e.DbId)
		                .SelectMany(m => m.Slaves).Where(s => s.Run);
					Task.WhenAll(slaves.Select(slave => CopyToAccount(e, slave))).Wait();
                }
            });
        }

        private Task CopyToAccount(NewPositionEventArgs e, Slave slave)
        {
            if (slave.Account.MetaTraderAccountId.HasValue) return CopyToMtAccount(e, slave);
            if (slave.Account.CTraderAccountId.HasValue) return CopyToCtAccount(e, slave);
	        //if (slave.Account.FixTraderAccountId.HasValue) return CopyToFtAccount(e, slave);
			if (slave.Account.FixApiAccountId.HasValue) return CopyToFixAccount(e, slave);
			return Task.FromResult(0);
		}

		private Task CopyToCtAccount(NewPositionEventArgs e, Slave slave)
        {
	        if (!(slave.Account?.Connector is CTraderIntegration.Connector slaveConnector)) return Task.FromResult(0);

            var type = e.Position.Side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;
            var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
                ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
                : e.Position.Symbol + (slave.SymbolSuffix ?? "");

            var tasks = slave.Copiers.Where(s => s.Run).Select(copier => Task.Factory.StartNew(() =>
			{
				if (copier.DelayInMilliseconds > 0) Thread.Sleep(copier.DelayInMilliseconds);

				var volume = (long)(100 * Math.Abs(e.Position.RealVolume) * copier.CopyRatio);
                if (e.Action == NewPositionEventArgs.Actions.Open && copier.OrderType == Copier.CopierOrderTypes.MarketRange)
                    slaveConnector.SendMarketRangeOrderRequest(symbol, type, volume, e.Position.OpenPrice,
                        copier.SlippageInPips, $"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMs);
                else if (e.Action == NewPositionEventArgs.Actions.Open && copier.OrderType == Copier.CopierOrderTypes.Market)
                    slaveConnector.SendMarketOrderRequest(symbol, type, volume, $"{slave.Id}-{e.Position.Id}",
                        copier.MaxRetryCount, copier.RetryPeriodInMs);
                else if (e.Action == NewPositionEventArgs.Actions.Close)
                    slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Position.Id}",
                        copier.MaxRetryCount, copier.RetryPeriodInMs);

            }, TaskCreationOptions.LongRunning));
            return Task.WhenAll(tasks);
        }

        private Task CopyToMtAccount(NewPositionEventArgs e, Slave slave)
        {
	        if (!(slave.Account?.Connector is Mt4Integration.Connector slaveConnector)) return Task.FromResult(0);

            var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
                ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
                : e.Position.Symbol + (slave.SymbolSuffix ?? "");

            var tasks = slave.Copiers.Where(s => s.Run).Select(copier => Task.Factory.StartNew(() =>
            {
                if (copier.DelayInMilliseconds > 0) Thread.Sleep(copier.DelayInMilliseconds);
				// TODO
				//var lots = Math.Abs(e.Position.RealVolume) / slaveConnector.GetContractSize(symbol) *
				//           (double) copier.CopyRatio;
				var lots = e.Position.Lots;
                if (e.Action == NewPositionEventArgs.Actions.Open)
                    slaveConnector.SendMarketOrderRequest(symbol, e.Position.Side, lots, e.Position.MagicNumber,
                        $"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMs);
                else if (e.Action == NewPositionEventArgs.Actions.Close)
                    slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMs);
            }, TaskCreationOptions.LongRunning));
            return Task.WhenAll(tasks);
		}

		private Task CopyToFixAccount(NewPositionEventArgs e, Slave slave)
		{
			if (!(slave.Account?.Connector is FixApiIntegration.Connector slaveConnector)) return Task.FromResult(0);
			if (slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) != true) return Task.FromResult(0);
			var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
				? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
				: e.Position.Symbol + (slave.SymbolSuffix ?? "");

			var tasks = slave.FixApiCopiers.Where(s => s.Run).Select(copier => Task.Factory.StartNew(async () =>
			{
				if (copier.DelayInMilliseconds > 0) Thread.Sleep(copier.DelayInMilliseconds);

				var quantity = (decimal) Math.Abs(e.Position.Lots) * copier.CopyRatio;
				quantity = Math.Floor(quantity);
				if (quantity == 0) return;

				if (e.Action == NewPositionEventArgs.Actions.Open && copier.OrderType == FixApiCopier.FixApiOrderTypes.Aggressive)
				{
					var response = await slaveConnector.SendAggressiveOrderRequest(symbol, e.Position.Side, quantity, e.Position.OpenPrice,
						copier.Deviation, copier.TimeWindowInMs, copier.MaxRetryCount, copier.RetryPeriodInMs);
					copier.OrderResponses[e.Position.Id] = response;
				}
				else if (e.Action == NewPositionEventArgs.Actions.Open && copier.OrderType == FixApiCopier.FixApiOrderTypes.Market)
				{
					var response = await slaveConnector.SendMarketOrderRequest(symbol, e.Position.Side, quantity);
					copier.OrderResponses[e.Position.Id] = response;
				}
				else if (e.Action == NewPositionEventArgs.Actions.Close && copier.OrderType == FixApiCopier.FixApiOrderTypes.Aggressive)
				{
					if (!copier.OrderResponses.TryGetValue(e.Position.Id, out OrderResponse openResponse)) return;
					if (!openResponse.IsFilled || openResponse.FilledQuantity == 0) return;
					var closeResponse = await slaveConnector.SendAggressiveOrderRequest(symbol, e.Position.Side.Inv(),
						openResponse.FilledQuantity, e.Position.ClosePrice,
						copier.Deviation, copier.TimeWindowInMs, copier.MaxRetryCount, copier.RetryPeriodInMs);

					var remainingQuantity = closeResponse.OrderedQuantity - closeResponse.FilledQuantity;
					if (remainingQuantity == 0) return;
					await slaveConnector.SendMarketOrderRequest(symbol, e.Position.Side.Inv(), remainingQuantity);
				}
				else if (e.Action == NewPositionEventArgs.Actions.Close && copier.OrderType == FixApiCopier.FixApiOrderTypes.Market)
				{
					if (!copier.OrderResponses.TryGetValue(e.Position.Id, out OrderResponse openResponse)) return;
					if (!openResponse.IsFilled || openResponse.FilledQuantity == 0) return;
					await slaveConnector.SendMarketOrderRequest(symbol, e.Position.Side.Inv(), openResponse.FilledQuantity);
				}

			}, TaskCreationOptions.LongRunning));
			return Task.WhenAll(tasks);
		}
	}
}

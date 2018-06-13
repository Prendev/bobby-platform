using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.FixTraderIntegration;

namespace QvaDev.Orchestration.Services
{
    public interface ICopierService
    {
        void Start(DuplicatContext duplicatContext);
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

        public void Start(DuplicatContext duplicatContext)
        {
			var copiers = duplicatContext.Copiers.Local
                .Where(c => c.Slave.Master.Account.State == Account.States.Connected)
                .Where(c => c.Slave.Account.State == Account.States.Connected)
                .Select(c => c.Slave.Master);

	        var fixApiCopiers = duplicatContext.FixApiCopiers.Local
		        .Where(c => c.Slave.Master.Account.State == Account.States.Connected)
		        .Where(c => c.Slave.Account.State == Account.States.Connected)
		        .Select(c => c.Slave.Master);

	        _masters = copiers.Union(fixApiCopiers).Distinct();

			foreach (var master in _masters)
            {
                master.Account.Connector.OnPosition -= MasterOnOrderUpdate;
                master.Account.Connector.OnPosition += MasterOnOrderUpdate;
            }

            _isStarted = true;
            _log.Info("Copiers are started");
        }

        public void Stop()
        {
            _isStarted = false;
        }

        private void MasterOnOrderUpdate(object sender, PositionEventArgs e)
        {
            if (!_isStarted) return;
			Task.Factory.StartNew(() =>
            {
                lock (sender)
                {
                    _log.Info($"Master {e.Action:F} {e.Position.Side:F} signal on " +
                              $"{e.Position.Symbol} with open time: {e.Position.OpenTime:o}");

	                var slaves = _masters.Where(m => m.Run && m.AccountId == e.DbId)
		                .SelectMany(m => m.Slaves).Where(s => s.Run);
					Task.WhenAll(slaves.Select(slave => CopyToAccount(e, slave))).Wait();
                }
            });
        }

        private Task CopyToAccount(PositionEventArgs e, Slave slave)
        {
            if (slave.Account.MetaTraderAccountId.HasValue) return CopyToMtAccount(e, slave);
            if (slave.Account.CTraderAccountId.HasValue) return CopyToCtAccount(e, slave);
	        if (slave.Account.FixTraderAccountId.HasValue) return CopyToFtAccount(e, slave);
			if (slave.Account.FixApiAccountId.HasValue) return CopyToFixAccount(e, slave);
			return Task.FromResult(0);
		}

	    private Task CopyToFtAccount(PositionEventArgs e, Slave slave)
	    {
		    if (!(slave.Account?.Connector is Connector slaveConnector)) return Task.FromResult(0);

		    var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
			    ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
			    : e.Position.Symbol + (slave.SymbolSuffix ?? "");

		    var tasks = slave.FixApiCopiers.Where(s => s.Run).Select(copier => Task.Factory.StartNew(() =>
		    {
			    if (copier.DelayInMilliseconds > 0) Thread.Sleep(copier.DelayInMilliseconds);

			    var lots = Math.Abs(e.Position.Lots) * (double) copier.CopyRatio;
			    if (e.Action == PositionEventArgs.Actions.Open && copier.OrderType == FixApiCopier.FixApiOrderTypes.Aggressive)
				    slaveConnector.SendAggressiveOrderRequest(symbol, e.Position.Side, lots, e.Position.OpenPrice, copier.Slippage,
					    copier.BurstPeriodInMilliseconds, copier.MaxRetryCount, copier.RetryPeriodInMilliseconds, $"{slave.Id}-{e.Position.Id}");
				else if (e.Action == PositionEventArgs.Actions.Open && copier.OrderType == FixApiCopier.FixApiOrderTypes.Market)
					slaveConnector.SendMarketOrderRequest(symbol, e.Position.Side, lots, $"{slave.Id}-{e.Position.Id}");
				else if (e.Action == PositionEventArgs.Actions.Close)
					slaveConnector.OrderMultipleCloseBy(symbol);

			}, TaskCreationOptions.LongRunning));
		    return Task.WhenAll(tasks);
	    }

		private Task CopyToCtAccount(PositionEventArgs e, Slave slave)
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
                if (e.Action == PositionEventArgs.Actions.Open && copier.OrderType == Copier.CopierOrderTypes.MarketRange)
                    slaveConnector.SendMarketRangeOrderRequest(symbol, type, volume, e.Position.OpenPrice,
                        copier.SlippageInPips, $"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                else if (e.Action == PositionEventArgs.Actions.Open && copier.OrderType == Copier.CopierOrderTypes.Market)
                    slaveConnector.SendMarketOrderRequest(symbol, type, volume, $"{slave.Id}-{e.Position.Id}",
                        copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                else if (e.Action == PositionEventArgs.Actions.Close)
                    slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Position.Id}",
                        copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);

            }, TaskCreationOptions.LongRunning));
            return Task.WhenAll(tasks);
        }

        private Task CopyToMtAccount(PositionEventArgs e, Slave slave)
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
                if (e.Action == PositionEventArgs.Actions.Open)
                    slaveConnector.SendMarketOrderRequest(symbol, e.Position.Side, lots, e.Position.MagicNumber,
                        $"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                else if (e.Action == PositionEventArgs.Actions.Close)
                    slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
            }, TaskCreationOptions.LongRunning));
            return Task.WhenAll(tasks);
		}

		private Task CopyToFixAccount(PositionEventArgs e, Slave slave)
		{
			throw new NotImplementedException();
			if (!(slave.Account?.Connector is FixApiIntegration.Connector slaveConnector)) return Task.FromResult(0);

			var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
				? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
				: e.Position.Symbol + (slave.SymbolSuffix ?? "");

			//var tasks = slave.FixApiCopiers.Where(s => s.Run).Select(copier => Task.Factory.StartNew(() =>
			//{
			//	if (copier.DelayInMilliseconds > 0) Thread.Sleep(copier.DelayInMilliseconds);

			//	var lots = Math.Abs(e.Position.Lots) * (double)copier.CopyRatio;
			//	if (e.Action == PositionEventArgs.Actions.Open && copier.OrderType == FixApiCopier.FixApiOrderTypes.Aggressive)
			//		slaveConnector.SendAggressiveOrderRequest(symbol, e.Position.Side, lots, e.Position.OpenPrice, copier.Slippage,
			//			copier.BurstPeriodInMilliseconds, copier.MaxRetryCount, copier.RetryPeriodInMilliseconds, $"{slave.Id}-{e.Position.Id}");
			//	else if (e.Action == PositionEventArgs.Actions.Open && copier.OrderType == FixApiCopier.FixApiOrderTypes.Market)
			//		slaveConnector.SendMarketOrderRequest(symbol, e.Position.Side, lots, $"{slave.Id}-{e.Position.Id}");
			//	else if (e.Action == PositionEventArgs.Actions.Close)
			//		slaveConnector.OrderMultipleCloseBy(symbol);

			//}, TaskCreationOptions.LongRunning));
			//return Task.WhenAll(tasks);
		}
	}
}

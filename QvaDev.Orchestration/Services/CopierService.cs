using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;

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
            _masters = duplicatContext.Copiers.Local
                .Where(c => c.Slave.Master.MetaTraderAccount.State == BaseAccountEntity.States.Connected)
                .Where(c => c.Slave.CTraderAccount?.State == BaseAccountEntity.States.Connected ||
                            c.Slave.MetaTraderAccount?.State == BaseAccountEntity.States.Connected)
                .Select(c => c.Slave.Master).Distinct();
            foreach (var master in _masters)
            {
                master.MetaTraderAccount.Connector.OnPosition -= MasterOnOrderUpdate;
                master.MetaTraderAccount.Connector.OnPosition += MasterOnOrderUpdate;
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
            if (_masters?.Any() != true) return;
            Task.Factory.StartNew(() =>
            {
                lock (sender)
                {
                    _log.Info($"Master {e.Action:F} {e.Position.Side:F} signal on " +
                              $"{e.Position.Symbol} with open time: {e.Position.OpenTime:o}");

                    var masters = _masters.Where(m => m.MetaTraderAccountId == e.DbId);

                    Task.WhenAll(masters.SelectMany(m => m.Slaves).Select(slave => CopyToAccount(e, slave))).Wait();
                }
            });
        }

        private Task CopyToAccount(PositionEventArgs e, Slave slave)
        {
            if (slave.MetaTraderAccount != null) return CopyToMtAccount(e, slave);
            if (slave.CTraderAccount != null) return CopyToCtAccount(e, slave);
            return Task.FromResult(0);
        }

        private Task CopyToCtAccount(PositionEventArgs e, Slave slave)
        {
            var slaveConnector = slave.CTraderAccount?.Connector as CTraderIntegration.Connector;
            if (slaveConnector == null) return Task.FromResult(0);

            var type = e.Position.Side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;
            var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
                ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
                : e.Position.Symbol + (slave.SymbolSuffix ?? "");

            var tasks = slave.Copiers.Select(copier => Task.Factory.StartNew(() =>
            {
                var volume = (long)(100 * Math.Abs(e.Position.RealVolume) * copier.CopyRatio);
                if (e.Action == PositionEventArgs.Actions.Open && copier.UseMarketRangeOrder)
                    slaveConnector.SendMarketRangeOrderRequest(symbol, type, volume, e.Position.OpenPrice,
                        copier.SlippageInPips, $"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                else if (e.Action == PositionEventArgs.Actions.Open && !copier.UseMarketRangeOrder)
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
            var slaveConnector = slave.MetaTraderAccount?.Connector as Mt4Integration.Connector;
            if (slaveConnector == null) return Task.FromResult(0);

            var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
                ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
                : e.Position.Symbol + (slave.SymbolSuffix ?? "");

            var tasks = slave.Copiers.Select(copier => Task.Factory.StartNew(() =>
            {
                if (copier.DelayInMilliseconds > 0) Thread.Sleep(copier.DelayInMilliseconds);
				// TODO
				//var lots = Math.Abs(e.Position.RealVolume) / slaveConnector.GetContractSize(symbol) *
				//           (double) copier.CopyRatio;
				var lots = e.Position.Lots;
                if (e.Action == PositionEventArgs.Actions.Open)
                    slaveConnector.SendMarketOrderRequest(symbol, e.Position.Side, lots, e.Position.MagicNumber,
                        $"{slave.Id}-{e.Position.Id}");
                else if (e.Action == PositionEventArgs.Actions.Close)
                    slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Position.Id}");
            }, TaskCreationOptions.LongRunning));
            return Task.WhenAll(tasks);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration
{
    public partial class Orchestrator
    {
        private bool _areCopiersStarted;

        public async Task StartCopiers(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId)
        {
            await Connect(duplicatContext, alphaMonitorId, betaMonitorId);
            foreach (var master in _duplicatContext.Copiers.Local
                .Where(c => c.Slave.CTraderAccount.State == BaseAccountEntity.States.Connected &&
                            c.Slave.Master.MetaTraderAccount.State == BaseAccountEntity.States.Connected)
                .Select(c => c.Slave.Master).Distinct())
            {
                master.MetaTraderAccount.Connector.OnPosition -= MasterOnOrderUpdate;
                master.MetaTraderAccount.Connector.OnPosition += MasterOnOrderUpdate;
            }

            _areCopiersStarted = true;
            _log.Info("Copiers are started");
        }

        public void StopCopiers()
        {
            _areCopiersStarted = false;
        }

        private void MasterOnOrderUpdate(object sender, PositionEventArgs e)
        {
            if (!_areCopiersStarted) return;
            Task.Factory.StartNew(() =>
            {
                lock (sender)
                {
                    _log.Info($"Master {e.Action:F} {e.Position.Side:F} signal on " +
                              $"{e.Position.Symbol} with open time: {e.Position.OpenTime:o}");

                    var masters = _duplicatContext.Masters.Local
                        .Where(m => m.MetaTraderAccountId == e.DbId);
                    var type = e.Position.Side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;

                    foreach (var master in masters)
                    foreach (var slave in master.Slaves)
                    {
                        var slaveConnector = (CTraderIntegration.Connector)slave.CTraderAccount.Connector;
                        var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
                            ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
                            : e.Position.Symbol + (slave.SymbolSuffix ?? "");
                        foreach (var copier in slave.Copiers)
                        {
                            var volume = (long)(100 * Math.Abs(e.Position.RealVolume) * copier.CopyRatio);
                            if (e.Action == PositionEventArgs.Actions.Open && copier.UseMarketRangeOrder)
                                slaveConnector.SendMarketRangeOrderRequest(symbol, type, volume, e.Position.OperPrice,
                                    copier.SlippageInPips, $"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                            else if (e.Action == PositionEventArgs.Actions.Open && !copier.UseMarketRangeOrder)
                                slaveConnector.SendMarketOrderRequest(symbol, type, volume, $"{slave.Id}-{e.Position.Id}",
                                    copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                            else if (e.Action == PositionEventArgs.Actions.Close)
                                slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Position.Id}",
                                    copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                        }
                    }
                }
            });
        }
    }
}

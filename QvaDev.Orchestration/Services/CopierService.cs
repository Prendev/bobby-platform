﻿using System;
using System.Linq;
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
        private DuplicatContext _duplicatContext;
        private readonly ILog _log;

        public CopierService(ILog log)
        {
            _log = log;
        }

        public void Start(DuplicatContext duplicatContext)
        {
            _duplicatContext = duplicatContext;
            foreach (var master in _duplicatContext.Copiers.Local
                .Where(c => c.Slave.CTraderAccount.State == BaseAccountEntity.States.Connected &&
                            c.Slave.Master.MetaTraderAccount.State == BaseAccountEntity.States.Connected)
                .Select(c => c.Slave.Master).Distinct())
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
﻿using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Experts.Quadro.Services;
using QvaDev.Mt4Integration;

namespace QvaDev.Orchestration.Services
{

    public interface IExpertService
    {
        Task Start(DuplicatContext duplicatContext);
        void Stop();
    }

    public class ExpertService : IExpertService
    {
        private bool _isStarted;
        private readonly ILog _log;
        private readonly IQuadroService _quadroService;
        private DuplicatContext _duplicatContext;

        public ExpertService(
            IQuadroService quadroService,
            ILog log)
        {
            _quadroService = quadroService;
            _log = log;
        }

        public Task Start(DuplicatContext duplicatContext)
        {
            _duplicatContext = duplicatContext;
            _isStarted = true;
            var tasks = _duplicatContext.TradingAccounts.Local.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() => TradeAccount(account)));
            return Task.WhenAll(Task.WhenAll(tasks));
        }

        public void Stop()
        {
            _isStarted = false;
            _quadroService.Stop();
        }

        private void TradeAccount(TradingAccount tradingAccount)
        {
            var connector = tradingAccount.MetaTraderAccount.Connector as Connector;
            if (connector == null) return;

            connector.OnBarHistory -= Connector_OnBarHistory;
            connector.OnBarHistory += Connector_OnBarHistory;

            var symbols = tradingAccount.ExpertSets.Select(e => new Tuple<string, int>(e.Symbol1, e.Period))
                .Union(tradingAccount.ExpertSets.Select(e => new Tuple<string, int>(e.Symbol2, e.Period)))
                .Distinct().ToList();

            connector.Subscribe(symbols);
        }

        private void Connector_OnBarHistory(object sender, BarHistoryEventArgs e)
        {
            if (!_isStarted) return;
            foreach (var expertSet in _duplicatContext.ExpertSets.Local)
                Task.Factory.StartNew(() => _quadroService.OnBarHistory((Connector) sender, expertSet, e));
        }
    }
}

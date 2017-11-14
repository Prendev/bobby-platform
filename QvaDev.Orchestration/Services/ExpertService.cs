using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private IEnumerable<ExpertSet> _expertSets;

        public ExpertService(
            IQuadroService quadroService,
            ILog log)
        {
            _quadroService = quadroService;
            _log = log;
        }

        public Task Start(DuplicatContext duplicatContext)
        {
            _expertSets = duplicatContext.TradingAccounts.Local
                .SelectMany(ta => ta.ExpertSets)
                .Where(ta => ta.ShouldRun)
                .Distinct();
            _isStarted = true;

            var tasks = duplicatContext.TradingAccounts.Local.AsEnumerable()
                .Where(a => a.ShouldTrade)
                .Select(account => Task.Factory.StartNew(() => TradeAccount(account)));
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
            connector.OnTick -= Connector_OnTick;
            connector.OnTick += Connector_OnTick;

            var symbols1 = tradingAccount.ExpertSets
                .Where(e => e.ShouldRun)
                .Select(e => new Tuple<string, int, short>(e.Symbol1, e.TimeFrame, (short) e.GetMaxBarCount()));

            var symbols2 = tradingAccount.ExpertSets
                .Where(e => e.ShouldRun)
                .Select(e => new Tuple<string, int, short>(e.Symbol2, e.TimeFrame, (short)e.GetMaxBarCount()));

            var symbols = symbols1.Union(symbols2).Distinct().ToList();

            connector.Subscribe(symbols);
        }

        private void Connector_OnTick(object sender, TickEventArgs e)
        {
            if (!_isStarted) return;
            if (_expertSets?.Any() == false) return;
            Task.Factory.StartNew(() =>
            {
                //TODO
                try
                {
                    foreach (var expertSet in _expertSets)
                        Task.Factory.StartNew(() => _quadroService.OnTick((Connector) sender, expertSet));

                }
                catch (Exception ex)
                {
                    _log.Error("Connector_OnTick exception", ex);
                    Thread.Sleep(10);
                    Connector_OnTick(sender, e);
                }
            });
        }

        private void Connector_OnBarHistory(object sender, BarHistoryEventArgs e)
        {
            //_log.Debug($"{e.Symbol}: Connector_OnBarHistory => {e.BarHistory.Count} | {e.BarHistory.First().OpenTime}");
            if (!_isStarted) return;
            if (_expertSets?.Any() == false) return;
            Task.Factory.StartNew(() =>
            {
                //TODO
                try
                {
                    foreach (var expertSet in _expertSets)
                        Task.Factory.StartNew(() => _quadroService.OnBarHistory((Connector)sender, expertSet, e));
                }
                catch (Exception ex)
                {
                    _log.Error("Connector_OnBarHistory exception", ex);
                    Thread.Sleep(10);
                    Connector_OnBarHistory(sender, e);
                }
            });
        }
    }
}

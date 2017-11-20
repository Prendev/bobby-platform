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
        private IEnumerable<TradingAccount> _tradingAccounts;

        public ExpertService(
            IQuadroService quadroService,
            ILog log)
        {
            _quadroService = quadroService;
            _log = log;
        }

        public Task Start(DuplicatContext duplicatContext)
        {
            _tradingAccounts = duplicatContext.TradingAccounts.Local
                .Where(ta => ta.ShouldTrade).ToList();
            _isStarted = true;

            var tasks = _tradingAccounts.Select(account => Task.Factory.StartNew(() => TradeAccount(account)));
            return Task.WhenAll(Task.WhenAll(tasks));
        }

        public void Stop()
        {
            _quadroService.Stop();
            _isStarted = false;
            _tradingAccounts = null;
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
                .Select(e => new Tuple<string, int, short>(e.Symbol1, (int)e.TimeFrame, (short) e.GetMaxBarCount()));

            var symbols2 = tradingAccount.ExpertSets
                .Where(e => e.ShouldRun)
                .Select(e => new Tuple<string, int, short>(e.Symbol2, (int)e.TimeFrame, (short)e.GetMaxBarCount()));

            var symbols = symbols1.Union(symbols2).Distinct().ToList();

            connector.Subscribe(symbols);
        }

        private void Connector_OnTick(object sender, TickEventArgs e)
        {

            if (!_isStarted) return;
            if (_tradingAccounts == null || !_tradingAccounts.Any()) return;

            var connector = (Connector) sender;
            var pnl = connector.GetFloatingProfit();
            foreach (var tradingAccount in _tradingAccounts)
            {
                foreach (var expertSet in tradingAccount.ExpertSets)
                {
                    if (tradingAccount.SyncStates)
                    {
                        expertSet.SyncSellState = true;
                        expertSet.SyncBuyState = true;
                    }
                    if (tradingAccount.CloseAll)
                    {
                        expertSet.CloseAllBuy = true;
                        expertSet.CloseAllSell = true;
                    }
                    if (tradingAccount.BisectingClose)
                    {
                        expertSet.BisectingCloseBuy = true;
                        expertSet.BisectingCloseSell = true;
                    }
                    if (pnl < tradingAccount.TradeSetFloatingSwitch)
                        expertSet.TradeOpeningEnabled = false;
                }
                tradingAccount.CloseAll = false;
                tradingAccount.BisectingClose = false;
                tradingAccount.SyncStates = false;
            }

            Task.Factory.StartNew(() =>
            {
                //TODO
                try
                {
                    foreach (var expertSet in _tradingAccounts.SelectMany(ta => ta.ExpertSets)
                        .Where(es => es.ShouldRun && (es.Symbol1 == e.Tick.Symbol || es.Symbol2 == e.Tick.Symbol)))
                        Task.Factory.StartNew(() => _quadroService.OnTick((Connector) sender, expertSet));

                }
                catch (Exception ex)
                {
                    _log.Error("Connector_OnTick exception", ex);
                    Thread.Sleep(1000);
                    Connector_OnTick(sender, e);
                }
            });
        }

        private void Connector_OnBarHistory(object sender, BarHistoryEventArgs e)
        {
            //_log.Debug($"{e.Symbol}: Connector_OnBarHistory => {e.BarHistory.Count} | {e.BarHistory.First().OpenTime}");
            if (!_isStarted) return;
            if (_tradingAccounts == null || !_tradingAccounts.Any()) return;
            Task.Factory.StartNew(() =>
            {
                //TODO
                try
                {
                    foreach (var expertSet in _tradingAccounts.SelectMany(ta => ta.ExpertSets)
                        .Where(es => es.ShouldRun && (es.Symbol1 == e.Symbol || es.Symbol2 == e.Symbol)))
                        Task.Factory.StartNew(() => _quadroService.OnBarHistory((Connector) sender, expertSet, e));
                }
                catch (Exception ex)
                {
                    _log.Error("Connector_OnBarHistory exception", ex);
                    Thread.Sleep(1000);
                    Connector_OnBarHistory(sender, e);
                }
            });
        }
    }
}

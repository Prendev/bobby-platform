using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;

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
        private DuplicatContext _duplicatContext;

        public ExpertService(ILog log)
        {
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
        }

        private void TradeAccount(TradingAccount tradingAccount)
        {
            var connector = tradingAccount.MetaTraderAccount.Connector as Mt4Integration.Connector;
            if (connector == null) return;

            connector.OnBarHistory -= Connector_OnBarHistory;
            connector.OnBarHistory += Connector_OnBarHistory;

            var symbols = tradingAccount.ExpertSets.Select(e => e.Symbol1)
                .Union(tradingAccount.ExpertSets.Select(e => e.Symbol2))
                .Distinct().ToList();

            connector.Subscribe(symbols);
        }

        private void Connector_OnBarHistory(object sender, BarHistoryEventArgs e)
        {
            foreach (var expertSet in _duplicatContext.ExpertSets.Local)
            {
                lock (expertSet)
                {
                    if (expertSet.Symbol1 != e.Symbol && expertSet.Symbol2 != e.Symbol) continue;
                    if (expertSet.Symbol1 == e.Symbol) expertSet.BarHistory1 = e.BarHistory;
                    else expertSet.BarHistory2 = e.BarHistory;
                    if (expertSet.BarHistory1?.Count > 1 || expertSet.BarHistory2?.Count > 1 != true) continue;
                    if (expertSet.BarHistory1?.Last().OpenTime != expertSet.BarHistory2?.Last().OpenTime) continue;

                    Task.Factory.StartNew(() =>
                    {
                        lock (expertSet)
                        {
                            expertSet.Quants = new List<double>();
                            for (var i = 0; i < expertSet.BarHistory1.Count; i++)
                            {
                                var price1Close = MyRoundToDigits((IConnector)sender, expertSet.Symbol1, expertSet.BarHistory1[i].Close);
                                var price2Close = MyRoundToDigits((IConnector)sender, expertSet.Symbol2, expertSet.BarHistory2[i].Close);
                                var quant = MyRoundToDigits((IConnector)sender, expertSet.Symbol1, price2Close - expertSet.M * price1Close);
                                expertSet.Quants.Add(quant);
                            }
                        }
                    });
                }
            }
        }

        private double MyRoundToDigits(IConnector connector, string symbol, double value)
        {
            decimal dec = Convert.ToDecimal(value);
            dec = Math.Round(dec, connector.GetDigits(symbol), MidpointRounding.AwayFromZero);
            return Convert.ToDouble(dec);
        }
    }
}

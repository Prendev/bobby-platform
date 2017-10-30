using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
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
            connector.QuoteClient.OnQuoteHistory += QuoteClient_OnQuoteHistory;
            connector.QuoteClient.DownloadQuoteHistory("EURUSD+", TradingAPI.MT4Server.Timeframe.M15,
                DateTime.UtcNow, 100);
            connector.QuoteClient.Subscribe("EURUSD+");
            connector.QuoteClient.OnQuote += QuoteClient_OnQuote;
        }

        private void QuoteClient_OnQuote(object sender, TradingAPI.MT4Server.QuoteEventArgs args)
        {
        }

        private void QuoteClient_OnQuoteHistory(object sender, TradingAPI.MT4Server.QuoteHistoryEventArgs args)
        {
        }
    }
}

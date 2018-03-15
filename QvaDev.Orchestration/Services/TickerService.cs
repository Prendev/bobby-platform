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
    public interface ITickerService
    {
        void Start(DuplicatContext duplicatContext);
        void Stop();
    }

    public class TickerService : ITickerService
	{
        private bool _isStarted;
        private readonly ILog _log;
        private IEnumerable<Ticker> _tickers;

        public TickerService(ILog log)
        {
            _log = log;
        }

        public void Start(DuplicatContext duplicatContext)
        {
			_tickers = duplicatContext.Tickers.Local
				.Where(c => c.MetaTraderAccount?.State == BaseAccountEntity.States.Connected ||
							c.FixTraderAccount?.State == BaseAccountEntity.States.Connected);
            foreach (var ticker in _tickers)
            {
            }

            _isStarted = true;
            _log.Info("Tickers are started");
        }

        public void Stop()
        {
            _isStarted = false;
        }
    }
}

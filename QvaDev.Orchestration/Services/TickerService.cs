using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;
using FtConnector = QvaDev.FixTraderIntegration.Connector;
using MtConnector = QvaDev.Mt4Integration.Connector;

namespace QvaDev.Orchestration.Services
{
    public interface ITickerService
    {
        void Start(DuplicatContext duplicatContext);
        void Stop();
    }

    public class TickerService : ITickerService
	{
		public class CsvRow
		{
			public string Time { get; set; }
			public double Ask { get; set; }
			public double Bid { get; set; }
		}

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
				if (ticker.MetaTraderAccount != null)
				{
					var connector = (MtConnector)ticker.MetaTraderAccount.Connector;
					connector.OnTick -= Connector_OnTick;
					connector.OnTick += Connector_OnTick;
					connector.Subscribe(new List<Tuple<string, int, short>> { new Tuple<string, int, short>(ticker.Symbol, 1, 1) });
				}
				else if (ticker.FixTraderAccount != null)
				{
				}
            }

            _isStarted = true;
            _log.Info("Tickers are started");
        }

		public void Stop()
        {
            _isStarted = false;
		}

		private string GetCsvFile(MtConnector connector, string symbol)
		{
			return $"{connector.GetUser()}_{symbol}_{DateTime.UtcNow.Date:yyyyMMdd}.csv";
		}

		private void Connector_OnTick(object sender, TickEventArgs e)
		{
			var connector = (MtConnector)sender;
			connector.WriteCsv(GetCsvFile(connector, e.Tick.Symbol), new CsvRow { Time = e.Tick.Time.ToString("yyyy.MM.dd hh:mm:ss.fff"), Ask = e.Tick.Ask, Bid = e.Tick.Bid });
		}
	}
}

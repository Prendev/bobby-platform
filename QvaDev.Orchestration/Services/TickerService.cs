﻿using System;
using System.Collections.Concurrent;
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

		public class CsvRowPair
		{
			public string Time { get; set; }
			public double Ask { get; set; }
			public double Bid { get; set; }
			public double PairAsk { get; set; }
			public double PairBid { get; set; }
		}

		public class Writer
		{
			public StreamWriter StreamWriter { get; set; }
			public CsvHelper.CsvWriter CsvWriter { get; set; }

			public Writer(string file)
			{
				StreamWriter = new StreamWriter(file, true);
				CsvWriter = new CsvHelper.CsvWriter(StreamWriter);
			}

			public void Disconnect()
			{
				StreamWriter.Close();
				CsvWriter.Dispose();
			}
		}

		private bool _isStarted;
        private readonly ILog _log;
        private IEnumerable<Ticker> _tickers;

		private readonly ConcurrentDictionary<string, Writer> _csvWriters =
			new ConcurrentDictionary<string, Writer>();

		public TickerService(ILog log)
        {
            _log = log;
        }

        public void Start(DuplicatContext duplicatContext)
        {
			_tickers = duplicatContext.Tickers.Local
				.Where(c => c.FixTraderAccount?.State == BaseAccountEntity.States.Connected &&
							(c.PairMetaTraderAccount?.State == BaseAccountEntity.States.Connected ||
							c.PairFixTraderAccount?.State == BaseAccountEntity.States.Connected));

			foreach (var ticker in _tickers)
            {
				if (ticker.PairMetaTraderAccount?.Connector?.IsConnected == true)
				{
					var connector = (MtConnector)ticker.PairMetaTraderAccount.Connector;
					connector.Subscribe(new List<Tuple<string, int, short>> { new Tuple<string, int, short>(ticker.PairSymbol, 1, 1) });
				}
            }

			foreach (var ft in _tickers.Select(t => t.FixTraderAccount).Distinct())
			{
				ft.Connector.OnTick -= Connector_OnTick;
				ft.Connector.OnTick += Connector_OnTick;
			}

			_isStarted = true;
            _log.Info("Tickers are started");
        }

		public void Stop()
		{
			_isStarted = false;
			foreach (var csvWriter in _csvWriters)
				csvWriter.Value.Disconnect();
			_csvWriters.Clear();
		}

		private void Connector_OnTick(object sender, TickEventArgs e)
		{
			if (!_isStarted) return;
			var connector = (IConnector)sender;
			//WriteCsv(GetCsvFile(connector.Description, e.Tick.Symbol), new CsvRow { Time = e.Tick.Time.ToString("yyyy.MM.dd hh:mm:ss.fff"), Ask = e.Tick.Ask, Bid = e.Tick.Bid });

			foreach (var ticker in _tickers)
			{
				if (ticker.Symbol != e.Tick.Symbol) continue;
				if (ticker.FixTraderAccount?.Connector != connector) continue;

				Tick lastTick = null;
				if(ticker.PairMetaTraderAccount?.Connector?.IsConnected == true)
					lastTick = ticker.PairMetaTraderAccount.Connector.GetLastTick(ticker.PairSymbol);
				else if(ticker.PairFixTraderAccount?.Connector?.IsConnected == true)
					lastTick = ticker.PairFixTraderAccount.Connector.GetLastTick(ticker.PairSymbol);

				WriteCsv(GetCsvFile(connector.Description, e.Tick.Symbol), new CsvRowPair
				{
					Time = e.Tick.Time.ToString("yyyy.MM.dd hh:mm:ss.fff"),
					Ask = e.Tick.Ask,
					Bid = e.Tick.Bid,
					PairAsk = lastTick?.Ask ?? 0,
					PairBid = lastTick?.Bid ?? 0
				});
			}
		}

		private string GetCsvFile(string id, string symbol)
		{
			return $"{id}_{symbol}_{DateTime.UtcNow.Date:yyyyMMdd}.csv";
		}

		private void WriteCsv<T>(string file, T obj)
		{
			var writer = _csvWriters.GetOrAdd(file, key => new Writer(file));
			lock (writer)
			{
				writer.CsvWriter.WriteRecord(obj);
				writer.CsvWriter.NextRecord();
			}
		}
	}
}

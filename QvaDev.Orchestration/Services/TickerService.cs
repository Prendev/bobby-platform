﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration.Services
{
    public interface ITickerService
    {
        void Start(List<Ticker> tickers);
        void Stop();
    }

    public class TickerService : ITickerService
	{
		public class CsvRow
		{
			public string Time { get; set; }
			public decimal Ask { get; set; }
			public decimal Bid { get; set; }
		}

		public class CsvRowPair
		{
			public string Time { get; set; }
			public decimal Ask { get; set; }
			public decimal Bid { get; set; }
			public decimal PairAsk { get; set; }
			public decimal PairBid { get; set; }
		}

		public class Writer
		{
			public DateTime LastFlush { get; set; }
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

        public void Start(List<Ticker> tickers)
        {
			_tickers = tickers;

			foreach (var ticker in _tickers)
			{
				if (ticker.PairAccount == null && ticker.MainAccount.Connector is FixApiIntegration.Connector fix)	
				{
					fix.NewQuote -= Fix_NewQuote;
					fix.NewQuote += Fix_NewQuote;
				}
				else
				{
					ticker.MainAccount.Connector.NewTick -= Connector_NewTick;
					ticker.MainAccount.Connector.NewTick += Connector_NewTick;
				}

				ticker.MainAccount?.Connector?.Subscribe(ticker.MainSymbol);
				ticker.PairAccount?.Connector?.Subscribe(ticker.PairSymbol);
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

		private void Fix_NewQuote(object sender, QuoteSet quoteSet)
		{
			if (!_isStarted) return;
			var connector = (IConnector)sender;

			if (quoteSet?.Entries?.Any() != true) return;
			if (!_tickers.Any(t => t.MainSymbol == quoteSet.Symbol.ToString() || t.MainSymbol == null)) return;

			WriteQuoteCsv(GetCsvFile(connector.Description, quoteSet.Symbol.ToString()), quoteSet);
		}

		private void Connector_NewTick(object sender, NewTickEventArgs e)
		{
			if (!_isStarted) return;
			var connector = (IConnector)sender;

			if (_tickers.Any(t => t.MainSymbol == e.Tick.Symbol || t.MainSymbol == null))
				WriteCsv(GetCsvFile(connector.Description, e.Tick.Symbol),
					new CsvRow { Time = e.Tick.Time.ToString("yyyy.MM.dd hh:mm:ss.fff"), Ask = e.Tick.Ask, Bid = e.Tick.Bid });

			foreach (var ticker in _tickers)
			{
				if (ticker.MainSymbol != e.Tick.Symbol) continue;
				if (ticker.MainAccount?.Connector != connector) continue;

				Tick lastTick = null;
				string pair = "";
				if (ticker.PairAccount?.Connector?.IsConnected == true)
				{
					lastTick = ticker.PairAccount.Connector.GetLastTick(ticker.PairSymbol);
					pair = ticker.PairAccount.Connector.Description;
				}
				else continue;

				WriteCsv(GetCsvFile($"{connector.Description}_{pair}", e.Tick.Symbol), new CsvRowPair
				{
					Time = e.Tick.Time.ToString("yyyy.MM.dd HH:mm:ss.fff"),
					Ask = e.Tick.Ask,
					Bid = e.Tick.Bid,
					PairAsk = lastTick?.Ask ?? 0,
					PairBid = lastTick?.Bid ?? 0
				});
			}
		}

		private string GetCsvFile(string id, string symbol)
		{
			symbol = Path.GetInvalidFileNameChars()
				.Aggregate(symbol, (current, c) => current.Replace(c, '_'));

			return $"Tickers/{id}_{symbol}_{DateTime.UtcNow.Date:yyyyMMdd}.csv";
		}

		private void WriteCsv<T>(string file, T obj)
		{
			new FileInfo(file).Directory.Create();
			var writer = _csvWriters.GetOrAdd(file, key => new Writer(file));
			lock (writer)
			{
				writer.CsvWriter.WriteRecord(obj);
				writer.CsvWriter.NextRecord();
				if (DateTime.UtcNow - writer.LastFlush < new TimeSpan(0, 1, 0)) return;
				writer.StreamWriter.Flush();
				writer.LastFlush = DateTime.UtcNow;
			}
		}

		private void WriteQuoteCsv(string file, QuoteSet quoteSet)
		{
			new FileInfo(file).Directory.Create();
			var writer = _csvWriters.GetOrAdd(file, key => new Writer(file));
			lock (writer)
			{
				var w = writer.CsvWriter;
				w.WriteField(DateTime.UtcNow.ToString("yyyy.MM.dd hh:mm:ss.fff"));
				foreach (var qe in quoteSet.Entries)
				{
					w.WriteField(qe.Ask);
					w.WriteField(qe.Bid);
					w.WriteField(qe.AskVolume);
					w.WriteField(qe.BidVolume);
				}
				w.NextRecord();
				if (DateTime.UtcNow - writer.LastFlush < new TimeSpan(0, 1, 0)) return;
				writer.StreamWriter.Flush();
				writer.LastFlush = DateTime.UtcNow;
			}
		}
	}
}

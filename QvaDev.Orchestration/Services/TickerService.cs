using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
	    private const string ZipArchivePath = "Tickers/Tickers.zip";

		public class CsvRow : IEquatable<CsvRow>
		{
			public string Time { get; set; }
			public decimal Ask { get; set; }
			public decimal Bid { get; set; }

			public bool Equals(CsvRow other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return Ask == other.Ask && Bid == other.Bid;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((CsvRow) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (Ask.GetHashCode() * 397) ^ Bid.GetHashCode();
				}
			}
		}

		public class CsvRowPair : IEquatable<CsvRowPair>
		{
			public string Time { get; set; }
			public decimal Ask { get; set; }
			public decimal Bid { get; set; }
			public decimal PairAsk { get; set; }
			public decimal PairBid { get; set; }

			public bool Equals(CsvRowPair other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return Ask == other.Ask && Bid == other.Bid && PairAsk == other.PairAsk && PairBid == other.PairBid;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((CsvRowPair) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = Ask.GetHashCode();
					hashCode = (hashCode * 397) ^ Bid.GetHashCode();
					hashCode = (hashCode * 397) ^ PairAsk.GetHashCode();
					hashCode = (hashCode * 397) ^ PairBid.GetHashCode();
					return hashCode;
				}
			}
		}

		public class Writer
		{
			public bool Archived { get; set; }
			public DateTime LastFlush { get; set; }
			public StreamWriter StreamWriter { get; set; }
			public CsvHelper.CsvWriter CsvWriter { get; set; }
			public object LastRecord { get; set; }

			public Writer(string file)
			{
				StreamWriter = new StreamWriter(file, true);
				CsvWriter = new CsvHelper.CsvWriter(StreamWriter);
			}

			public void Disconnect()
			{
				StreamWriter.Flush();
				StreamWriter.Close();
				CsvWriter.Dispose();
			}
		}

		private bool _isStarted;
        private IEnumerable<Ticker> _tickers;

		private readonly ConcurrentDictionary<string, Writer> _csvWriters =
			new ConcurrentDictionary<string, Writer>();

        public void Start(List<Ticker> tickers)
        {
	        using (new FileStream(ZipArchivePath, FileMode.OpenOrCreate)) { }

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
	        Logger.Info("Tickers are started");
        }

		public void Stop()
		{
			_isStarted = false;
			foreach (var csvWriter in _csvWriters.Where(w => !w.Value.Archived))
				csvWriter.Value.Disconnect();
			_csvWriters.Clear();
		}

	    private void CheckArchive()
	    {
		    var now = DateTime.UtcNow;

			var toArchive = _csvWriters
			 .Where(w => !w.Value.Archived)
			 .Where(w => w.Value.LastFlush.Date.AddHours(w.Value.LastFlush.Hour) != now.Date.AddHours(now.Hour))
			 .ToList();

			if (!toArchive.Any()) return;

			using (var zipToOpen = new FileStream(ZipArchivePath, FileMode.OpenOrCreate))
		    using (var zipArchive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
			    foreach (var csvWriter in toArchive)
			    {
				    csvWriter.Value.Disconnect();
				    csvWriter.Value.Archived = true;

					var fileInfo = new FileInfo(csvWriter.Key);
				    zipArchive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);

				    File.Delete(csvWriter.Key);
			    }
	    }

	    private void Fix_NewQuote(object sender, QuoteSet quoteSet)
		{
			if (!_isStarted) return;
			CheckArchive();

			var connector = (IConnector)sender;

			if (quoteSet?.Entries?.Any() != true) return;

			var tickers = _tickers
				.Where(t => !t.PairAccountId.HasValue)
				.Where(t => t.MainAccount.Connector == connector)
				.Where(t => t.MainSymbol == quoteSet.Symbol.ToString() || t.MainSymbol == null);

			foreach (var ticker in tickers)
				WriteQuoteCsv(GetCsvFile(connector.Description, quoteSet.Symbol.ToString()), quoteSet, ticker.MarketDepth);
		}

		private void Connector_NewTick(object sender, NewTickEventArgs e)
		{
			if (!_isStarted) return;
			CheckArchive();

			var connector = (IConnector)sender;

			foreach (var ticker in _tickers)
			{
				if (ticker.MainAccount?.Connector != connector) continue;

				if (!ticker.PairAccountId.HasValue && (ticker.MainSymbol ?? e.Tick.Symbol) == e.Tick.Symbol)
					WriteCsv(GetCsvFile(connector.Description, e.Tick.Symbol),
						new CsvRow {Time = e.Tick.Time.ToString("yyyy.MM.dd hh:mm:ss.fff"), Ask = e.Tick.Ask, Bid = e.Tick.Bid});

				if (ticker.MainSymbol != e.Tick.Symbol) continue;
					

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

			return $"Tickers/{id}_{symbol}_{DateTime.UtcNow.Date:yyyyMMdd}_{DateTime.UtcNow:hh}.csv";
		}

		private void WriteCsv<T>(string file, T obj)
		{
			new FileInfo(file).Directory.Create();
			var writer = _csvWriters.GetOrAdd(file, key => new Writer(file) {LastFlush = DateTime.UtcNow});
			lock (writer)
			{
				if (obj.Equals(writer.LastRecord)) return;
				writer.LastRecord = obj;
				writer.CsvWriter.WriteRecord(obj);
				writer.CsvWriter.NextRecord();

				if (DateTime.UtcNow - writer.LastFlush < new TimeSpan(0, 1, 0)) return;
				writer.StreamWriter.Flush();
				writer.LastFlush = DateTime.UtcNow;
			}
		}

		private void WriteQuoteCsv(string file, QuoteSet quoteSet, int marketDepth)
		{
			new FileInfo(file).Directory.Create();
			var writer = _csvWriters.GetOrAdd(file, key => new Writer(file) { LastFlush = DateTime.UtcNow });
			var take = marketDepth <= 0 ? quoteSet.Entries.Count : marketDepth;
			var forLastRecord = ToLastRecord(quoteSet, take);

			lock (writer)
			{
				if (AreQuoteSetEqual(writer.LastRecord, forLastRecord)) return;
				writer.LastRecord = forLastRecord;

				var w = writer.CsvWriter;
				w.WriteField(DateTime.UtcNow.ToString("yyyy.MM.dd hh:mm:ss.fff"));
				foreach (var qe in quoteSet.Entries.Take(take))
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

	    private List<decimal?> ToLastRecord(QuoteSet quoteSet, int take)
	    {
		    var list = new List<decimal?>();

		    foreach (var qe in quoteSet.Entries.Take(take))
			    list.AddRange(new List<decimal?> { qe.Ask, qe.Bid, qe.AskVolume, qe.BidVolume });

		    return list;
	    }

		private bool AreQuoteSetEqual(object lastRecord, List<decimal?> newRecord)
	    {
		    if (lastRecord == null) return false;
		    if (!(lastRecord is List<decimal?> lr)) return false;

		    var nr = newRecord;
		    if (lr.Count != nr.Count) return false;

		    return !lr.Where((t, i) => t != nr[i]).Any();
	    }
    }
}

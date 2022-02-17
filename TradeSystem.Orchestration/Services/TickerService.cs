using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using TradeSystem.Collections;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services
{
    public interface ITickerService
    {
        void Start(List<Ticker> tickers);
        void Stop();
    }

    public class TickerService : ITickerService
	{
		public class TickOrQuote
		{
			public object Sender { get; set; }
			public NewTick NewTick { get; set; }
			public QuoteSet QuoteSet { get; set; }
		}

		public class CsvRow : IEquatable<CsvRow>
		{
			public string Time { get; set; }
			public string Ask { get; set; }
			public string Bid { get; set; }

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
			public string Ask { get; set; }
			public string Bid { get; set; }
			public string PairAsk { get; set; }
			public string PairBid { get; set; }

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

			public Writer(string file, string delimeter)
			{
				StreamWriter = new StreamWriter(file, true);
				CsvWriter = new CsvHelper.CsvWriter(StreamWriter);
				CsvWriter.Configuration.Delimiter = delimeter;
			}

			public void Disconnect()
			{
				StreamWriter.Flush();
				StreamWriter.Close();
				CsvWriter.Dispose();
			}
		}

		private volatile CancellationTokenSource _cancellation;
		private const string TickersFolderPath = "Tickers";
		private const string ArchiveFolderPath = "Tickers/Archive";
		private readonly ConcurrentDictionary<string, Writer> _csvWriters =
			new ConcurrentDictionary<string, Writer>();

        public void Start(List<Ticker> tickers)
		{
			_cancellation?.Dispose();
			_cancellation = new CancellationTokenSource();

			Directory.CreateDirectory(TickersFolderPath);
			Directory.CreateDirectory(ArchiveFolderPath);
			new Thread(() => Loop(tickers, _cancellation.Token))
			{
				Name = "Tickers",
				IsBackground = true,
				Priority = ThreadPriority.BelowNormal
			}.Start();
			Logger.Info("Tickers are started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("Tickers are stopped");
		}

		private void Loop(List<Ticker> tickers, CancellationToken token)
		{
			using (var queue = new FastBlockingCollection<TickOrQuote>())
			{
				void FixOnNewQuote(object sender, QuoteSet quoteSet)
				{
					if (token.IsCancellationRequested) return;
					queue.Add(new TickOrQuote {Sender = sender, QuoteSet = quoteSet});
				}

				void MainAccountOnNewTick(object sender, NewTick newTick)
				{
					if (token.IsCancellationRequested) return;
					queue.Add(new TickOrQuote() {Sender = sender, NewTick = newTick});
				}

				try
				{
					// Subscribe to events
					foreach (var ticker in tickers)
					{
						if (ticker.PairAccount == null && ticker.MainAccount.Connector is FixApiIntegration.Connector fix)
							fix.NewQuote += FixOnNewQuote;
						else ticker.MainAccount.NewTick += MainAccountOnNewTick;

						ticker.MainAccount?.Connector?.Subscribe(ticker.MainSymbol);
						ticker.PairAccount?.Connector?.Subscribe(ticker.PairSymbol);
					}

					while (!token.IsCancellationRequested)
					{
						Check(tickers, queue.Take(token));
						CheckArchive();
					}
				}
				catch (OperationCanceledException) { }
				catch (Exception e)
				{
					Logger.Error("TickerService.Loop exception", e);
				}
				finally
				{
					foreach (var ticker in tickers)
					{
						if (ticker.MainAccount.Connector is FixApiIntegration.Connector fix)
							fix.NewQuote -= FixOnNewQuote;
						ticker.MainAccount.NewTick -= MainAccountOnNewTick;
					}

					foreach (var csvWriter in _csvWriters.Where(w => !w.Value.Archived))
						csvWriter.Value.Disconnect();
					_csvWriters.Clear();
				}
			}
		}

		private void Check(List<Ticker> tickers, TickOrQuote newTickOrQuote)
		{
			try
			{
				if (newTickOrQuote.Sender == null) return;

				var activeTickers = tickers.Where(t => t.Run).ToList();

				if (newTickOrQuote.QuoteSet != null)
					Fix_NewQuote(newTickOrQuote.Sender, newTickOrQuote.QuoteSet, activeTickers);

				if (newTickOrQuote.NewTick != null)
					Account_NewTick(newTickOrQuote.Sender, newTickOrQuote.NewTick, activeTickers);
			}
			catch (Exception e)
			{
				Logger.Error("TickerService.Check exception", e);
			}
		}

		private void CheckArchive()
		{
			try
			{
				var now = HiResDatetime.UtcNow;
				if (now.Minute < 45) return;

				var toArchive = _csvWriters
					.Where(w => !w.Value.Archived)
					.Where(w => w.Value.LastFlush.Date.AddHours(w.Value.LastFlush.Hour) != now.Date.AddHours(now.Hour))
					.ToList();

				if (!toArchive.Any()) return;

				toArchive = toArchive.Where(w => !w.Value.Archived).ToList();
				toArchive.ForEach(a => a.Value.Archived = true);
				if (!toArchive.Any()) return;

				foreach (var csvWriter in toArchive)
				{
					csvWriter.Value.Disconnect();
					var fileInfo = new FileInfo(csvWriter.Key);
					var archive = $"{ArchiveFolderPath}/{Path.GetFileNameWithoutExtension(fileInfo.FullName)}.zip";
					using (var zipToOpen = new FileStream(archive, FileMode.OpenOrCreate))
					using (var zipArchive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
						zipArchive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
					File.Delete(csvWriter.Key);
				}
			}
			catch (Exception e)
			{
				Logger.Error("TickerService.CheckArchive exception", e);
			}
		}

		private void Fix_NewQuote(object sender, QuoteSet quoteSet, List<Ticker> tickers)
	    {
		    if (!(sender is IConnector connector)) return;
			if (quoteSet?.Entries?.Any() != true) return;

		    foreach (var ticker in tickers)
		    {
				if (ticker.MainAccount?.Connector != connector) continue;
				if (ticker.PairAccount != null) continue;
				if (!(ticker.MainAccount.Connector is FixApiIntegration.Connector)) continue;
				if (!string.IsNullOrWhiteSpace(ticker.MainSymbol) && ticker.MainSymbol != quoteSet.Symbol.ToString())
				{
					Logger.Error($"TickerService.Fix_NewQuote symbol mismatch {ticker.MainSymbol} {quoteSet.Symbol}");
					continue;
				}
			    WriteQuoteCsv(ticker, GetCsvFile(ticker, connector.Description, quoteSet.Symbol.ToString()), quoteSet, ticker.MarketDepth);
		    }
		}

		private void Account_NewTick(object sender, NewTick e, List<Ticker> tickers)
		{
			var connector = (sender as Account)?.Connector;
			if (connector == null) return;

			foreach (var ticker in tickers)
			{
				if (ticker.MainAccount?.Connector != connector) continue;
				if (ticker.PairAccount == null && ticker.MainAccount.Connector is FixApiIntegration.Connector)
					continue;

				if (!ticker.PairAccountId.HasValue && (ticker.MainSymbol ?? e.Tick.Symbol) == e.Tick.Symbol)
					WriteCsv(ticker, GetCsvFile(ticker, connector.Description, e.Tick.Symbol),
						new CsvRow
						{
							Time = e.Tick.Time.ToString(ticker.GetDateTimeFormat()),
							Ask = e.Tick.Ask.ToString(CultureInfo.InvariantCulture),
							Bid = e.Tick.Bid.ToString(CultureInfo.InvariantCulture)
						});

				if (ticker.MainSymbol != e.Tick.Symbol) continue;

				Tick lastTick;
				string pair;
				if (ticker.PairAccount?.Connector?.IsConnected == true)
				{
					lastTick = ticker.PairAccount.Connector.GetLastTick(ticker.PairSymbol);
					pair = ticker.PairAccount.Connector.Description;
				}
				else continue;

				WriteCsv(ticker, GetCsvFile(ticker, $"{connector.Description}_{pair}", e.Tick.Symbol), new CsvRowPair
				{
					Time = e.Tick.Time.ToString(ticker.GetDateTimeFormat()),
					Ask = e.Tick.Ask.ToString(CultureInfo.InvariantCulture),
					Bid = e.Tick.Bid.ToString(CultureInfo.InvariantCulture),
					PairAsk = (lastTick?.Ask ?? 0).ToString(CultureInfo.InvariantCulture),
					PairBid = (lastTick?.Bid ?? 0).ToString(CultureInfo.InvariantCulture)
				});
			}
		}

		private string GetCsvFile(Ticker ticker, string id, string symbol)
		{
			symbol = Path.GetInvalidFileNameChars()
				.Aggregate(symbol, (current, c) => current.Replace(c, '_'));

			return $"Tickers/{id}_{symbol}_{HiResDatetime.UtcNow.Date:yyyyMMdd}_{HiResDatetime.UtcNow:HH}.{ticker.GetExtension()}";
		}

		private void WriteCsv<T>(Ticker ticker, string file, T obj)
		{
			new FileInfo(file).Directory.Create();
			var writer = _csvWriters.GetOrAdd(file, key => new Writer(file, ticker.GetDelimeter()) {LastFlush = HiResDatetime.UtcNow});
			lock (writer)
			{
				if (obj.Equals(writer.LastRecord)) return;
				writer.LastRecord = obj;
				writer.CsvWriter.WriteRecord(obj);
				writer.CsvWriter.NextRecord();

				if (HiResDatetime.UtcNow - writer.LastFlush < new TimeSpan(0, 1, 0)) return;
				writer.StreamWriter.Flush();
				writer.LastFlush = HiResDatetime.UtcNow;
			}
		}

		private void WriteQuoteCsv(Ticker ticker, string file, QuoteSet quoteSet, int marketDepth)
		{
			new FileInfo(file).Directory.Create();
			var writer = _csvWriters.GetOrAdd(file, key => new Writer(file, ticker.GetDelimeter()) { LastFlush = HiResDatetime.UtcNow });
			var take = marketDepth <= 0 ? quoteSet.Entries.Count : marketDepth;
			var forLastRecord = ToLastRecord(quoteSet, take);

			lock (writer)
			{
				if (AreQuoteSetEqual(writer.LastRecord, forLastRecord)) return;
				writer.LastRecord = forLastRecord;

				var w = writer.CsvWriter;
				w.WriteField(HiResDatetime.UtcNow.ToString(ticker.GetDateTimeFormat()));
				var first = true;
				foreach (var qe in quoteSet.Entries.Take(take))
				{
					if (first)
					{
						first = false;
						w.WriteField(qe.Trade?.ToString(CultureInfo.InvariantCulture));
						w.WriteField(qe.TradeVolume?.ToString(CultureInfo.InvariantCulture));
						w.WriteField(qe.EntryDateTime);
					}
					w.WriteField(qe.Ask?.ToString(CultureInfo.InvariantCulture));
					w.WriteField(qe.Bid?.ToString(CultureInfo.InvariantCulture));
					w.WriteField(qe.AskVolume?.ToString(CultureInfo.InvariantCulture));
					w.WriteField(qe.BidVolume?.ToString(CultureInfo.InvariantCulture));
				}
				w.NextRecord();

				if (HiResDatetime.UtcNow - writer.LastFlush < new TimeSpan(0, 1, 0)) return;
				writer.StreamWriter.Flush();
				writer.LastFlush = HiResDatetime.UtcNow;
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

			return lr.Where((t, i) => t != nr[i]).Count() == 0;
	    }
    }
}

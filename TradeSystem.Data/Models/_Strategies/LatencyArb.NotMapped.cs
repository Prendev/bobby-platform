using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public partial class LatencyArb
	{
		public class Statistics
		{
			public string Groups { get; set; }
			public int Total { get; set; }
			public decimal? AvgPip { get; set; }

			public string Accounts { get; set; }
			public decimal? LivePip { get; set; }
			public decimal? LivePnl { get; set; }
			public decimal? ClosedPip { get; set; }
			public decimal? ClosedPnl { get; set; }

			public string Prices { get; set; }
			public decimal? Ask { get; set; }
			public decimal? Bid { get; set; }
			public decimal? NormAsk { get; set; }
			public decimal? NormBid { get; set; }
			public decimal? Spread { get; set; }
			public decimal? AvgPrice { get; set; }
			public decimal? OpenDiffPip { get; set; }
			public decimal? CloseDiffPip { get; set; }
		}

		public class LatencyArbPos
		{
			public long? ShortTicket { get; set; }
			public long? LongTicket { get; set; }
			public decimal? MinMax { get; set; }
		}

		private readonly List<Tick> _feedTicks = new List<Tick>();
		private readonly List<Tick> _shortTicks = new List<Tick>();
		private readonly List<Tick> _longTicks = new List<Tick>();

		public event EventHandler<NewTick> NewTick;
		public event EventHandler<ConnectionStates> ConnectionChanged;

		[NotMapped]
		[InvisibleColumn]
		public bool IsConnected
		{
			get => Get<bool>();
			set => Set(value);
		}

		[NotMapped] [InvisibleColumn] public DateTime? LastActionTime { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastFeedTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastShortTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastLongTick { get; set; }

		[NotMapped] [InvisibleColumn] public decimal? FeedSpread => (LastFeedTick?.Ask - LastFeedTick?.Bid) / PipSize;
		[NotMapped] [InvisibleColumn] public decimal? ShortSpread => (LastShortTick?.Ask - LastShortTick?.Bid) / PipSize;
		[NotMapped] [InvisibleColumn] public decimal? LongSpread => (LastLongTick?.Ask - LastLongTick?.Bid) / PipSize;

		[NotMapped][InvisibleColumn] public bool FastFeedSpreadCheck => SpreadCheck(LastFeedTick, FastFeedSpreadFilterInPip);
		[NotMapped] [InvisibleColumn] public bool ShortSpreadCheck => SpreadCheck(LastShortTick, ShortSpreadFilterInPip);
		[NotMapped] [InvisibleColumn] public bool LongSpreadCheck => SpreadCheck(LastLongTick, LongSpreadFilterInPip);

		[NotMapped] [InvisibleColumn] public decimal? FeedAvg { get; set; }
		[NotMapped] [InvisibleColumn] public decimal? ShortAvg { get; set; }
		[NotMapped] [InvisibleColumn] public decimal? LongAvg { get; set; }

		[NotMapped] [InvisibleColumn] public decimal? NormFeedAsk => NormAsk(LastFeedTick, FeedAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormFeedBid => NormBid(LastFeedTick, FeedAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormShortAsk => NormAsk(LastShortTick, ShortAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormShortBid => NormBid(LastShortTick, ShortAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormLongAsk => NormAsk(LastLongTick, LongAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormLongBid => NormBid(LastLongTick, LongAvg);
		[NotMapped] [InvisibleColumn] public bool HasPrices => NormFeedAsk.HasValue && NormFeedBid.HasValue &&
		                                                       NormShortAsk.HasValue && NormShortBid.HasValue &&
		                                                       NormLongAsk.HasValue && NormLongBid.HasValue;

		[NotMapped] [InvisibleColumn] public bool HasTiming => EarliestTradeTime.HasValue && LatestTradeTime.HasValue;
		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;
		[NotMapped] [InvisibleColumn] public decimal Diff => PriceDiffInPip * PipSize;
		[NotMapped] public List<LatencyArbPosition> LivePositions => LatencyArbPositions.Where(p => !p.Archived).ToList();
		[NotMapped] [InvisibleColumn] public DateTime LastPnlTime { get; set; }
		[NotMapped] [InvisibleColumn] public decimal? LivePnl { get; set; }
		[NotMapped] [InvisibleColumn] public decimal? ClosedPnl { get; set; }

		[NotMapped][InvisibleColumn] public string ShortComment2 => string.IsNullOrWhiteSpace(ShortComment) ? Comment : ShortComment;
		[NotMapped][InvisibleColumn] public string LongComment2 => string.IsNullOrWhiteSpace(LongComment) ? Comment : LongComment;

		[NotMapped] [InvisibleColumn] public Stopwatch Stopwatch { get; } = new Stopwatch();

		[NotMapped] [InvisibleColumn] public DateTime UtcNow =>
			FastFeedAccount.BacktesterAccount?.UtcNow ?? HiResDatetime.UtcNow;
		[NotMapped] [InvisibleColumn] public AutoResetEvent WaitHandle { get; } = new AutoResetEvent(false);

		public LatencyArb()
		{
			SetAction<Account>(nameof(FastFeedAccount),
				a =>
				{
					if (a == null) return;
					a.ConnectionChanged -= Account_ConnectionChanged;
					a.NewTick -= Account_NewTick;
				},
				a =>
				{
					if (a == null) return;
					a.ConnectionChanged += Account_ConnectionChanged;
					a.NewTick += Account_NewTick;
				});
			SetAction<Account>(nameof(ShortAccount),
				a =>
				{
					if (a == null) return;
					a.ConnectionChanged -= Account_ConnectionChanged;
					a.NewTick -= Account_NewTick;
				},
				a =>
				{
					if (a == null) return;
					a.ConnectionChanged += Account_ConnectionChanged;
					a.NewTick += Account_NewTick;
				});
			SetAction<Account>(nameof(LongAccount),
				a =>
				{
					if (a == null) return;
					a.ConnectionChanged -= Account_ConnectionChanged;
					a.NewTick -= Account_NewTick;
				},
				a =>
				{
					if (a == null) return;
					a.ConnectionChanged += Account_ConnectionChanged;
					a.NewTick += Account_NewTick;
				});
		}

		public void OnTickProcessed() => FastFeedAccount.Connector.OnTickProcessed();

		public void Reset()
		{
			LastActionTime = null;
			LastFeedTick = null;
			LastLongTick = null;
			LastShortTick = null;
			FeedAvg = null;
			LongAvg = null;
			ShortAvg = null;
			_feedTicks.Clear();
			_longTicks.Clear();
			_shortTicks.Clear();
		}

		private void Account_NewTick(object sender, NewTick newTick)
		{
			var tick = newTick?.Tick;
			if (tick == null) return;

			var newTickFound = false;
			if (sender == FastFeedAccount && tick.Symbol == FastFeedSymbol && LastFeedTick != tick)
			{
				newTickFound = true;
				LastFeedTick = tick;
				FeedAvg = Averaging(_feedTicks, FeedAvg, LastFeedTick);
			}

			if (sender == ShortAccount && tick.Symbol == ShortSymbol && LastShortTick != tick)
			{
				newTickFound = true;
				LastShortTick = tick;
				ShortAvg = Averaging(_shortTicks, ShortAvg, LastShortTick);
			}

			if (sender == LongAccount && tick.Symbol == LongSymbol && LastLongTick != tick)
			{
				newTickFound = true;
				LastLongTick = tick;
				LongAvg = Averaging(_longTicks, LongAvg, LastLongTick);
			}

			if (!newTickFound) return;

			if (EmergencyPnlPeriodInSec > 0 && UtcNow > LastPnlTime.AddSeconds(EmergencyPnlPeriodInSec))
				CalculateStatistics();

			NewTick?.Invoke(this, newTick);
		}

		private decimal? Averaging(List<Tick> ticks, decimal? oldAvg, Tick lastTick)
		{
			if (AveragingPeriodInSeconds <= 0) return null;
			if (State == LatencyArbStates.Reset) return null;
			if (State == LatencyArbStates.ResetOpening) return null;

			lock (ticks)
			{
				ticks.Add(lastTick);
				var avg = oldAvg;

				var doAvg = false;
				while (ticks.Any() && lastTick.Time - ticks.First().Time >
				       TimeSpan.FromSeconds(AveragingPeriodInSeconds))
				{
					ticks.RemoveAt(0);
					doAvg = true;
				}

				if (doAvg || oldAvg.HasValue) avg = ticks.Select(t => t.Ask + t.Bid).Average() / 2;

				return avg;
			}
		}

		private void Account_ConnectionChanged(object sender, ConnectionStates connectionStates)
		{
			IsConnected =
				FastFeedAccount?.Connector?.IsConnected == true &&
				ShortAccount?.Connector?.IsConnected == true &&
				LongAccount?.Connector?.IsConnected == true;
			ConnectionChanged?.Invoke(this, IsConnected ? ConnectionStates.Connected : ConnectionStates.Disconnected);
		}

		public decimal GetAvgClosedPip()
		{
			var closedPositions = LatencyArbPositions.Where(p => p.IsFull).ToList();
			var avgClosed = closedPositions.Sum(p => p.Result) / Math.Max(1, closedPositions.Count) / PipSize;
			return avgClosed ?? 0;
		}



		public IList CalculateStatistics()
		{
			if (PipSize == 0)
			{
				Logger.Error($"{this} latency arb - {nameof(PipSize)} cannot be 0");
				return null;
			}

			var closedPositions = LatencyArbPositions.Where(p => p.IsFull).ToList();
			var avgClosed = 2 * closedPositions.Sum(p => p.Result) / Math.Max(1, closedPositions.Count) / PipSize;
			var avgClosedLong = closedPositions.Sum(p => p.LongResult) / Math.Max(1, closedPositions.Count) / PipSize;
			var avgClosedLongPnl = closedPositions.Sum(p => p.LongPnl(this));
			var avgClosedShort = closedPositions.Sum(p => p.ShortResult) / Math.Max(1, closedPositions.Count) / PipSize;
			var avgClosedShortPnl = closedPositions.Sum(p => p.ShortPnl(this));
			ClosedPnl = avgClosedLongPnl + avgClosedShortPnl;

			var livePositions = LivePositions.Where(p => p.HasBothSides).ToList();
			var avgHedge = livePositions.Sum(p => p.OpenResult) / Math.Max(1, livePositions.Count) / PipSize;
			var normAvgHedge = livePositions.Sum(p => p.NormOpenResult(ShortAvg, LongAvg)) / Math.Max(1, livePositions.Count) / PipSize;

			decimal? avgLiveLong = null;
			decimal? avgLiveLongPnl = null;
			decimal? avgLiveShort = null;
			decimal? avgLiveShortPnl = null;
			LivePnl = null;

			if (LastLongTick?.HasValue == true && LastShortTick?.HasValue == true)
			{
				avgLiveLong = (LastLongTick.Bid - livePositions.Average(p => p.LongOpenPrice)) / PipSize;
				avgLiveLongPnl = livePositions.Sum(p => p.LongPnl(this));
				avgLiveShort = (livePositions.Average(p => p.ShortOpenPrice) - LastShortTick.Ask) / PipSize;
				avgLiveShortPnl = livePositions.Sum(p => p.ShortPnl(this));
				LivePnl = avgLiveLongPnl + avgLiveShortPnl;
			}

			var statistics = new List<Statistics>()
			{
				new Statistics()
				{
					Groups = "--Norm--",
					Total = LatencyArbPositions.Count,
					AvgPip = normAvgHedge,

					Accounts = "--All--",
					LivePip = avgLiveLong + avgLiveShort,
					LivePnl = LivePnl,
					ClosedPip = avgClosed,
					ClosedPnl = ClosedPnl,

					Prices = "--Feed--",
					Ask = LastFeedTick?.Ask,
					Bid = LastFeedTick?.Bid,
					NormAsk = NormFeedAsk,
					NormBid = NormFeedBid,
					Spread = (LastFeedTick?.Bid - LastFeedTick?.Ask) / PipSize,
					AvgPrice = FeedAvg
				},
				new Statistics()
				{
					Groups = "--Hedged--",
					Total = LivePositions.Count,
					AvgPip = avgHedge,

					Accounts = "--Long--",
					LivePip = avgLiveLong,
					LivePnl = avgLiveLongPnl,
					ClosedPip = avgClosedLong,
					ClosedPnl = avgClosedLongPnl,

					Prices = "--Long--",
					Ask = LastLongTick?.Ask,
					Bid = LastLongTick?.Bid,
					NormAsk = NormLongAsk,
					NormBid = NormLongBid,
					Spread = (LastLongTick?.Bid - LastLongTick?.Ask) / PipSize,
					AvgPrice = LongAvg,
					OpenDiffPip = (LastFeedTick?.Ask - LastLongTick?.Ask - (FeedAvg ?? 0) + (LongAvg ?? 0)) / PipSize,
					CloseDiffPip = (LastLongTick?.Bid - LastFeedTick?.Bid + (FeedAvg ?? 0) - (LongAvg ?? 0)) / PipSize
				},
				new Statistics()
				{
					Groups = "--Closed--",
					Total = closedPositions.Count,
					AvgPip = avgClosed,

					Accounts = "--Short--",
					LivePip = avgLiveShort,
					LivePnl = avgLiveShortPnl,
					ClosedPip = avgClosedShort,
					ClosedPnl = avgClosedShortPnl,

					Prices = "--Short--",
					Ask = LastShortTick?.Ask,
					Bid = LastShortTick?.Bid,
					NormAsk = NormShortAsk,
					NormBid = NormShortBid,
					Spread = (LastShortTick?.Bid - LastShortTick?.Ask) / PipSize,
					AvgPrice = ShortAvg,
					OpenDiffPip = (LastShortTick?.Bid - LastFeedTick?.Bid + (FeedAvg ?? 0) - (ShortAvg ?? 0)) / PipSize,
					CloseDiffPip = (LastFeedTick?.Ask - LastShortTick?.Ask - (FeedAvg ?? 0) + (ShortAvg ?? 0)) / PipSize
				}
			};

			LastPnlTime = UtcNow;

			return statistics.Select(s => new
			{
				s.Groups,
				Total = s.Total.ToString("0"),
				AvgPip = s.AvgPip?.ToString("F2"),

				s.Accounts,
				LivePip = s.LivePip?.ToString("F2"),
				LivePnl = s.LivePnl?.ToString("F2"),
				ClosedPip = s.ClosedPip?.ToString("F2"),
				ClosedPnl = s.ClosedPnl?.ToString("F2"),

				s.Prices,
				Ask = s.Ask?.ToString("F5"),
				Bid = s.Bid?.ToString("F5"),
				NormAsk = s.NormAsk?.ToString("F5"),
				NormBid = s.NormBid?.ToString("F5"),
				Spread = s.Spread?.ToString("F2"),
				AvgPrice = s.AvgPrice?.ToString("F5"),
				OpenDiff = s.OpenDiffPip?.ToString("F2"),
				CloseDiff = s.CloseDiffPip?.ToString("F2"),
			}).ToList();
		}

		private decimal? NormAsk(Tick lastTick, decimal? avg)
		{
			if (lastTick?.HasValue != true) return null;
			var price = lastTick.Ask;
			if (AveragingPeriodInSeconds <= 0) return price;
			return price - avg;
		}
		private decimal? NormBid(Tick lastTick, decimal? avg)
		{
			if (lastTick?.HasValue != true) return null;
			var price = lastTick.Bid;
			if (AveragingPeriodInSeconds <= 0) return price;
			return price - avg;
		}

		private bool SpreadCheck(Tick lastTick, decimal spread)
		{
			if (lastTick?.HasValue != true) return false;
			if (spread <= 0) return true;
			return lastTick.Ask - lastTick.Bid <= spread * PipSize;
		}
	}
}

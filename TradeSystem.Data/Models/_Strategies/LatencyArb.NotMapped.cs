using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public partial class LatencyArb
	{
		public class Statistics
		{
			public string Group { get; set; }
			public int Total { get; set; }
			public decimal? AvgPip { get; set; }
			public decimal? NormAvgPip { get; set; }
			public string Account { get; set; }
			public decimal? Ask { get; set; }
			public decimal? Bid { get; set; }
			public decimal? NormAsk { get; set; }
			public decimal? NormBid { get; set; }
			public decimal? Spread { get; set; }
			public decimal? AvgPrice { get; set; }
			public decimal? OpenPip { get; set; }
			public decimal? ClosePip { get; set; }
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
		[NotMapped] public List<LatencyArbPosition> LivePositions => LatencyArbPositions.Where(p => !p.Archived).ToList();

		[NotMapped] [InvisibleColumn] public Stopwatch Stopwatch { get; } = new Stopwatch();

		[NotMapped]
		[InvisibleColumn]
		public bool IsBacktest => FastFeedAccount.BacktesterAccount != null;
		[NotMapped]
		[InvisibleColumn]
		public DateTime UtcNow => IsBacktest ? FastFeedAccount.BacktesterAccount.UtcNow : HiResDatetime.UtcNow;

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

		private void Account_NewTick(object sender, NewTick newTick)
		{
			if (newTick?.Tick == null) return;

			var newTickFound = false;
			if (sender == FastFeedAccount && newTick.Tick.Symbol == FastFeedSymbol)
			{
				newTickFound = true;
				LastFeedTick = newTick.Tick;
				FeedAvg = Averaging(_feedTicks, FeedAvg, LastFeedTick);
			}
			if (sender == ShortAccount && newTick.Tick.Symbol == ShortSymbol)
			{
				newTickFound = true;
				LastShortTick = newTick.Tick;
				ShortAvg = Averaging(_shortTicks, ShortAvg, LastShortTick);
			}
			if (sender == LongAccount && newTick.Tick.Symbol == LongSymbol)
			{
				newTickFound = true;
				LastLongTick = newTick.Tick;
				LongAvg = Averaging(_longTicks, LongAvg, LastLongTick);
			}

			if (!newTickFound) return;
			NewTick?.Invoke(this, newTick);
		}

		private decimal? Averaging(List<Tick> ticks, decimal? oldAvg, Tick lastTick)
		{
			var avg = oldAvg;
			if (AveragingPeriodInSeconds <= 0) return null;

			lock (ticks)
			{
				var doAvg = false;
				while (ticks.Any() && lastTick.Time - ticks.First().Time >
				       TimeSpan.FromSeconds(AveragingPeriodInSeconds))
				{
					ticks.RemoveAt(0);
					doAvg = true;
				}
				ticks.Add(lastTick);
				if (doAvg || oldAvg.HasValue) avg = ticks.Select(t => (t.Ask + t.Bid) / 2).Average();
			}

			return avg;
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
			var avgClosed = closedPositions.Sum(p => p.Result) / Math.Max(1, closedPositions.Count) / PipSize;

			var livePositions = LivePositions.Where(p => p.HasBothSides).ToList();
			var avgLive = livePositions.Sum(p => p.OpenResult) / Math.Max(1, livePositions.Count) / PipSize;
			var normAvgLive = livePositions.Sum(p => p.NormOpenResult(ShortAvg, LongAvg)) / Math.Max(1, livePositions.Count) / PipSize;

			var statistics = new List<Statistics>()
			{
				new Statistics()
				{
					Group = "All",
					Total = LatencyArbPositions.Count,
					Account = "Feed",
					AvgPrice = FeedAvg,
					Ask = LastFeedTick?.Ask,
					Bid = LastFeedTick?.Bid,
					NormAsk = NormFeedAsk,
					NormBid = NormFeedBid,
					Spread = (LastFeedTick?.Bid - LastFeedTick?.Ask) / PipSize
				},
				new Statistics()
				{
					Group = "Live",
					Total = LivePositions.Count,
					AvgPip = avgLive,
					NormAvgPip = normAvgLive,
					Account = "Long",
					AvgPrice = LongAvg,
					Ask = LastLongTick?.Ask,
					Bid = LastLongTick?.Bid,
					NormAsk = NormLongAsk,
					NormBid = NormLongBid,
					Spread = (LastLongTick?.Bid - LastLongTick?.Ask) / PipSize,
					OpenPip = (LastFeedTick?.Ask - LastLongTick?.Ask - (FeedAvg ?? 0) + (LongAvg ?? 0)) / PipSize,
					ClosePip = (LastLongTick?.Bid - LastFeedTick?.Bid + (FeedAvg ?? 0) - (LongAvg ?? 0)) / PipSize
				},
				new Statistics()
				{
					Group = "Closed",
					Total = closedPositions.Count,
					AvgPip = avgClosed,
					Account = "Short",
					AvgPrice = ShortAvg,
					Ask = LastShortTick?.Ask,
					Bid = LastShortTick?.Bid,
					NormAsk = NormShortAsk,
					NormBid = NormShortBid,
					Spread = (LastShortTick?.Bid - LastShortTick?.Ask) / PipSize,
					OpenPip = (LastShortTick?.Bid - LastFeedTick?.Bid + (FeedAvg ?? 0) - (ShortAvg ?? 0)) / PipSize,
					ClosePip = (LastFeedTick?.Ask - LastShortTick?.Ask - (FeedAvg ?? 0) + (ShortAvg ?? 0)) / PipSize
				}
			};

			return statistics.Select(s => new
			{
				Group = s.Group,
				Total = s.Total.ToString("0"),
				AvgPip = s.AvgPip?.ToString("F2"),
				III = s.NormAvgPip?.ToString("F2"),
				Account = s.Account,
				Ask = s.Ask?.ToString("F5"),
				Bid = s.Bid?.ToString("F5"),
				NormAsk = s.NormAsk?.ToString("F5"),
				NormBid = s.NormBid?.ToString("F5"),
				Spread = s.Spread?.ToString("F2"),
				AvgPrice = s.AvgPrice?.ToString("F5"),
				OpenDiff = s.OpenPip?.ToString("F2"),
				CloseDiff = s.ClosePip?.ToString("F2"),
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

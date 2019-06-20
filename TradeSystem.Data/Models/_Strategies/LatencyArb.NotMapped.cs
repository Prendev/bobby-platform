using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
			public string Account { get; set; }
			public decimal? Ask { get; set; }
			public decimal? Bid { get; set; }
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
		private decimal? _feedAvg = null;
		private decimal? _shortAvg = null;
		private decimal? _longAvg = null;

		public event EventHandler<NewTick> NewTick;
		public event EventHandler<ConnectionStates> ConnectionChanged;

		[NotMapped]
		[InvisibleColumn]
		public bool IsConnected
		{
			get => Get<bool>();
			set => Set(value);
		}

		[NotMapped] [InvisibleColumn] public Tick LastFeedTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastShortTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastLongTick { get; set; }
		[NotMapped] [InvisibleColumn] public bool ShortSpreadCheck => SpreadCheck(LastShortTick, ShortSpreadFilterInPip);
		[NotMapped] [InvisibleColumn] public bool LongSpreadCheck => SpreadCheck(LastLongTick, LongSpreadFilterInPip);

		[NotMapped] [InvisibleColumn] public decimal? NormFeedAsk => NormAsk(LastFeedTick, _feedAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormFeedBid => NormBid(LastFeedTick, _feedAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormShortAsk => NormAsk(LastShortTick, _shortAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormShortBid => NormBid(LastShortTick, _shortAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormLongAsk => NormAsk(LastLongTick, _longAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormLongBid => NormBid(LastLongTick, _longAvg);
		[NotMapped] [InvisibleColumn] public bool HasPrices => NormFeedAsk.HasValue && NormFeedBid.HasValue &&
		                                                       NormShortAsk.HasValue && NormShortBid.HasValue &&
		                                                       NormLongAsk.HasValue && NormLongBid.HasValue;

		[NotMapped] [InvisibleColumn] public bool HasTiming => EarliestTradeTime.HasValue && LatestTradeTime.HasValue;
		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;
		[NotMapped] public List<LatencyArbPosition> LivePositions => LatencyArbPositions.Where(p => !p.Archived).ToList();

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
			if (sender == FastFeedAccount && newTick.Tick.Symbol != FastFeedSymbol) return;
			if (sender == ShortAccount && newTick.Tick.Symbol != ShortSymbol) return;
			if (sender == LongAccount && newTick.Tick.Symbol != LongSymbol) return;

			if (sender == FastFeedAccount) LastFeedTick = newTick.Tick;
			if (sender == ShortAccount) LastShortTick = newTick.Tick;
			if (sender == LongAccount) LastLongTick = newTick.Tick;

			if (sender == FastFeedAccount) _feedAvg = Averaging(_feedTicks, _feedAvg, LastFeedTick);
			if (sender == ShortAccount) _shortAvg = Averaging(_shortTicks, _shortAvg, LastShortTick);
			if (sender == LongAccount) _longAvg = Averaging(_longTicks, _longAvg, LastLongTick);

			if (LastFeedTick?.HasValue != true) return;
			if (LastShortTick?.HasValue != true) return;
			if (LastLongTick?.HasValue != true) return;

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

		public IList CalculateStatistics()
		{
			var closedPositions = LatencyArbPositions.Where(p => p.IsFull).ToList();
			var avgClosed = closedPositions.Sum(p => p.Result) / Math.Max(1, closedPositions.Count) / PipSize;

			var livePositions = LivePositions.Where(p => p.HasBothSides).ToList();
			var avgLive = livePositions.Sum(p => p.OpenResult) / Math.Max(1, livePositions.Count) / PipSize;

			var statistics = new List<Statistics>()
			{
				new Statistics()
				{
					Group = "All",
					Total = LatencyArbPositions.Count,
					Account = "Feed",
					AvgPrice = _feedAvg,
					Ask = LastFeedTick?.Ask,
					Bid = LastFeedTick?.Bid
				},
				new Statistics()
				{
					Group = "Live",
					Total = LivePositions.Count,
					AvgPip = avgLive,
					Account = "Long",
					AvgPrice = _longAvg,
					Ask = LastLongTick?.Ask,
					Bid = LastLongTick?.Bid,
					OpenPip = (LastFeedTick?.Ask - LastLongTick?.Ask - (_feedAvg ?? 0) + (_longAvg ?? 0)) / PipSize,
					ClosePip = (LastLongTick?.Bid - LastFeedTick?.Bid + (_feedAvg ?? 0) - (_longAvg ?? 0)) / PipSize
				},
				new Statistics()
				{
					Group = "Closed",
					Total = closedPositions.Count,
					AvgPip = avgClosed,
					Account = "Short",
					AvgPrice = _shortAvg,
					Ask = LastShortTick?.Ask,
					Bid = LastShortTick?.Bid,
					OpenPip = (LastShortTick?.Bid - LastFeedTick?.Bid + (_feedAvg ?? 0) - (_shortAvg ?? 0)) / PipSize,
					ClosePip = (LastFeedTick?.Ask - LastShortTick?.Ask - (_feedAvg ?? 0) + (_shortAvg ?? 0)) / PipSize
				}
			};

			return statistics.Select(s => new
			{
				Group = s.Group,
				Total = s.Total.ToString("0"),
				AvgPip = s.AvgPip?.ToString("F2"),
				III = "",
				Account = s.Account,
				Ask = s.Ask?.ToString("F5"),
				Bid = s.Bid?.ToString("F5"),
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
			return lastTick.Ask - lastTick.Bid <= spread * PipSize;
		}
	}
}

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
	public partial class NewsArb
	{
		public class Statistics
		{
			public string Group { get; set; }
			public int Total { get; set; }
			public decimal? AvgPip { get; set; }
			public string Account { get; set; }
			public decimal? Ask { get; set; }
			public decimal? Bid { get; set; }
			public decimal? OpenPip { get; set; }
			public decimal? ClosePip { get; set; }
		}

		public class NewsArbPos
		{
			public long? ShortTicket { get; set; }
			public long? LongTicket { get; set; }
			public decimal? MinMax { get; set; }
		}

		public event EventHandler<NewTick> NewTick;
		public event EventHandler<ConnectionStates> ConnectionChanged;

		[NotMapped]
		[InvisibleColumn]
		public bool IsConnected
		{
			get => Get<bool>();
			set => Set(value);
		}

		[NotMapped] [InvisibleColumn] public Tick LastSnwTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastFirstTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastHedgeTick { get; set; }
		[NotMapped] [InvisibleColumn] public bool ShortSpreadCheck => SpreadCheck(LastFirstTick, FirstSpreadFilterInPip);
		[NotMapped] [InvisibleColumn] public bool LongSpreadCheck => SpreadCheck(LastHedgeTick, HedgeSpreadFilterInPip);

		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;
		[NotMapped] public List<NewsArbPosition> LivePositions => NewsArbPositions.Where(p => !p.Archived).ToList();

		[NotMapped] [InvisibleColumn] public Stopwatch Stopwatch { get; } = new Stopwatch();

		public NewsArb()
		{
			SetAction<Account>(nameof(SnwAccount),
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
			SetAction<Account>(nameof(FirstAccount),
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
			SetAction<Account>(nameof(HedgeAccount),
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
			if (sender == SnwAccount && newTick.Tick.Symbol != SnwSymbol) return;
			if (sender == FirstAccount && newTick.Tick.Symbol != FirstSymbol) return;
			if (sender == HedgeAccount && newTick.Tick.Symbol != HedgeSymbol) return;

			if (sender == SnwAccount) LastSnwTick = newTick.Tick;
			if (sender == FirstAccount) LastFirstTick = newTick.Tick;
			if (sender == HedgeAccount) LastHedgeTick = newTick.Tick;

			if (LastFirstTick?.HasValue != true) return;
			if (LastHedgeTick?.HasValue != true) return;

			NewTick?.Invoke(this, newTick);
		}

		private void Account_ConnectionChanged(object sender, ConnectionStates connectionStates)
		{
			IsConnected =
				SnwAccount?.Connector?.IsConnected == true &&
				FirstAccount?.Connector?.IsConnected == true &&
				HedgeAccount?.Connector?.IsConnected == true;
			ConnectionChanged?.Invoke(this, IsConnected ? ConnectionStates.Connected : ConnectionStates.Disconnected);
		}

		public IList CalculateStatistics()
		{
			var closedPositions = NewsArbPositions.Where(p => p.IsFull).ToList();
			var avgClosed = closedPositions.Sum(p => p.Result) / Math.Max(1, closedPositions.Count) / PipSize;

			var livePositions = LivePositions.Where(p => p.HasBothSides).ToList();
			var avgLive = livePositions.Sum(p => p.OpenResult) / Math.Max(1, livePositions.Count) / PipSize;

			var statistics = new List<Statistics>()
			{
				new Statistics()
				{
					Group = "All",
					Total = NewsArbPositions.Count,
					Account = "Snw",
					Ask = LastSnwTick?.Ask,
					Bid = LastSnwTick?.Bid
				},
				new Statistics()
				{
					Group = "Live",
					Total = LivePositions.Count,
					AvgPip = avgLive,
					Account = "First",
					Ask = LastHedgeTick?.Ask,
					Bid = LastHedgeTick?.Bid,
					OpenPip = (LastSnwTick?.Ask - LastHedgeTick?.Ask) / PipSize,
					ClosePip = (LastHedgeTick?.Bid - LastSnwTick?.Bid) / PipSize
				},
				new Statistics()
				{
					Group = "Closed",
					Total = closedPositions.Count,
					AvgPip = avgClosed,
					Account = "Hedge",
					Ask = LastFirstTick?.Ask,
					Bid = LastFirstTick?.Bid,
					OpenPip = (LastFirstTick?.Bid - LastSnwTick?.Bid) / PipSize,
					ClosePip = (LastSnwTick?.Ask - LastFirstTick?.Ask) / PipSize
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
				OpenDiff = s.OpenPip?.ToString("F2"),
				CloseDiff = s.ClosePip?.ToString("F2"),
			}).ToList();
		}

		private bool SpreadCheck(Tick lastTick, decimal spread)
		{
			if (lastTick?.HasValue != true) return false;
			return lastTick.Ask - lastTick.Bid <= spread * PipSize;
		}
	}
}

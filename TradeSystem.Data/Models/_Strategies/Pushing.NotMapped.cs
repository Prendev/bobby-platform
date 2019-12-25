using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
    public partial class Pushing
	{
		public event EventHandler<ConnectionStates> ConnectionChanged;

		[NotMapped] [InvisibleColumn] public Spoof Spoof { get; set; }
		[NotMapped] [InvisibleColumn] public IStratState StratState { get; set; }

		[NotMapped] [InvisibleColumn] public bool IsBuyBeta { get => Get(() => true); set => Set(value); }
		[NotMapped] [InvisibleColumn] public bool IsSellBeta { get => Get(() => true); set => Set(value); }
		[NotMapped] [InvisibleColumn] public bool IsCloseShortBuyFutures { get => Get(() => true); set => Set(value); }
		[NotMapped] [InvisibleColumn] public bool IsCloseLongSellFutures { get => Get(() => true); set => Set(value); }
		[NotMapped] [InvisibleColumn] public Sides BetaOpenSide { get; set; }
		[NotMapped] [InvisibleColumn] public Sides FirstCloseSide { get; set; }
		[NotMapped] [InvisibleColumn] public bool IsHedgeClose { get => Get(() => true); set => Set(value); }
		[NotMapped] [InvisibleColumn] public Position AlphaPosition { get; set; }
		[NotMapped] [InvisibleColumn] public Position BetaPosition { get; set; }
		[NotMapped] [InvisibleColumn] public Position ScalpPosition { get; set; }
		[NotMapped] [InvisibleColumn] public Position ReopenPosition { get; set; }
		[NotMapped] [InvisibleColumn] public bool InPanic { get; set; }
		[NotMapped] [InvisibleColumn] public bool IsConnected { get => Get<bool>(); set => Set(value); }

		[NotMapped] [InvisibleColumn] public Tick LastFeedTick { get => Get<Tick>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public Tick LastSlowTick { get => Get<Tick>(); set => Set(value); }

		[NotMapped] [InvisibleColumn] public decimal? NormFeedAsk => NormAsk(LastFeedTick, _feedAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormFeedBid => NormBid(LastFeedTick, _feedAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormSlowAsk => NormAsk(LastSlowTick, _slowAvg);
		[NotMapped] [InvisibleColumn] public decimal? NormSlowBid => NormBid(LastSlowTick, _slowAvg);

		public event EventHandler<NewTick> NewTick;

		private readonly List<Tick> _feedTicks = new List<Tick>();
		private readonly List<Tick> _slowTicks = new List<Tick>();
		private decimal? _feedAvg = null;
		private decimal? _slowAvg = null;

		public Pushing()
		{
			SetAction<Account>(nameof(FutureAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(AlphaMaster),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(BetaMaster),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(ScalpAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(HedgeAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(ReopenAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(FeedAccount),
				a => { if (a != null) a.NewTick -= Account_NewTick; },
				a => { if (a != null) a.NewTick += Account_NewTick; });
			SetAction<Account>(nameof(SlowAccount),
				a => { if (a != null) a.NewTick -= Account_NewTick; },
				a => { if (a != null) a.NewTick += Account_NewTick; });
		}

		private void Account_NewTick(object sender, NewTick newTick)
		{
			if (newTick?.Tick == null) return;
			if (sender == FeedAccount && newTick.Tick.Symbol != FeedSymbol) return;
			if (sender == SlowAccount && newTick.Tick.Symbol != SlowSymbol) return;

			if (sender == FeedAccount) LastFeedTick = newTick.Tick;
			if (sender == SlowAccount) LastSlowTick = newTick.Tick;

			if (sender == FeedAccount) _feedAvg = Averaging(_feedTicks, _feedAvg, LastFeedTick);
			if (sender == SlowAccount) _slowAvg = Averaging(_slowTicks, _slowAvg, LastSlowTick);

			if (LastFeedTick?.HasValue != true) return;
			if (LastSlowTick?.HasValue != true) return;
			if (AveragingPeriodInSeconds > 0 && _feedAvg.HasValue != true) return;
			if (AveragingPeriodInSeconds > 0 && _slowAvg.HasValue != true) return;

			NewTick?.Invoke(this, newTick);
		}

		private void Account_ConnectionChanged(object sender, ConnectionStates connectionStates)
		{
			IsConnected =
				FutureAccount?.Connector?.IsConnected == true &&
				AlphaMaster?.Connector?.IsConnected == true &&
				BetaMaster?.Connector?.IsConnected == true &&
				ScalpAccount?.Connector?.IsConnected != false &&
				HedgeAccount?.Connector?.IsConnected != false &&
				FeedAccount?.Connector?.IsConnected != false &&
				SpoofAccount?.Connector?.IsConnected != false &&
				ReopenAccount?.Connector?.IsConnected != false;
			ConnectionChanged?.Invoke(this, IsConnected ? ConnectionStates.Connected : ConnectionStates.Disconnected);
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
	}
}

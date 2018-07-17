using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
	public class StratDealingArb : BaseDescriptionEntity
	{
		public enum StratDealingArbOrderTypes
		{
			Market,
			Aggressive
		}

		public event EventHandler OnTick;

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[NotMapped] public decimal? AlphaAsk { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] public decimal? BetaBid { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] public decimal? BetaAsk { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] public decimal? AlphaBid { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] [InvisibleColumn]  public Tick AlphaTick { get; set; }
		[NotMapped] [InvisibleColumn]  public Tick BetaTick { get; set; }

		[DisplayName("MaxPos")]
		public int MaxNumberOfPositions { get; set; }

		[DisplayName("SignalDiff")]
		public decimal SignalDiffInPip { get; set; }
		[DisplayName("SignalStep")]
		public decimal SignalStepInPip { get; set; }
		[DisplayName("Target")]
		public decimal TargetInPip { get; set; }

		[DisplayName("MinOpenTime")]
		public int MinOpenTimeInMinutes { get; set; }
		[DisplayName("ReOpenInterval")]
		public int ReOpenIntervalInMinutes { get; set; }

		[InvisibleColumn] public TimeSpan? EarliestOpenTime { get; set; }
		[InvisibleColumn] public TimeSpan? LatestOpenTime { get; set; }
		[InvisibleColumn] public TimeSpan? LatestCloseTime { get; set; }

		public StratDealingArbOrderTypes OrderType { get; set; }
		[DisplayName("Slippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("TimeWindow")]
		public int TimeWindowInMs { get; set; }

		public int AlphaAccountId { get; set; }
		private Account _alphaAccount;
		public Account AlphaAccount
		{
			get => _alphaAccount;
			set
			{
				if (_alphaAccount != null)
					_alphaAccount.OnTick -= Account_OnTick;
				if (value != null)
					value.OnTick += Account_OnTick;
				_alphaAccount = value;
			}
		}
		[Required] public string AlphaSymbol { get; set; }
		public decimal AlphaSize { get; set; }

		public int BetaAccountId { get; set; }
		private Account _betaAccount;
		public Account BetaAccount
		{
			get => _betaAccount;
			set
			{
				if (_betaAccount != null)
					_betaAccount.OnTick -= Account_OnTick;
				if (value != null)
					value.OnTick += Account_OnTick;
				_betaAccount = value;
			}
		}
		[Required] public string BetaSymbol { get; set; }
		public decimal BetaSize { get; set; }

		public decimal? ShiftInPip { get => Get<decimal?>(); set => Set(value); }
		public TimeSpan ShiftCalcInterval { get; set; }

		public decimal PipSize { get; set; }
		[InvisibleColumn] public int MagicNumber { get; set; }

		[InvisibleColumn] public int MaxRetryCount { get; set; }
		[InvisibleColumn] public int RetryPeriodInMs { get; set; }

		[InvisibleColumn] public DateTime? LastOpenTime { get => Get<DateTime?>(); set => Set(value); }

		public List<StratDealingArbPosition> Positions { get => Get(() => new List<StratDealingArbPosition>()); set => Set(value, false); }

		[NotMapped] [InvisibleColumn] public decimal ShiftDiffSumInPip { get; set; }
		[NotMapped] [InvisibleColumn] public Stopwatch ShiftCalcStopwatch { get; set; }
		[NotMapped] [InvisibleColumn] public int ShiftTickCount { get; set; }

		[NotMapped] [InvisibleColumn] public bool HasTiming => EarliestOpenTime.HasValue && LatestOpenTime.HasValue && LatestCloseTime.HasValue;
		[NotMapped] [InvisibleColumn] public List<StratDealingArbPosition> OpenPositions => Positions.Where(p => !p.IsClosed).ToList();
		[NotMapped] [InvisibleColumn] public Sides AlphaSide => GetSide(Positions?.FirstOrDefault()?.AlphaSide);
		[NotMapped] [InvisibleColumn] public Sides BetaSide => GetSide(Positions?.FirstOrDefault()?.BetaSide);
		[NotMapped] [InvisibleColumn] public decimal? LastAlphaOpenPrice => Positions?.LastOrDefault()?.AlphaOpenPrice;
		[NotMapped] [InvisibleColumn] public decimal? LastBetaOpenPrice => Positions?.LastOrDefault()?.BetaOpenPrice;
		[NotMapped] [InvisibleColumn] public int PositionCount => OpenPositions?.Count ?? 0;
		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;

		[NotMapped] [InvisibleColumn] public bool DoOpenSide1 { get; set; }
		[NotMapped] [InvisibleColumn] public bool DoOpenSide2 { get; set; }
		[NotMapped] [InvisibleColumn] public bool DoClose { get; set; }

		[NotMapped]
		[InvisibleColumn]
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}
		private volatile bool _isBusy;


		private Sides GetSide(StratDealingArbPosition.Sides? side)
		{
			switch (side)
			{
				case StratDealingArbPosition.Sides.Buy:
					return Sides.Buy;
				case StratDealingArbPosition.Sides.Sell:
					return Sides.Sell;
				default: return Sides.None;
			}
		}

		private void Account_OnTick(object sender, TickEventArgs tickEventArgs)
		{
			if (tickEventArgs?.Tick?.HasValue != true) return;
			if (sender == AlphaAccount && tickEventArgs.Tick.Symbol != AlphaSymbol) return;
			if (sender == BetaAccount && tickEventArgs.Tick.Symbol != BetaSymbol) return;

			if (sender == AlphaAccount)
			{
				AlphaAsk = tickEventArgs.Tick.Ask;
				AlphaBid = tickEventArgs.Tick.Bid;
				AlphaTick = tickEventArgs.Tick;
			}
			else if (sender == BetaAccount)
			{
				BetaAsk = tickEventArgs.Tick.Ask;
				BetaBid = tickEventArgs.Tick.Bid;
				BetaTick = tickEventArgs.Tick;
			}

			if (AlphaTick?.HasValue != true || BetaTick?.HasValue != true) return;
			if (DateTime.UtcNow - AlphaTick.Time > new TimeSpan(0, 1, 0)) return;
			if (DateTime.UtcNow - BetaTick.Time > new TimeSpan(0, 1, 0)) return;

			OnTick?.Invoke(this, null);
		}
	}
}

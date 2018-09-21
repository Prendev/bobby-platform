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

		public event EventHandler NewTick;

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public bool Run { get => Get<bool>(); set => Set(value); }

		[NotMapped] public decimal? AlphaAsk { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] public decimal? BetaBid { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] public decimal? BetaAsk { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] public decimal? AlphaBid { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public Tick AlphaTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick BetaTick { get; set; }

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
		[DisplayName("MaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("RetryPeriod")]
		public int RetryPeriodInMs { get; set; }
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
					_alphaAccount.NewTick -= Account_NewTick;
				if (value != null)
					value.NewTick += Account_NewTick;
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
					_betaAccount.NewTick -= Account_NewTick;
				if (value != null)
					value.NewTick += Account_NewTick;
				_betaAccount = value;
			}
		}
		[Required] public string BetaSymbol { get; set; }
		public decimal BetaSize { get; set; }

		public decimal? ShiftInPip { get => Get<decimal?>(); set => Set(value); }
		public TimeSpan ShiftCalcInterval { get; set; }

		public decimal PipSize { get; set; }
		[InvisibleColumn] public int MagicNumber { get; set; }

		[InvisibleColumn] public DateTime? LastOpenTime { get => Get<DateTime?>(); set => Set(value); }

		public List<StratDealingArbPosition> Positions { get => Get(() => new List<StratDealingArbPosition>()); set => Set(value, false); }

		[NotMapped] [InvisibleColumn] public decimal ShiftDiffSumInPip { get; set; }
		[NotMapped] [InvisibleColumn] public Stopwatch ShiftCalcStopwatch { get; set; }
		[NotMapped] [InvisibleColumn] public int ShiftTickCount { get; set; }

		[NotMapped] [InvisibleColumn] public bool HasTiming => EarliestOpenTime.HasValue && LatestOpenTime.HasValue && LatestCloseTime.HasValue;
		[NotMapped] [InvisibleColumn] public List<StratDealingArbPosition> OpenPositions => Positions.Where(p => !p.IsClosed).ToList();
		[NotMapped] [InvisibleColumn] public Sides AlphaSide => GetSide(OpenPositions?.FirstOrDefault()?.AlphaSide);
		[NotMapped] [InvisibleColumn] public Sides BetaSide => GetSide(OpenPositions?.FirstOrDefault()?.BetaSide);
		[NotMapped] [InvisibleColumn] public decimal? LastAlphaOpenPrice => OpenPositions?.LastOrDefault()?.AlphaOpenPrice;
		[NotMapped] [InvisibleColumn] public decimal? LastBetaOpenPrice => OpenPositions?.LastOrDefault()?.BetaOpenPrice;
		[NotMapped] [InvisibleColumn] public int PositionCount => OpenPositions?.Count ?? 0;
		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;

		[NotMapped]
		[InvisibleColumn]
		public bool DoOpenSide1
		{
			get => _doOpenSide1;
			set => _doOpenSide1 = value;
		}

		[NotMapped]
		[InvisibleColumn]
		public bool DoOpenSide2
		{
			get => _doOpenSide2;
			set => _doOpenSide2 = value;
		}

		[NotMapped]
		[InvisibleColumn]
		public bool DoClose
		{
			get => _doClose;
			set => _doClose = value;
		}

		[NotMapped]
		[InvisibleColumn]
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}
		private volatile bool _isBusy;
		private volatile bool _doOpenSide1;
		private volatile bool _doOpenSide2;
		private volatile bool _doClose;


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

		private void Account_NewTick(object sender, NewTickEventArgs newTickEventArgs)
		{
			if (!Run) return;
			if (newTickEventArgs?.Tick?.HasValue != true) return;
			if (sender == AlphaAccount && newTickEventArgs.Tick.Symbol != AlphaSymbol) return;
			if (sender == BetaAccount && newTickEventArgs.Tick.Symbol != BetaSymbol) return;

			if (sender == AlphaAccount)
			{
				AlphaAsk = newTickEventArgs.Tick.Ask;
				AlphaBid = newTickEventArgs.Tick.Bid;
				AlphaTick = newTickEventArgs.Tick;
			}
			else if (sender == BetaAccount)
			{
				BetaAsk = newTickEventArgs.Tick.Ask;
				BetaBid = newTickEventArgs.Tick.Bid;
				BetaTick = newTickEventArgs.Tick;
			}

			if (AlphaTick?.HasValue != true || BetaTick?.HasValue != true) return;
			if (DateTime.UtcNow - AlphaTick.Time > new TimeSpan(0, 1, 0)) return;
			if (DateTime.UtcNow - BetaTick.Time > new TimeSpan(0, 1, 0)) return;

			SetNetProfits();
			NewTick?.Invoke(this, null);
		}

		public void SetNetProfits()
		{
			if (Positions == null || Positions.Count == 0) return;
			foreach (var pos in Positions)
				NetProfitInPip(pos);
		}

		private void NetProfitInPip(StratDealingArbPosition pos)
		{
			var alpha = 0m;
			var beta = 0m;

			if (pos.AlphaSide == StratDealingArbPosition.Sides.Sell)
			{
				alpha += pos.AlphaOpenPrice;
				alpha -= pos.AlphaClosePrice ?? AlphaTick?.Ask ?? 0;
			}
			else if (pos.AlphaSide == StratDealingArbPosition.Sides.Buy)
			{
				alpha -= pos.AlphaOpenPrice;
				alpha += pos.AlphaClosePrice ?? AlphaTick?.Bid ?? 0;
			}

			if (pos.BetaSide == StratDealingArbPosition.Sides.Sell)
			{
				beta += pos.BetaOpenPrice;
				beta -= pos.BetaClosePrice ?? BetaTick?.Ask ?? 0;
			}
			else if (pos.BetaSide == StratDealingArbPosition.Sides.Buy)
			{
				beta -= pos.BetaOpenPrice;
				beta += pos.BetaClosePrice ?? BetaTick?.Bid ?? 0;
			}

			alpha *= pos.AlphaSize;
			beta *= pos.BetaSize;

			if (pos.AlphaSize + pos.BetaSize == 0) pos.NetProfitInPip = 0;
			else pos.NetProfitInPip = (alpha + beta) / (pos.AlphaSize + pos.BetaSize) / PipSize;
		}
	}
}

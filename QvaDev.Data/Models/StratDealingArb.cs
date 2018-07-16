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

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[NotMapped] public decimal? AlphaAsk { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] public decimal? BetaBid { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] public decimal? BetaAsk { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] public decimal? AlphaBid { get => Get<decimal?>(); set => Set(value); }

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

		[InvisibleColumn] public StratDealingArbOrderTypes OrderType { get; set; }
		[InvisibleColumn] public decimal Deviation { get; set; }
		[InvisibleColumn] public int Ttl { get; set; }

		public int AlphaAccountId { get; set; }
		public Account AlphaAccount { get; set; }
		[Required] public string AlphaSymbol { get; set; }
		public decimal AlphaSize { get; set; }

		public int BetaAccountId { get; set; }
		public Account BetaAccount { get; set; }
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

		[NotMapped] [InvisibleColumn] public bool DoOpenSide1 { get; set; }
		[NotMapped] [InvisibleColumn] public bool DoOpenSide2 { get; set; }
		[NotMapped] [InvisibleColumn] public bool DoClose { get; set; }

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
	}
}

using System;
using System.Collections.Generic;
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

		public int ProfileId { get; set; }
		public Profile Profile { get; set; }

		public int MaxNumberOfPositions { get; set; }

		public decimal SignalDiffInPip { get; set; }
		public decimal SignalStepInPip { get; set; }
		public decimal TargetInPip { get; set; }

		public int MinOpenTimeInMinutes { get; set; }
		public int ReOpenIntervalInMinutes { get; set; }

		public TimeSpan EarliestOpenTime { get; set; }
		public TimeSpan LatestOpenTime { get; set; }
		public TimeSpan LatestCloseTime { get; set; }

		public StratDealingArbOrderTypes OrderType { get; set; }
		public decimal Deviation { get; set; }
		public int Ttl { get; set; }

		public int AlphaAccountId { get; set; }
		public Account AlphaAccount { get; set; }
		[Required] public string AlphaSymbol { get; set; }
		public decimal AlphaSize { get; set; }

		public int BetaAccountId { get; set; }
		public Account BetaAccount { get; set; }
		[Required] public string BetaSymbol { get; set; }
		public decimal BetaSize { get; set; }

		public decimal? ShiftInPip { get; set; }
		public TimeSpan ShiftCalcInterval { get; set; }

		public decimal PipSize { get; set; }
		public int MagicNumber { get; set; }

		public int MaxRetryCount { get; set; }
		public int RetryPeriodInMilliseconds { get; set; }

		[NotMapped] [InvisibleColumn] public decimal ShiftDiffSumInPip { get; set; }
		[NotMapped] [InvisibleColumn] public Stopwatch ShiftCalcStopwatch { get; set; }
		[NotMapped] [InvisibleColumn] public int ShiftTickCount { get; set; }

		public List<StratDealingArbPosition> Positions { get => Get(() => new List<StratDealingArbPosition>()); set => Set(value, false); }

		[NotMapped] [InvisibleColumn] public List<StratDealingArbPosition> OpenPositions => Positions.Where(p => !p.IsClosed).ToList();
		[NotMapped] [InvisibleColumn] public Sides AlphaSide => GetSide(Positions?.FirstOrDefault()?.AlphaSide);
		[NotMapped] [InvisibleColumn] public Sides BetaSide => GetSide(Positions?.FirstOrDefault()?.BetaSide);
		[NotMapped] [InvisibleColumn] public decimal? LastAlphaOpenPrice => Positions?.LastOrDefault()?.AlphaOpenPrice;
		[NotMapped] [InvisibleColumn] public decimal? LastBetaOpenPrice => Positions?.LastOrDefault()?.BetaOpenPrice;
		[NotMapped] [InvisibleColumn] public DateTime? LastOpenTime => Positions?.LastOrDefault()?.OpenTime;
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

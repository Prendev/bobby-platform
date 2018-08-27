using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
	public class StratHubArb : BaseDescriptionEntity
	{
		public enum StratHubArbOrderTypes
		{
			Market,
			Aggressive
		}

		[InvisibleColumn] public int AggregatorId { get; set; }
		[InvisibleColumn] public Aggregator Aggregator { get; set; }

		public bool Run { get => Get<bool>(); set => Set(value); }

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

		public StratHubArbOrderTypes OrderType { get; set; }
		[DisplayName("MaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("RetryPeriod")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("Slippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("TimeWindow")]
		public int TimeWindowInMs { get; set; }

		public decimal Size { get; set; }
		public decimal PipSize { get; set; }

		[NotMapped] [InvisibleColumn] public bool HasTiming => EarliestOpenTime.HasValue && LatestOpenTime.HasValue && LatestCloseTime.HasValue;
		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;

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

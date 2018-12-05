using System;
using System.Collections.Generic;
using System.ComponentModel;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public partial class StratHubArb : BaseDescriptionEntity
	{
		public enum StratHubArbOrderTypes
		{
			Market,
			Aggressive,
			DelayedAggressive
		}

		public enum StratHubArbOpeningLogics
		{
			Parallel,
			SlowFirst
		}

		[DisplayPriority(-1)] public bool Run { get; set; }

		[InvisibleColumn] public int AggregatorId { get; set; }
		public Aggregator Aggregator { get => Get<Aggregator>(); set => Set(value); }

		public decimal Size { get; set; }
		[DisplayName("MaxSize")]
		public decimal MaxSizePerAccount { get; set; }

		[DisplayName("SignalDiff")]
		public decimal SignalDiffInPip { get; set; }
		[DisplayName("HighRiskDiff")]
		public decimal? HighRiskSignalDiffInPip { get; set; }
		[DisplayName("RestPeriod")] // Resting period per account
		public int RestingPeriodInMinutes { get; set; }

		[DisplayName("EarliestOpen")]
		public TimeSpan? EarliestOpenTime { get; set; }
		[DisplayName("LatestOpen")]
		public TimeSpan? LatestOpenTime { get; set; }
		[DisplayName("LatestClose")]
		public TimeSpan? LatestCloseTime { get; set; }

		public StratHubArbOpeningLogics OpeningLogic { get; set; }
		public StratHubArbOrderTypes OrderType { get; set; }
		public StratHubArbOrderTypes SlowOrderType { get; set; }
		public StratHubArbOrderTypes FastOrderType { get; set; }
		[DisplayName("MaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("RetryPeriod")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("Slippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("Correction")]
		public decimal CorrectionInPip { get; set; }
		[DisplayName("TimeWindow")]
		public int TimeWindowInMs { get; set; }
		[DisplayName("DelayTimeWindow")]
		public int DelayTimeWindowInMs { get; set; }

		public decimal PipSize { get; set; }

		public List<StratHubArbPosition> StratHubArbPositions { get; } = new List<StratHubArbPosition>();
	}
}

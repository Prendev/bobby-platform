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
			Aggressive
		}

		[DisplayPriority(-1)] public bool Run { get; set; }

		[InvisibleColumn] public int AggregatorId { get; set; }
		public Aggregator Aggregator { get => Get<Aggregator>(); set => Set(value); }

		public decimal Size { get; set; }
		[DisplayName("MaxSize")]
		public decimal MaxSizePerAccount { get; set; }
		public decimal PipSize { get; set; }

		[DisplayName("SignalDiff")]
		public decimal SignalDiffInPip { get; set; }
		[DisplayName("HighRiskDiff")]
		public decimal? HighRiskSignalDiffInPip { get; set; }
		[DisplayName("RestingPeriod")] // Resting period per account
		public int RestingPeriodInMinutes { get; set; }

		public TimeSpan? EarliestOpenTime { get; set; }
		public TimeSpan? LatestOpenTime { get; set; }
		public TimeSpan? LatestCloseTime { get; set; }

		public StratHubArbOrderTypes OrderType { get; set; }
		[DisplayName("MaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("RetryPeriod")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("Slippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("TimeWindow")]
		public int TimeWindowInMs { get; set; }

		public bool EmailAlert { get; set; }

		[InvisibleColumn] public List<StratHubArbPosition> StratHubArbPositions { get; } = new List<StratHubArbPosition>();
	}
}

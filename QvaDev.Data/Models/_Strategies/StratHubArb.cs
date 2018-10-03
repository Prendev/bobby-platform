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

		[InvisibleColumn] public int AggregatorId { get; set; }
		[InvisibleColumn] public Aggregator Aggregator { get => Get<Aggregator>(); set => Set(value); }

		public bool Run { get => Get<bool>(); set => Set(value); }

		public decimal Size { get; set; }
		[DisplayName("MaxSize")]
		public decimal MaxSizePerAccount { get; set; }
		public decimal PipSize { get; set; }

		[DisplayName("SignalDiff")]
		public decimal SignalDiffInPip { get; set; }
		[DisplayName("MinOpenTime")]
		public int MinOpenTimeInMinutes { get; set; }

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

		[InvisibleColumn] public List<StratHubArbPosition> StratHubArbPositions { get => Get(() => new List<StratHubArbPosition>()); set => Set(value); }
		[InvisibleColumn] public DateTime? LastOpenTime { get => Get<DateTime?>(); set => Set(value); }
	}
}

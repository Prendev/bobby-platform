using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;
using QvaDev.Communication.FixApi;

namespace QvaDev.Data.Models
{
	public class StratHubArb : BaseDescriptionEntity
	{
		public enum StratHubArbOrderTypes
		{
			Market,
			Aggressive
		}

		public event EventHandler<GroupQuoteEventArgs> GroupQuote;

		[InvisibleColumn] public int AggregatorId { get; set; }
		private Aggregator _aggregator;
		[InvisibleColumn]
		public Aggregator Aggregator
		{
			get => _aggregator;
			set
			{
				if (_aggregator != null)
					_aggregator.GroupQuote -= Aggregator_GroupQuote;

				if (value != null)
					value.GroupQuote += Aggregator_GroupQuote;

				_aggregator = value;
			}
		}

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

		private void Aggregator_GroupQuote(object sender, GroupQuoteEventArgs e)
		{
			GroupQuote?.Invoke(this, e);
		}
	}
}

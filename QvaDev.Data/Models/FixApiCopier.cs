using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public class FixApiCopier : BaseEntity
	{
		public enum FixApiOrderTypes
		{
			Market,
			Aggressive,
			GtcLimit
		}

		public enum BasePriceTypes
		{
			Slave,
			Master
		}

		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }
		public decimal CopyRatio { get; set; }
		public FixApiOrderTypes OrderType { get; set; }
		public BasePriceTypes BasePriceType { get; set; }
		[DisplayName("FallbackToMarket")]
		public bool FallbackToMarketOrderType { get; set; }
		[DisplayName("FallbackTimeWindow")]
		public int FallbackTimeWindowInMs { get; set; }
		public int DelayInMilliseconds { get; set; }

		[DisplayName("MarketMaxRetry")]
		public int MarketMaxRetryCount { get; set; }
		[DisplayName("MarketRetryPeriod")]
		public int MarketRetryPeriodInMs { get; set; }
		[DisplayName("MarketTimeWindow")]
		public int MarketTimeWindowInMs { get; set; }

		[DisplayName("LimitMaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("LimitRetryPeriod")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("LimitSlippage")]
		public decimal SlippageInPip { get; set; }

		[DisplayName("LimitTimeWindow")]
		public int TimeWindowInMs { get; set; }
		[DisplayName("LimitDiff")]
		public decimal LimitDiffInPip { get; set; }
		public decimal PipSize { get; set; }

		public List<FixApiCopierPosition> FixApiCopierPositions { get; } = new List<FixApiCopierPosition>();

		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;
		[NotMapped] [InvisibleColumn] public decimal LimitDiff => LimitDiffInPip * PipSize;
		[NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
	}
}

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
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
		[DisplayName("Delay")]
		public int DelayInMilliseconds { get; set; }

		[DisplayName("FallbackMarketOpen")]
		public bool FallbackToMarketOrderType { get; set; }
		[DisplayName("F Retry")]
		public int FallbackMaxRetryCount { get; set; }
		[DisplayName("F Period")]
		public int FallbackRetryPeriodInMs { get; set; }
		[DisplayName("F Window")]
		public int FallbackTimeWindowInMs { get; set; }

		[DisplayName("MarketRetry")]
		public int MarketMaxRetryCount { get; set; }
		[DisplayName("M Period")]
		public int MarketRetryPeriodInMs { get; set; }
		[DisplayName("M Window")]
		public int MarketTimeWindowInMs { get; set; }

		[DisplayName("LimitRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("L Period")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("L Window")]
		public int TimeWindowInMs { get; set; }
		[DisplayName("L Slippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("L Diff")]
		public decimal LimitDiffInPip { get; set; }

		public decimal PipSize { get; set; }

		public List<FixApiCopierPosition> FixApiCopierPositions { get; } = new List<FixApiCopierPosition>();

		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;
		[NotMapped] [InvisibleColumn] public decimal LimitDiff => LimitDiffInPip * PipSize;
		[NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
	}
}

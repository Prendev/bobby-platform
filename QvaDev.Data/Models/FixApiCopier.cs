using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
	public class FixApiCopier : BaseEntity
	{
		public enum FixApiOrderTypes
		{
			Market,
			AggressiveOnMasterPrice,
			AggressiveOnSlavePrice
		}

		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }
		public decimal CopyRatio { get; set; }
		public FixApiOrderTypes OrderType { get; set; }
		public int DelayInMilliseconds { get; set; }

		[DisplayName("MarketMaxRetry")]
		public int MarketMaxRetryCount { get; set; }
		[DisplayName("MarketRetryPeriod")]
		public int MarketRetryPeriodInMs { get; set; }
		[DisplayName("MarketTimeWindow")]
		public int MarketTimeWindowInMs { get; set; }

		[DisplayName("AggMaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("AggRetryPeriod")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("AggSlippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("AggTimeWindow")]
		public int TimeWindowInMs { get; set; }
		public decimal PipSize { get; set; }

		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;
		[NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public Dictionary<long, OrderResponse> OrderResponses = new Dictionary<long, OrderResponse>();
	}
}

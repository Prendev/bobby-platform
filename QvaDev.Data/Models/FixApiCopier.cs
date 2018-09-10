using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
	public class FixApiCopier : BaseEntity, IFilterableEntity
	{
		public enum FixApiOrderTypes
		{
			Market,
			Aggressive
		}

		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		public bool Run { get; set; }
		public decimal CopyRatio { get; set; }
		public FixApiOrderTypes OrderType { get; set; }
		public int DelayInMilliseconds { get; set; }

		[DisplayName("MaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("RetryPeriod")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("Slippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("TimeWindow")]
		public int TimeWindowInMs { get; set; }
		public decimal PipSize { get; set; }

		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;
		[NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public Dictionary<long, OrderResponse> OrderResponses = new Dictionary<long, OrderResponse>();
	}
}

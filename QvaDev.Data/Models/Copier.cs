using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class Copier : BaseEntity
	{
		public enum CopierOrderTypes
		{
			Market,
			MarketRange
		}

		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }
        public decimal CopyRatio { get; set; }
		public CopierOrderTypes OrderType { get; set; }
		public int SlippageInPips { get; set; }
        public int MaxRetryCount { get; set; }
        public int RetryPeriodInMs { get; set; }
        public int DelayInMilliseconds { get; set; }
    }
}

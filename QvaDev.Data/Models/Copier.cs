using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class Copier : BaseEntity, IFilterableEntity
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

	    [NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
    }
}

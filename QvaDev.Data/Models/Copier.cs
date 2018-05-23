using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class Copier : BaseEntity, IFilterableEntity
	{
		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		public bool Run { get; set; }
        public decimal CopyRatio { get; set; }
        public bool UseMarketRangeOrder { get; set; }
        public int SlippageInPips { get; set; }
        public int MaxRetryCount { get; set; }
        public int RetryPeriodInMilliseconds { get; set; }
        public int DelayInMilliseconds { get; set; }

	    [NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
    }
}

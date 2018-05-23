using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class FixApiCopier : BaseEntity, IFilterableEntity
	{
		public enum OrderTypes
		{
			Market,
			Agressive
		}

		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		public bool Run { get; set; }
		public decimal CopyRatio { get; set; }
        public OrderTypes OrderType { get; set; }
        public double Slippage { get; set; }
		public int BurstPeriodInMilliseconds { get; set; }
		public int DelayInMilliseconds { get; set; }
        public int MaxRetryCount { get; set; }
        public int RetryPeriodInMilliseconds { get; set; }

        [NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
    }
}

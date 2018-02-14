using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class Copier : BaseEntity, IFilterableEntity
    {
        [Required]
        public int SlaveId { get; set; }
        [Required]
        public Slave Slave { get; set; }

        [Required]
        public decimal CopyRatio { get; set; }

        [Required]
        public bool UseMarketRangeOrder { get; set; }

        [Required]
        public int SlippageInPips { get; set; }

        [Required]
        public int MaxRetryCount { get; set; }

        [Required]
        public int RetryPeriodInMilliseconds { get; set; }

        [Required]
        public int DelayInMilliseconds { get; set; }

        [NotMapped]
        [InvisibleColumn]
        public bool IsFiltered { get => Get<bool>(); set => Set(value); }
    }
}

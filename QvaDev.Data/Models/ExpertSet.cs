using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class ExpertSet : BaseEntity, IFilterableEntity
    {
        [Required]
        public int TradingAccountId { get; set; }
        [Required]
        public TradingAccount TradingAccount { get; set; }

        [Required]
        public string Symbol1 { get; set; }
        [Required]
        public string Symbol2 { get; set; }

        [NotMapped]
        [InvisibleColumn]
        public bool IsFiltered { get => Get<bool>(); set => Set(value); }
    }
}

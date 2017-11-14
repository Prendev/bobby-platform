using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class TradingAccount : BaseEntity
    {
        [Required]
        public bool ShouldTrade { get; set; }

        public int ProfileId { get; set; }
        public Profile Profile { get; set; }

        public int ExpertId { get; set; }
        public Expert Expert { get; set; }

        public int MetaTraderAccountId { get; set; }
        public MetaTraderAccount MetaTraderAccount { get; set; }

        public List<ExpertSet> ExpertSets { get => Get(() => new List<ExpertSet>()); set => Set(value, false); }
    }
}

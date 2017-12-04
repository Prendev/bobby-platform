using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QvaDev.Data.Models
{
    public class TradingAccount : BaseDescriptionEntity
    {
        [Required]
        public bool ShouldTrade { get; set; }

        public int ProfileId { get; set; }
        public Profile Profile { get; set; }

        public int ExpertId { get; set; }
        public Expert Expert { get; set; }

        public int MetaTraderAccountId { get; set; }
        public MetaTraderAccount MetaTraderAccount { get; set; }

        public double TradeSetFloatingSwitch { get; set; }
        [NotMapped] public bool CloseAll { get; set; }
        [NotMapped] public bool BisectingClose { get; set; }
        [NotMapped] public bool SyncStates { get; set; }

        public List<QuadroSet> ExpertSets { get => Get(() => new List<QuadroSet>()); set => Set(value, false); }
    }
}

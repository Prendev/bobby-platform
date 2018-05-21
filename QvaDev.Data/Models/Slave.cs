using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class Slave : BaseEntity
    {
        [Required]
        public int MasterId { get; set; }
        [Required]
        public Master Master { get; set; }

        public int? CTraderAccountId { get; set; }
        public CTraderAccount CTraderAccount { get; set; }

        public int? MetaTraderAccountId { get; set; }
        public MetaTraderAccount MetaTraderAccount { get; set; }

	    public int? FixTraderAccountId { get; set; }
	    public FixTraderAccount FixTraderAccount { get; set; }

		public string SymbolSuffix { get; set; }

        public List<SymbolMapping> SymbolMappings { get => Get(() => new List<SymbolMapping>()); set => Set(value, false); }
        public List<Copier> Copiers { get => Get(() => new List<Copier>()); set => Set(value, false); }
    }
}

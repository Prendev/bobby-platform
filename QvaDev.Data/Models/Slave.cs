using System.Collections.Generic;

namespace QvaDev.Data.Models
{
    public class Slave : BaseEntity
	{
		public bool Run { get; set; }

        public int MasterId { get; set; }
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
	    public List<FixApiCopier> FixApiCopiers { get => Get(() => new List<FixApiCopier>()); set => Set(value, false); }
	}
}

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

        [Required]
        public int CTraderAccountId { get; set; }
        [Required]
        public CTraderAccount CTraderAccount { get; set; }

        public string SymbolSuffix { get; set; }

        public List<SymbolMapping> SymbolMappings { get => Get(() => new List<SymbolMapping>()); set => Set(value, false); }
        public List<Copier> Copiers { get => Get(() => new List<Copier>()); set => Set(value, false); }
    }
}

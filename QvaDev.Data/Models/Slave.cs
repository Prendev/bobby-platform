using System.Collections.Generic;

namespace QvaDev.Data.Models
{
    public class Slave : BaseEntity
	{
		public bool Run { get; set; }

        public int MasterId { get; set; }
        public Master Master { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

		public string SymbolSuffix { get; set; }

        public List<SymbolMapping> SymbolMappings { get => Get(() => new List<SymbolMapping>()); set => Set(value); }
        public List<Copier> Copiers { get => Get(() => new List<Copier>()); set => Set(value); }
	    public List<FixApiCopier> FixApiCopiers { get => Get(() => new List<FixApiCopier>()); set => Set(value); }

		public override string ToString()
		{
			return $"{(Id == 0 ? "UNSAVED - " : "")}{Master} - {Account}";
		}
	}
}

using System.Collections.Generic;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class Slave : BaseEntity
	{
		[DisplayPriority(-1)] public bool Run { get; set; }

        public int MasterId { get; set; }
        public Master Master { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

		public string SymbolSuffix { get; set; }

        public List<SymbolMapping> SymbolMappings { get; } = new List<SymbolMapping>();
		public List<Copier> Copiers { get; } = new List<Copier>();
		public List<FixApiCopier> FixApiCopiers { get; } = new List<FixApiCopier>();

		public override string ToString()
		{
			return $"{(Id == 0 ? "UNSAVED - " : "")}{Master} - {Account}";
		}
	}
}

using QvaDev.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class Ticker : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public int FixTraderAccountId { get; set; }
		public FixTraderAccount FixTraderAccount { get; set; }

		[Required]
		public string FtSymbol { get; set; }

		public int MetaTraderAccountId { get; set; }
        public MetaTraderAccount MetaTraderAccount { get; set; }

		[Required]
		public string MtSymbol { get; set; }
	}
}

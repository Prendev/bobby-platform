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
		public string Symbol { get; set; }

		public int? PairMetaTraderAccountId { get; set; }
        public MetaTraderAccount PairMetaTraderAccount { get; set; }

		public int? PairFixTraderAccountId { get; set; }
		public FixTraderAccount PairFixTraderAccount { get; set; }

		[Required]
		public string PairSymbol { get; set; }
	}
}

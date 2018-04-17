using QvaDev.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class Ticker : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public int? MainMetaTraderAccountId { get; set; }
		public MetaTraderAccount MainMetaTraderAccount { get; set; }

		public int? MainFixTraderAccountId { get; set; }
		public FixTraderAccount MainFixTraderAccount { get; set; }

		public string MainSymbol { get; set; }

		public int? PairMetaTraderAccountId { get; set; }
        public MetaTraderAccount PairMetaTraderAccount { get; set; }

		public int? PairFixTraderAccountId { get; set; }
		public FixTraderAccount PairFixTraderAccount { get; set; }

		public string PairSymbol { get; set; }
	}
}

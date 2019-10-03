using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class MetaTraderInstrumentConfig : BaseEntity
	{
		[InvisibleColumn] public int MetaTraderAccountId { get; set; }
		[InvisibleColumn] public MetaTraderAccount MetaTraderAccount { get; set; }

		[Required] public string Symbol { get; set; }
		public decimal? Multiplier { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace TradeSystem.Data.Models
{
	public class MetaTraderInstrumentConfig : BaseEntity
	{
		public int MetaTraderAccountId { get; set; }
		public MetaTraderAccount MetaTraderAccount { get; set; }

		[Required] public string Symbol { get; set; }
		public decimal? Multiplier { get; set; }
	}
}

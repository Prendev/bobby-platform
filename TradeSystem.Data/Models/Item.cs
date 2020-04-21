using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class Item : BaseDescriptionEntity
	{
		[InvisibleColumn]
		[Category("Relaciok")]
		[DisplayName("Arajanlat ID")]
		public int QuotationId { get; set; }

		[InvisibleColumn]
		[Category("Relaciok")]
		[DisplayName("Arajanlat")]
		public Quotation Quotation { get; set; }
	}
}

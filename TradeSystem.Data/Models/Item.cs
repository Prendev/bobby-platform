using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class Item : BaseDescriptionEntity
	{
		[InvisibleColumn]
		[Category("Habkitöltött alumínium redőny")]
		[DisplayName("Szelesseg (cm)")]
		public int ShutterWidth { get; set; }

		[InvisibleColumn]
		[Category("Habkitöltött alumínium redőny")]
		[DisplayName("Magassag (cm)")]
		public int ShutterHeight { get; set; }

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

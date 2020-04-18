using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
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
		[NotMapped]
		[Category("Habkitöltött alumínium redőny")]
		[DisplayName("Terulet (nm)")]
		public decimal ShutterArea => ShutterWidth * ShutterHeight / 10000m;

		[InvisibleColumn]
		[NotMapped]
		[Category("Habkitöltött alumínium redőny")]
		[DisplayName("Suly (kg)")]
		public decimal? ShutterWeight => ShutterArea * Quotation?.Profile?.ShutterWeight;

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

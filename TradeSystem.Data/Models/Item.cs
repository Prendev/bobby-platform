using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class Item : BaseDescriptionEntity
	{
		private const string ShutterCategory = "1 - Habkitöltött alumínium redőny";
		private const string MotorCategory = "2 - 220 V csőmotor";
		private const string RelationsCategory = "* - Relaciok";

		[InvisibleColumn]
		[Category(ShutterCategory)]
		[DisplayName("Szelesseg (cm)")]
		public int ShutterWidth { get; set; }

		[InvisibleColumn]
		[Category(ShutterCategory)]
		[DisplayName("Magassag (cm)")]
		public int ShutterHeight { get; set; }

		[InvisibleColumn]
		[NotMapped]
		[Category(ShutterCategory)]
		[DisplayName("Terulet (nm)")]
		public decimal ShutterArea => ShutterWidth * ShutterHeight / 10000m;

		[InvisibleColumn]
		[NotMapped]
		[Category(ShutterCategory)]
		[DisplayName("Suly (kg)")]
		public decimal? ShutterWeight => ShutterArea * Quotation?.Profile?.ShutterWeight;

		
		[InvisibleColumn]
		[Category(MotorCategory)]
		[DisplayName("Van motor?")]
		public bool HasMotor { get; set; }

		[InvisibleColumn]
		[Category(MotorCategory)]
		[DisplayName("Teljesitmeny (Nm)")]
		public int? Torque => ShutterArea > Quotation?.Profile?.SmallMotorLimit
			? Quotation?.Profile?.BigMotorTorque
			: Quotation?.Profile?.SmallMotorTorque;


		[InvisibleColumn]
		[Category(RelationsCategory)]
		[DisplayName("Arajanlat ID")]
		public int QuotationId { get; set; }

		[InvisibleColumn]
		[Category(RelationsCategory)]
		[DisplayName("Arajanlat")]
		public Quotation Quotation { get; set; }
	}
}

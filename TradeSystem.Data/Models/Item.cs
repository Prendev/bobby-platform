using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class Item : BaseDescriptionEntity
	{
		public enum ControlTypes
		{
			Nincs,
			Radios
		}

		public enum SensorTypes
		{
			Sima,
			Nyomatekos
		}

		public enum LegTypes
		{
			Szeles,
			Keskeny,
			Rolos,
			Oszto
		}

		private const string RelationsCategory = "* - Relaciok";
		private const string ShutterCategory = "1 - Habkitöltött alumínium redőny";
		private const string MotorCategory = "2 - 220 V csőmotor";
		private const string LegsCategory = "3 - Labak";


		[InvisibleColumn]
		[Category(RelationsCategory)]
		[DisplayName("Arajanlat ID")]
		public int QuotationId { get; set; }

		[InvisibleColumn]
		[Category(RelationsCategory)]
		[DisplayName("Arajanlat")]
		public Quotation Quotation { get; set; }


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
		[DisplayName("Eros motor (Nm)?")]
		public bool? Torque => ShutterArea > Quotation?.Profile?.SmallMotorLimitTorque;

		[InvisibleColumn]
		[Category(MotorCategory)]
		[DisplayName("Szeles motor (cm)?")]
		public bool? MotorWidth => ShutterWidth > Quotation?.Profile?.SmallMotorLimitWidth;

		[InvisibleColumn]
		[Category(MotorCategory)]
		[DisplayName("Vezérlés")]
		public ControlTypes ControlType { get; set; }

		[InvisibleColumn]
		[Category(MotorCategory)]
		[DisplayName("Erzekelo")]
		public SensorTypes SensorType { get; set; }


		[InvisibleColumn]
		[Category(LegsCategory)]
		[DisplayName("Bal lab")]
		public LegTypes LeftLegType { get; set; }

		[InvisibleColumn]
		[Category(LegsCategory)]
		[DisplayName("Jobb lab")]
		public LegTypes RightLegType { get; set; }
	}
}

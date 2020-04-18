using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class Profile : BaseDescriptionEntity
	{
		[InvisibleColumn]
		[Category("Keszito")]
		[DisplayName("Nev")]
		public string FullName { get; set; }

		[InvisibleColumn]
		[Category("Keszito")]
		[DisplayName("Telefonszam")]
		public string Telephone { get; set; }

		[InvisibleColumn]
		[Category("Keszito")]
		[DisplayName("E-mail cim")]
		public string Email { get; set; }

		[InvisibleColumn]
		[Category("Parameterek")]
		[DisplayName("Redony suly (kg/nm)")]
		public decimal ShutterWeight { get; set; } = 3.7m;
	}
}

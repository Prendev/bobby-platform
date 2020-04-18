﻿using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class Profile : BaseDescriptionEntity
    {
	    private const string MakerCategory = "1 - Keszito";
	    private const string ParametersCategory = "2 - Parameterek";

		[InvisibleColumn]
		[Category(MakerCategory)]
		[DisplayName("Nev")]
		public string FullName { get; set; }

		[InvisibleColumn]
		[Category(MakerCategory)]
		[DisplayName("Telefonszam")]
		public string Telephone { get; set; }

		[InvisibleColumn]
		[Category(MakerCategory)]
		[DisplayName("E-mail cim")]
		public string Email { get; set; }

		[InvisibleColumn]
		[Category(ParametersCategory)]
		[DisplayName("Redony suly (kg/nm)")]
		public decimal ShutterWeight { get; set; } = 3.7m;

		[InvisibleColumn]
		[Category(ParametersCategory)]
		[DisplayName("Kis motor limit - terulet (nm)")]
		public decimal SmallMotorLimitTorque { get; set; } = 1m;

	    [InvisibleColumn]
	    [Category(ParametersCategory)]
	    [DisplayName("Kis motor limit - szelesseg (cm)")]
	    public int SmallMotorLimitWidth { get; set; } = 80;
	}
}

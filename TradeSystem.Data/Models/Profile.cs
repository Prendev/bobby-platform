using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class Profile : BaseDescriptionEntity
    {
	    private const string MakerCategory = "1 - Keszito";
	    private const string ParametersCategory = "2 - Parameterek";
	    private const string MotorParametersCategory = "3 - Parameterek - motor";
	    private const string LegParametersCategory = "4 - Parameterek - labak";
	    private const string LathParametersCategory = "5 - Parameterek - léc";

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
		[Category(MotorParametersCategory)]
		[DisplayName("Kis motor limit - terulet (nm)")]
		public decimal SmallMotorLimitTorque { get; set; } = 1m;

	    [InvisibleColumn]
	    [Category(MotorParametersCategory)]
	    [DisplayName("Kis motor limit - szelesseg (cm)")]
	    public int SmallMotorLimitWidth { get; set; } = 80;


	    [InvisibleColumn]
	    [Category(LegParametersCategory)]
	    [DisplayName("Szeles lab kivonas (cm)")]
	    public decimal SzelesDeduction { get; set; } = -3.75m;

	    [InvisibleColumn]
	    [Category(LegParametersCategory)]
	    [DisplayName("Rolos lab kivonas (cm)")]
	    public decimal RolosDeduction { get; set; } = -4.25m;

	    [InvisibleColumn]
	    [Category(LegParametersCategory)]
	    [DisplayName("Oszto lab kivonas (cm)")]
	    public decimal OsztoDeduction { get; set; } = -1.75m;

	    [InvisibleColumn]
	    [Category(LegParametersCategory)]
	    [DisplayName("Keskeny lab kivonas (cm)")]
	    public decimal KeskenyDeduction { get; set; } = -1.25m;


	    [InvisibleColumn]
	    [Category(LathParametersCategory)]
	    [DisplayName("Záró kivonas (cm)")]
	    public decimal CloseDeduction { get; set; } = -1.5m;

	    [InvisibleColumn]
	    [Category(LathParametersCategory)]
	    [DisplayName("Szál oszto")]
	    public decimal RodDivider { get; set; } = 3.9m;

	    [InvisibleColumn]
	    [Category(LathParametersCategory)]
	    [DisplayName("Szál hozzáadás (cm)")]
	    public decimal RodAddition { get; set; } = 3m;
	}
}

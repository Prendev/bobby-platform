using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class Quotation : BaseDescriptionEntity
	{
		private const string RelationsCategory = "* - Relaciok";
		private const string CustomerCategory = "1 - Megrendelo";

		[InvisibleColumn]
		[Category(CustomerCategory)]
		[DisplayName("Nev")]
		public string FullName { get; set; }

		[InvisibleColumn]
		[Category(CustomerCategory)]
		[DisplayName("Cim")]
		public string Address { get; set; }

		[InvisibleColumn]
		[Category(CustomerCategory)]
		[DisplayName("Telefonszam")]
		public string Telephone { get; set; }

		[InvisibleColumn]
		[Category(CustomerCategory)]
		[DisplayName("E-mail cim")]
		public string Email { get; set; }


		[InvisibleColumn]
		[Category(RelationsCategory)]
		[DisplayName("Profil ID")]
		public int ProfileId { get; set; }

		[InvisibleColumn]
		[Category(RelationsCategory)]
		[DisplayName("Profil")]
		public Profile Profile { get; set; }
	}
}

using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class Quotation : BaseDescriptionEntity
	{
		[InvisibleColumn]
		[Category("Megrendelo")]
		[DisplayName("Nev")]
		public string FullName { get; set; }

		[InvisibleColumn]
		[Category("Megrendelo")]
		[DisplayName("Cim")]
		public string Address { get; set; }

		[InvisibleColumn]
		[Category("Megrendelo")]
		[DisplayName("Telefonszam")]
		public string Telephone { get; set; }

		[InvisibleColumn]
		[Category("Megrendelo")]
		[DisplayName("E-mail cim")]
		public string Email { get; set; }

		[InvisibleColumn]
		[Category("Relaciok")]
		[DisplayName("Profil ID")]
		public int ProfileId { get; set; }

		[InvisibleColumn]
		[Category("Relaciok")]
		[DisplayName("Profil")]
		public Profile Profile { get; set; }
	}
}

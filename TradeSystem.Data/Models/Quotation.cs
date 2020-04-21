using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class Quotation : BaseDescriptionEntity
	{
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

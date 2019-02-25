using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class Spoofing : BaseDescriptionEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public int FeedAccountId { get; set; }
		public Account FeedAccount { get => Get<Account>(); set => Set(value); }
		public string FeedSymbol { get; set; }

		public int SpoofAccountId { get; set; }
		public Account SpoofAccount { get => Get<Account>(); set => Set(value); }
		public string SpoofSymbol { get; set; }

		public int AlphaMasterId { get; set; }
		public Account AlphaMaster { get => Get<Account>(); set => Set(value); }
		[Required] public string AlphaSymbol { get; set; }

		public int BetaMasterId { get; set; }
		public Account BetaMaster { get => Get<Account>(); set => Set(value); }
		[Required] public string BetaSymbol { get; set; }
	}
}

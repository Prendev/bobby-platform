using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class Export : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public int AccountId { get; set; }
		public Account Account { get; set; }
		public string Symbol { get; set; }
		public string Group { get; set; }
	}
}

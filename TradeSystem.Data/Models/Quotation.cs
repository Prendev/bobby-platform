namespace TradeSystem.Data.Models
{
	public class Quotation : BaseDescriptionEntity
	{
		public int ProfileId { get; set; }
		public Profile Profile { get; set; }
	}
}

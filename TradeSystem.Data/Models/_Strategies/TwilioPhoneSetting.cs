namespace TradeSystem.Data.Models
{
	public class TwilioPhoneSetting : BaseEntity
	{
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
		public bool Active { get; set; }

		public override string ToString()
		{
			return (Id == 0 ? "UNSAVED - " : "") + Name + PhoneNumber;
		}
	}
}

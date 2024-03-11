namespace TradeSystem.Data.Models
{
	public class TelegramChatSetting : BaseEntity
	{
		public long ChatId { get; set; }
		public bool Active { get; set; }

		public override string ToString()
		{
			return (Id == 0 ? "UNSAVED - " : "") + ChatId;
		}
	}
}

namespace TradeSystem.Data.Models
{
	public class TelegramChatSetting : BaseEntity
	{
		public int? TelegramBotId { get; set; }
		public TelegramBot TelegramBot { get; set; }
		public long ChatId { get; set; }
		public bool Active { get; set; }

		public NotificationType? NotificationType { get; set; }
		public int CoolDownTimerInMin { get; set; }

		public string Message { get; set; }

		public override string ToString()
		{
			return (Id == 0 ? "UNSAVED - " : "") + ChatId + " - " + TelegramBot?.ToString();
		}
	}

	public enum NotificationType
	{
		Account_Margin_Error,
		Account_Disconnection,
		HighestTicketDuration,
		HighLowEquity,
		RunIsUnchecked
	}
}

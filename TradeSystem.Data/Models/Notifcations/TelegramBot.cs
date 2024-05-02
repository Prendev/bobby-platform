using Telegram.Bot.Types;
namespace TradeSystem.Data.Models
{
	public class TelegramBot : BaseDescriptionEntity
	{
		public string Token { get; set; }

		public override string ToString()
		{
			return $"{(Id == 0 ? "UNSAVED - " : "")}{Description}";
		}
	}
}

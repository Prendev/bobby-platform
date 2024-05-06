using System.ComponentModel;

namespace TradeSystem.Data.Models
{
    public class TelegramSetting : BaseEntity
    {
		[ReadOnly(true)]
        [DisplayName("Telegram Settings")]
		public string Key { get; set; }

		public string Value { get; set; }

        public override string ToString()
        {
            return (Id == 0 ? "UNSAVED - " : "") + Key;
        }
    }
}

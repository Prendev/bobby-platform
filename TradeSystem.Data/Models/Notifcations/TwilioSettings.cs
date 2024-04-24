using System.ComponentModel;

namespace TradeSystem.Data.Models
{
    public class TwilioSetting : BaseEntity
    {
		[ReadOnly(true)]
        [DisplayName("Twilio Settings")]
		public string Key { get; set; }

		public string Value { get; set; }

        public override string ToString()
        {
            return (Id == 0 ? "UNSAVED - " : "") + Key;
        }
    }
}

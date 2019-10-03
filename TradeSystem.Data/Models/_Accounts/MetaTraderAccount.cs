using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TradeSystem.Data.Models
{
    public class MetaTraderAccount : BaseDescriptionEntity
	{
        public int User { get; set; }
        [Required] public string Password { get; set; }
        public int MetaTraderPlatformId { get; set; }
        public MetaTraderPlatform MetaTraderPlatform { get; set; }

		public List<Account> Accounts { get; } = new List<Account>();
		public List<MetaTraderInstrumentConfig> InstrumentConfigs { get; } = new List<MetaTraderInstrumentConfig>();

		public override string ToString()
        {
            return $"{(Id == 0 ? "UNSAVED - " : "")}{Description} ({User})";
        }
    }
}

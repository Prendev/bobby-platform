using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static TradeSystem.Data.Models.Proxy;

namespace TradeSystem.Data.Models
{
    public class MetaTraderAccount : BaseDescriptionEntity
	{
        public int User { get; set; }
        [Required] public string Password { get; set; }
        public int MetaTraderPlatformId { get; set; }
        public MetaTraderPlatform MetaTraderPlatform { get; set; }
        public bool IsAlert { get; set; }
        public double MarginLevelAlert { get; set; }
        public double MarginLevelWarning { get; set; }
		public bool ProxyEnable { get; set; }
		public string ProxyHost { get; set; }
		public int ProxyPort { get; set; }
		public ProxyTypes ProxyType { get; set; }
		public string ProxyUser { get; set; }
		public string ProxyPassword { get; set; }

		public List<Account> Accounts { get; } = new List<Account>();
		public List<MetaTraderInstrumentConfig> InstrumentConfigs { get; } = new List<MetaTraderInstrumentConfig>();

		public override string ToString()
        {
            return $"{(Id == 0 ? "UNSAVED - " : "")}{Description} ({User})";
        }
    }
}

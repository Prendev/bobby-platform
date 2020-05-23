using System.Collections.Generic;

namespace TradeSystem.Data.Models
{
	public partial class BacktesterAccount : BaseDescriptionEntity
	{
		public int SleepInMs { get; set; }
		public List<BacktesterInstrumentConfig> InstrumentConfigs { get; } = new List<BacktesterInstrumentConfig>();
		public List<Account> Accounts { get; } = new List<Account>();
	}
}

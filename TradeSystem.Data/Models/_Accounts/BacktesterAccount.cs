using System.Collections.Generic;
using System.ComponentModel;

namespace TradeSystem.Data.Models
{
	public partial class BacktesterAccount : BaseDescriptionEntity
	{
		public int Instances { get; set; } = 1;
		[DisplayName("TickSleep")] public int TickSleepInMs { get; set; }
		public List<BacktesterInstrumentConfig> InstrumentConfigs { get; } = new List<BacktesterInstrumentConfig>();
		public List<Account> Accounts { get; } = new List<Account>();
	}
}

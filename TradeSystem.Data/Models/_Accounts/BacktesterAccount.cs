using System.Collections.Generic;

namespace TradeSystem.Data.Models
{
	public class BacktesterAccount : BaseDescriptionEntity
	{
		public List<BacktesterInstrumentConfig> InstrumentConfigs { get; } = new List<BacktesterInstrumentConfig>();
		public List<Account> Accounts { get; } = new List<Account>();
	}
}

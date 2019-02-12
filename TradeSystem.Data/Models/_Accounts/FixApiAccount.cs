using System.Collections.Generic;

namespace TradeSystem.Data.Models
{
	public class FixApiAccount : BaseDescriptionEntity
	{
		public string ConfigPath { get; set; }

		public List<Account> Accounts { get; } = new List<Account>();
	}
}

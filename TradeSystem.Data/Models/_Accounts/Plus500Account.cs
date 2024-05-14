using System.Collections.Generic;

namespace TradeSystem.Data.Models
{
	public class Plus500Account : BaseDescriptionEntity
	{
		public string ClientId { get; set; }
		public string SrvPath { get; set; }
		public List<Account> Accounts { get; } = new List<Account>();
	}
}

using System.Collections.Generic;

namespace TradeSystem.Data.Models
{
	public class IbAccount : BaseDescriptionEntity
	{
		public int Port { get; set; }
		public int ClientId { get; set; }

		public List<Account> Accounts { get; } = new List<Account>();
	}
}

using System.Collections.Generic;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class FixApiAccount : BaseDescriptionEntity
	{
		public string ConfigPath { get; set; }
		[InvisibleColumn] public RiskManagement RiskManagement { get; set; }
		public List<Account> Accounts { get; } = new List<Account>();
	}
}

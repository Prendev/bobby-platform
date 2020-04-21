using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TradeSystem.Data.Models
{
	public class CqgClientApiAccount : BaseDescriptionEntity
	{
		[Required] public string UserName { get; set; }

		public List<Account> Accounts { get; } = new List<Account>();
	}
}

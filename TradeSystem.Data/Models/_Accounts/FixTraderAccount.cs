using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TradeSystem.Data.Models
{
	public class FixTraderAccount : BaseDescriptionEntity
	{
        [Required] public string IpAddress { get; set; }
        public int CommandSocketPort { get; set; }
        public int EventsSocketPort { get; set; }

		public List<Account> Accounts { get; } = new List<Account>();
	}
}

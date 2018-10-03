using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
	public class IlyaFastFeedAccount : BaseDescriptionEntity
	{
		[Required] public string IpAddress { get; set; }
		public int Port { get; set; }
		public string UserName { get; set; }

		public List<Account> Accounts { get => Get(() => new List<Account>()); set => Set(value); }
	}
}

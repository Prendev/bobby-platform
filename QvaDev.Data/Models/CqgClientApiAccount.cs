using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
	public class CqgClientApiAccount : BaseDescriptionEntity
	{
		[Required] public string UserName { get; set; }
		[Required] public string Password { get; set; }

		public List<Account> Accounts { get => Get(() => new List<Account>()); set => Set(value); }
	}
}

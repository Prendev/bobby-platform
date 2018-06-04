using System.Collections.Generic;

namespace QvaDev.Data.Models
{
    public class BaseAccountEntity : BaseDescriptionEntity
    {
		public List<MonitoredAccount> MonitoredAccounts { get => Get(() => new List<MonitoredAccount>()); set => Set(value, false); }
	}
}

using System.Collections.Generic;

namespace QvaDev.Data.Models
{
    public class Profile : BaseDescriptionEntity
    {
        public List<Pushing> Pushings { get => Get(() => new List<Pushing>()); set => Set(value); }
		public List<Account> Accounts { get => Get(() => new List<Account>()); set => Set(value); }
		public List<Master> Masters { get => Get(() => new List<Master>()); set => Set(value); }
	}
}

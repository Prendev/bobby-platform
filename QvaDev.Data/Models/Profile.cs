using System.Collections.Generic;

namespace QvaDev.Data.Models
{
    public class Profile : BaseDescriptionEntity
    {
        public List<Pushing> Pushings { get; } = new List<Pushing>();
		public List<Account> Accounts { get; } = new List<Account>();
		public List<Master> Masters { get; } = new List<Master>();
	}
}

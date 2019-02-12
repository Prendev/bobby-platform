using System.Collections.Generic;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public partial class Master : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }
		[InvisibleColumn] public int AccountId { get; set; }
        public Account Account { get => Get<Account>(); set => Set(value); }

		public List<Slave> Slaves { get; } = new List<Slave>();
    }
}

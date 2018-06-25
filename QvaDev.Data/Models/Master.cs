using System.Collections.Generic;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class Master : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public bool Run { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }

        public List<Slave> Slaves { get => Get(() => new List<Slave>()); set => Set(value, false); }

        public override string ToString()
        {
            return (Id == 0 ? "UNSAVED - " : "") + Account;
        }
    }
}

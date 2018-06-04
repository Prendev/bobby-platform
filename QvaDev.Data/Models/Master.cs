using System.Collections.Generic;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class Master : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public bool Run { get; set; }
        public int MetaTraderAccountId { get; set; }
        public Account MetaTraderAccount { get; set; }

        public List<Slave> Slaves { get => Get(() => new List<Slave>()); set => Set(value, false); }

        public override string ToString()
        {
            return MetaTraderAccount.ToString();
        }
    }
}

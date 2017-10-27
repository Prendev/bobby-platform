using System.Collections.Generic;

namespace QvaDev.Data.Models
{
    public class Master : BaseEntity
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int MetaTraderAccountId { get; set; }
        public MetaTraderAccount MetaTraderAccount { get; set; }

        public List<Slave> Slaves { get => Get(() => new List<Slave>()); set => Set(value, false); }

        public override string ToString()
        {
            return $"{Group?.Description} | {MetaTraderAccount?.Description}";
        }
    }
}

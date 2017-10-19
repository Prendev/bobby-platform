using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace QvaDev.Data.Models
{
    public class Master : BaseEntity
    {
        [Required]
        public int GroupId { get; set; }
        [Required]
        public Group Group { get; set; }

        [Required]
        public int MetaTraderAccountId { get; set; }
        [Required]
        public MetaTraderAccount MetaTraderAccount { get; set; }

        public List<Slave> Slaves { get => Get(() => new List<Slave>()); set => Set(value, false); }

        public override string ToString()
        {
            return $"{Group?.Description} | {MetaTraderAccount?.Description}";
        }
    }
}

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
    }
}

using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class Slave : BaseEntity
    {
        [Required]
        public int MasterId { get; set; }
        [Required]
        public Master Master { get; set; }

        [Required]
        public int CTraderAccountId { get; set; }
        [Required]
        public CTraderAccount CTraderAccount { get; set; }
    }
}

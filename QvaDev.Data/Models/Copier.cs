using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class Copier : BaseEntity
    {
        [Required]
        public int SlaveId { get; set; }
        [Required]
        public Slave Slave { get; set; }

        [Required]
        public decimal CopyRatio { get; set; }

        public int? SlippageInPips { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class SymbolMapping : BaseEntity
    {
        [Required]
        public int SlaveId { get; set; }
        [Required]
        public Slave Slave { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }
    }
}

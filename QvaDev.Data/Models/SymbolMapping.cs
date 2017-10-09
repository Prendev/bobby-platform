using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class SymbolMapping : BaseEntity, IFilterableEntity
    {
        [Required]
        public int SlaveId { get; set; }
        [Required]
        public Slave Slave { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [NotMapped]
        [InvisibleColumn]
        public bool IsFiltered { get => Get<bool>(); set => Set(value); }
    }
}

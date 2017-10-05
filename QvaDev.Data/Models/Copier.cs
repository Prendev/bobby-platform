using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public enum MmTypes
    {
        Fixed,
        Ratio
    }

    public class Copier : BaseEntity
    {
        [Required]
        public int SlaveId { get; set; }
        [Required]
        public Slave Slave { get; set; }

        [Required]
        public MmTypes MmType { get; set; }

        [Required]
        public decimal MmValue { get; set; }

        public int? SlippageInPips { get; set; }

        public string SymbolSuffix { get; set; }
    }
}

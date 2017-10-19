using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class MetaTraderAccount : BaseAccountEntity
    {
        [Required]
        public long User { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int MetaTraderPlatformId { get; set; }
        [Required]
        public MetaTraderPlatform MetaTraderPlatform { get; set; }

        public override string ToString()
        {
            return $"{Description} ({User})";
        }
    }
}

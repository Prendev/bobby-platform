using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class CTraderAccount : BaseDescriptionEntity
    {
        [Required]
        public long AccountNumber { get; set; }

        [Required]
        public int CTraderPlatformId { get; set; }
        [Required]
        public CTraderPlatform CTraderPlatform { get; set; }
    }
}

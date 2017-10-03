using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class CTraderAccount
    {
        [Key]
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public long AccountNumber { get; set; }

        [Required]
        public int CTraderPlatformId { get; set; }
        [Required]
        public CTraderPlatform CTraderPlatform { get; set; }
    }
}

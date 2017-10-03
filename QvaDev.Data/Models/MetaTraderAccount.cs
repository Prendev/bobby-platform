using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class MetaTraderAccount
    {
        [Key]
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int MetaTraderPlatformId { get; set; }
        [Required]
        public MetaTraderPlatform MetaTraderPlatform { get; set; }
    }
}

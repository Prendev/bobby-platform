using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class CTraderPlatform
    {
        [Key]
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string AccountsApi { get; set; }

        [Required]
        public string TradingHost { get; set; }

        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        public string Playground { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class CTraderPlatform : BaseDescriptionEntity
    {
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

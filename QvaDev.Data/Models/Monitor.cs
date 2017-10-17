using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class Monitor : BaseDescriptionEntity
    {
        [Required]
        public int ProfileId { get; set; }
        [Required]
        public Profile Profile { get; set; }

        [Required]
        public string Symbol { get; set; }
    }
}

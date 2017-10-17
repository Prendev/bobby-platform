using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [NotMapped]
        [ReadOnly(true)]
        public double ExpectedLots { get => Get<double>(); set => Set(value); }
        [NotMapped]
        [ReadOnly(true)]
        public double ActualLots { get => Get<double>(); set => Set(value); }
    }
}

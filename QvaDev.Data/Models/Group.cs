using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class Group : BaseDescriptionEntity
    {
        [Required]
        public int ProfileId { get; set; }
        [Required]
        public Profile Profile { get; set; }
    }
}

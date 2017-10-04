using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public abstract class BaseDescriptionEntity : BaseEntity
    {
        [Required]
        public string Description { get; set; }
    }
}

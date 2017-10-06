using System.ComponentModel.DataAnnotations;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public abstract class BaseDescriptionEntity : BaseEntity
    {
        [Required]
        [DisplayIndex(0)]
        public string Description { get; set; }
    }
}

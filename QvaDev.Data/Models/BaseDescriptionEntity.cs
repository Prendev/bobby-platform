using System.ComponentModel.DataAnnotations;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public abstract class BaseDescriptionEntity : BaseEntity
    {
        [Required]
        [DisplayPriority(0)]
        public string Description { get; set; }

        public override string ToString()
        {
            return (Id == 0 ? "UNSAVED - " : "") + Description;
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public abstract class BaseEntity : BaseNotifyPropertyChange
	{
        [Key]
        [Dapper.Contrib.Extensions.Key]
        [InvisibleColumn]
		public int Id { get; set; }

        [NotMapped]
        [InvisibleColumn]
        public string DisplayMember => ToString();

        public override string ToString()
        {
            return Id == 0 ? "UNSAVED - " : Id.ToString();
        }
    }
}

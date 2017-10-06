using System.ComponentModel.DataAnnotations;
using QvaDev.Common;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public abstract class BaseEntity : BaseViewModel
    {
        [Key]
        [Dapper.Contrib.Extensions.Key]
        [InvisibleColumn]
        public int Id { get; set; }
    }
}

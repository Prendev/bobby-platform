using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public abstract class BaseEntity
    {
        [Key]
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }
    }
}

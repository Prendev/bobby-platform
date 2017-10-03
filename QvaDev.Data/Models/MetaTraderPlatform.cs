using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class MetaTraderPlatform
    {
        [Key]
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string SrvFilePath { get; set; }
    }
}

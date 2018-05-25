using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class FixTraderAccount : BaseAccountEntity
    {
        [Required]
        public string IpAddress { get; set; }
        public int CommandSocketPort { get; set; }
        public int EventsSocketPort { get; set; }
	}
}

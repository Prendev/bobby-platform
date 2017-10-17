using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QvaDev.Data.Models
{
    public class MonitoredAccount : BaseEntity
    {
        [Required]
        public int MonitorId { get; set; }
        [Required]
        public Monitor Monitor { get; set; }

        public int? CTraderAccountId { get; set; }
        public CTraderAccount CTraderAccount { get; set; }

        public int? MetaTraderAccountId { get; set; }
        public MetaTraderAccount MetaTraderAccount { get; set; }

        public string Symbol { get; set; }

        [Required]
        public long ExpectedContracts { get; set; }

        [NotMapped]
        [ReadOnly(true)]
        public long ActualContracts { get; set; }
    }
}

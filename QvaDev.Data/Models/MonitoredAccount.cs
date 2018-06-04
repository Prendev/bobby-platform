using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QvaDev.Data.Models
{
    public class MonitoredAccount : BaseEntity
    {
        public int MonitorId { get; set; }
        public Monitor Monitor { get; set; }

        public bool IsMaster { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

        public string Symbol { get; set; }

        public long ExpectedContracts { get; set; }

        [NotMapped] [ReadOnly(true)] public long ActualContracts { get; set; }
    }
}

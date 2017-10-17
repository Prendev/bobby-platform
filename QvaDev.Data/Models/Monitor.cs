using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QvaDev.Data.Models
{
    public class Monitor : BaseDescriptionEntity
    {
        [Required]
        public int ProfileId { get; set; }
        [Required]
        public Profile Profile { get; set; }

        [Required]
        public string Symbol { get; set; }

        [NotMapped]
        [ReadOnly(true)]
        public long ExpectedContracts { get; set; }
        [NotMapped]
        [ReadOnly(true)]
        public long ActualContracts { get; set; }

        public List<MonitoredAccount> MonitoredAccounts { get => Get(() => new List<MonitoredAccount>()); set => Set(value, false); }
    }
}

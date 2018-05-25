using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
    public class BaseAccountEntity : BaseDescriptionEntity
    {
        public enum States
        {
            Disconnected,
            Connected,
            Error
        }

	    [DisplayIndex(0)]
		public bool Run { get; set; }

	    [Required]
	    [DisplayIndex(1)]
	    public new string Description { get; set; }

		public List<MonitoredAccount> MonitoredAccounts { get => Get(() => new List<MonitoredAccount>()); set => Set(value, false); }

        [NotMapped] [InvisibleColumn] public IConnector Connector { get; set; }

        [NotMapped] [ReadOnly(true)] public States State { get; set; }

	    public override string ToString()
	    {
		    return Description;
	    }
	}
}

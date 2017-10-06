using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
    public class BaseAccountEntity : BaseDescriptionEntity
    {
        [NotMapped]
        [InvisibleColumn]
        public IConnector Connector { get; set; }
        [NotMapped]
        [ReadOnly(true)]
        public bool IsConnected { get => Get<bool>(); set => Set(value); }
    }
}

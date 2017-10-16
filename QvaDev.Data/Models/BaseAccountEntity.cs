using System.ComponentModel;
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

        [NotMapped]
        [InvisibleColumn]
        public IConnector Connector { get; set; }

        [NotMapped]
        [ReadOnly(true)]
        public States State { get; set; }

        //[NotMapped]
        //[ReadOnly(true)]
        //public bool IsConnected => State == States.Connected;
    }
}

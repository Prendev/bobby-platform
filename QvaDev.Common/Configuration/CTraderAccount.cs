using System.Xml.Serialization;

namespace QvaDev.Common.Configuration
{
    public class CTraderAccount : Account
    {
        public long AccountNumber { get; set; }
        public string AccessToken { get; set; }

        [XmlIgnore]
        public CTraderPlatform Platform { get; set; }
    }
}

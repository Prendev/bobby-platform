using System.Xml.Serialization;

namespace QvaDev.Common.Configuration
{
    public class Mt4Account : Account
    {
        public string Username { get; set; }
        public string Password { get; set; }

        [XmlIgnore]
        public Mt4Platform Platform { get; set; }
    }
}

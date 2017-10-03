using System.Collections.Generic;
using System.Xml.Serialization;
using QvaDev.Common.Configuration;

namespace QvaDev.Config
{
    public class SlaveAccountsSection
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "CTraderAccount")]
        public List<CTraderAccount> CTraderAccounts { get; set; }
    }
}

using System.Collections.Generic;
using System.Xml.Serialization;
using QvaDev.Common.Configuration;

namespace QvaDev.Configuration
{
    public class AccountsSection
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Mt4Account")]
        public List<Mt4Account> Mt4Accounts { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "CTraderAccount")]
        public List<CTraderAccount> CTraderAccounts { get; set; }
    }
}

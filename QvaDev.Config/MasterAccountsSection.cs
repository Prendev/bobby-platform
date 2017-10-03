using System.Collections.Generic;
using System.Xml.Serialization;
using QvaDev.Common.Configuration;

namespace QvaDev.Config
{
    public class MasterAccountsSection
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Mt4Account")]
        public List<Mt4Account> Mt4Accounts { get; set; }
    }
}

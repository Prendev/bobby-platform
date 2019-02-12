using System.Collections.Generic;
using System.Xml.Serialization;
using QvaDev.Common.Configuration;

namespace QvaDev.Config
{
    public class CommonConfigSection
    {
        public string OpenExchangeRatesAppId { get; set; }
        public int GuiRefreshPeriodInMilliseconds { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "CTraderPlatform")]
        public List<CTraderPlatform> CTraderPlatforms { get; set; }

        [XmlIgnore]
        public List<Mt4Platform> Mt4Platforms { get; set; }
    }
}

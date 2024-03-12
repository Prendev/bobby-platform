using mtapi.mt5;
using TradeSystem.Common.Integration;

namespace TradeSystem.FixApiIntegration
{
    public class AccountInfo : BaseAccountInfo
    {
        public string ConfigPath { get; set; }
        public PlacedType PlacedType { get; set; }
    }
}

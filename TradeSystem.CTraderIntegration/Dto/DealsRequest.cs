using System;

namespace TradeSystem.CTraderIntegration.Dto
{
    public class DealsRequest : AccountRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}

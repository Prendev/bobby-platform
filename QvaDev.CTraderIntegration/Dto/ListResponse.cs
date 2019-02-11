using System.Collections.Generic;

namespace TradeSystem.CTraderIntegration.Dto
{
    public class ListResponse<T>
    {
        public List<T> data { get; set; }
    }
}

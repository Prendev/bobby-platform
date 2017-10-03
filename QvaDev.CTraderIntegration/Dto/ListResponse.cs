using System.Collections.Generic;

namespace QvaDev.CTraderIntegration.Dto
{
    public class ListResponse<T>
    {
        public List<T> data { get; set; }
    }
}

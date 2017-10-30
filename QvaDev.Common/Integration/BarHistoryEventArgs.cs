using System.Collections.Generic;

namespace QvaDev.Common.Integration
{
    public delegate void BarHistoryEventHandler(object sender, BarHistoryEventArgs e);

    public class BarHistoryEventArgs
    {
        public string Symbol { get; set; }
        public List<Bar> BarHistory { get; set; }
    }
}

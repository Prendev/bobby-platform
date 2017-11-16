using System;
using System.Collections.Concurrent;

namespace QvaDev.Common.Integration
{
    public delegate void BarHistoryEventHandler(object sender, BarHistoryEventArgs e);

    public class BarHistoryEventArgs
    {
        public string Symbol { get; set; }
        public ConcurrentDictionary<DateTime, Bar> BarHistory { get; set; }
    }
}

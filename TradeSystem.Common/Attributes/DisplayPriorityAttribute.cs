using System;

namespace TradeSystem.Common.Attributes
{
    public class DisplayPriorityAttribute : Attribute
    {
        public int Priority { get; }
        public bool Reverse { get; }

        public DisplayPriorityAttribute(int priority, bool reverse = false)
        {
            Priority = priority;
	        Reverse = reverse;
        }
    }
}

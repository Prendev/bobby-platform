using System;

namespace QvaDev.Common.Attributes
{
    public class DisplayIndexAttribute : Attribute
    {
        public int Index { get; }

        public DisplayIndexAttribute(int index)
        {
            Index = index;
        }
    }
}

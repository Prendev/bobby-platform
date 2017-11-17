using System;

namespace QvaDev.Common.Integration
{
    public class Bar : IEquatable<Bar>
    {
        //public double Open { get; set; }
        public double Close { get; set; }
        //public double Low { get; set; }
        //public double High { get; set; }
        public DateTime OpenTime { get; set; }

        public bool Equals(Bar other)
        {
            return other != null && OpenTime.Equals(other.OpenTime);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = hash * 7 + OpenTime.GetHashCode();
                //hash = hash * 7 + Open.GetHashCode();
                //hash = hash * 7 + Close.GetHashCode();
                //hash = hash * 7 + Low.GetHashCode();
                //hash = hash * 7 + High.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Bar;
            return other != null && Equals(other);
        }
    }
}

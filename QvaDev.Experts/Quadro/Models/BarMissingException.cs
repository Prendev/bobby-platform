using System;

namespace QvaDev.Experts.Quadro.Models
{
    public class BarMissingException : Exception
    {
        public DateTime OpenTime { get; }

        public BarMissingException(DateTime openTime)
        {
            OpenTime = openTime;
        }
    }
}

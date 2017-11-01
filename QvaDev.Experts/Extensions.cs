using System;
using QvaDev.Common.Integration;

namespace QvaDev.Experts
{
    public static class Extensions
    {
        public static double MyRoundToDigits(this IConnector connector, string symbol, double value)
        {
            decimal dec = Convert.ToDecimal(value);
            dec = Math.Round(dec, connector.GetDigits(symbol), MidpointRounding.AwayFromZero);
            return Convert.ToDouble(dec);
        }
    }
}

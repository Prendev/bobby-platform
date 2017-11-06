using System;
using QvaDev.Common.Integration;

namespace QvaDev.Experts
{
    public static class Extensions
    {
        public static double CheckLot(this double lot)
        {
            decimal d = Convert.ToDecimal(lot);
            d = Math.Round(d, 2, MidpointRounding.AwayFromZero);
            lot = Convert.ToDouble(d);
            if (lot < 0.01)
            {
                lot = 0.01;
            }
            return lot;
        }

        public static double MyRoundToDigits(this IConnector connector, string symbol, double value)
        {
            decimal dec = Convert.ToDecimal(value);
            dec = Math.Round(dec, connector.GetDigits(symbol), MidpointRounding.AwayFromZero);
            return Convert.ToDouble(dec);
        }
    }
}

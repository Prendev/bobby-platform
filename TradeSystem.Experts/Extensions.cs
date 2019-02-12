using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static string AsString(this List<double> list, int count = 5)
        {
            if (list == null || !list.Any()) return "[]";
            var sb = new StringBuilder();
            sb.Append("[ ");
            foreach (var item in list.Take(Math.Min(5, list.Count)))
            {
                sb.Append(item);
                sb.Append(" | ");
            }
            sb.Append("... ]");
            return sb.ToString();
        }

        public static Sides GetInverseOrder(this Sides orderType)
        {
            return orderType == Sides.Buy ? Sides.Sell : Sides.Buy;
        }
    }
}

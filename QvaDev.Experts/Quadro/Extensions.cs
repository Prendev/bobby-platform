using System;
using System.Collections.Generic;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro
{
    public static class Extensions
    {
        public static double BuyAveragePrice(this ExpertSetWrapper exp)
        {
            return exp.AveragePrice(exp.Sym1MinOrderType, exp.Sym2MinOrderType, exp.SpreadBuyMagicNumber);
        }

        public static double SellAveragePrice(this ExpertSetWrapper exp)
        {
            return exp.AveragePrice(exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, exp.SpreadSellMagicNumber);
        }

        private static double AveragePrice(this ExpertSetWrapper exp, Sides orderType1, Sides orderType2, int magicNumber)
        {
            double avgPrice = 0;
            double[] sumLotSum2 = exp.GetSumAndLotSum(exp.E.Symbol1, orderType2, magicNumber);
            double[] sumLotSum1 = exp.GetSumAndLotSum(exp.E.Symbol2, orderType1, magicNumber);
            double multiSum = sumLotSum1[0] + sumLotSum2[0];
            double lotSum = sumLotSum1[1] + sumLotSum2[1];
            if (lotSum > 0)
            {
                avgPrice = exp.Connector.MyRoundToDigits(exp.E.Symbol1, multiSum / lotSum);
            }
            return avgPrice;
        }

        private static double[] GetSumAndLotSum(this ExpertSetWrapper exp, string symbol, Sides orderType, int magicNumber)
        {
            double multiSum = 0;
            double lotSum = 0;
            foreach (var p in exp.GetOpenOrdersList(symbol, orderType, magicNumber))
            {
                double sellMultiplication = exp.Connector.MyRoundToDigits(p.Symbol, exp.BarQuant(p) * p.Lots);
                multiSum += sellMultiplication;
                lotSum += p.Lots;
            }
            return new[] { multiSum, lotSum };
        }

        public static List<Position> GetOpenOrdersList(this ExpertSetWrapper exp, string symbol, Sides orderType,
            int magicNumber)
        {
            return exp.Connector.Positions.Select(p => p.Value)
                .Where(p => p.Symbol == symbol && p.Side == orderType && p.MagicNumber == magicNumber)
                .ToList();
        }
        public static List<Position> GetOpenOrdersList(this ExpertSetWrapper exp, string symbol1, Sides orderType1,
            string symbol2, Sides orderType2, int magicNumber)
        {
            return exp.Connector.Positions.Select(p => p.Value)
                .Where(p => p.MagicNumber == magicNumber &&
                            ((p.Symbol == symbol1 && p.Side == orderType1) ||
                             (p.Symbol == symbol2 && p.Side == orderType2)))
                .ToList();
        }

        public static double BarQuant(this ExpertSetWrapper exp, Position p)
        {
            var orderCreationTime = p.OpenTime;
            int barIndex = exp.GetBarIndexForTime(orderCreationTime, p.Symbol);
            return barIndex >= 0 ? exp.Quants[barIndex + 1] : 0;
        }

        public static int GetBarIndexForTime(this ExpertSetWrapper exp, DateTime timeInBar, string symbol,
            bool exact = false)
        {
            int i = 0;
            var historyBar = exp.GetHistoryBar(symbol);
            while (i < historyBar?.Count)
            {
                Bar bar = exp.Bar(symbol, i);
                bool flag;
                if (exact || !(timeInBar >= bar.OpenTime)) flag = exact && timeInBar == bar.OpenTime;
                else flag = true;
                if (!flag) i++;
                else return i;
            }
            return -1;
        }

        private static List<Bar> GetHistoryBar(this ExpertSetWrapper exp, string symbol)
        {
            if (exp.E.Symbol1 == symbol)
                return exp.BarHistory1;
            if (exp.E.Symbol2 == symbol)
                return exp.BarHistory2;
            return null;
        }

        public static Bar Bar(this ExpertSetWrapper exp, string symbol, int index)
        {
            if (exp.E.Symbol1 == symbol)
                return exp.BarHistory1[index];
            if (exp.E.Symbol2 == symbol)
                return exp.BarHistory2[index];
            return null;
        }

        public static int GetMagicNumberBySpreadOrderType(this ExpertSetWrapper exp, Sides spreadOrderType)
        {
            return spreadOrderType != Sides.Buy ? exp.SpreadSellMagicNumber : exp.SpreadBuyMagicNumber;
        }

        public static IEnumerable<Position> GetBaseOpenOrdersList(this ExpertSetWrapper exp, Sides spreadOrderType)
        {
            var orders = spreadOrderType != Sides.Buy
                ? exp.GetOpenOrdersList(exp.E.Symbol1, exp.Sym1MaxOrderType, exp.E.Symbol2, exp.Sym2MaxOrderType,
                    exp.SpreadSellMagicNumber)
                : exp.GetOpenOrdersList(exp.E.Symbol1, exp.Sym1MinOrderType, exp.E.Symbol2, exp.Sym2MinOrderType,
                    exp.SpreadBuyMagicNumber);
            return orders;
        }

        public static bool IsInDeltaRange(this ExpertSetWrapper exp, Sides side)
        {
            bool sym1InRange;
            bool sym2InRange;
            var deltaRange = exp.Point;
            if (side != Sides.Sell)
            {
                sym1InRange = IsInDeltaRange(exp.Sym1LastMinActionPrice, deltaRange, exp.BarHistory1.Last().Close);
                sym2InRange = IsInDeltaRange(exp.Sym2LastMinActionPrice, deltaRange, exp.BarHistory2.Last().Close);
            }
            else
            {
                sym1InRange = IsInDeltaRange(exp.Sym1LastMaxActionPrice, deltaRange, exp.BarHistory1.Last().Close);
                sym2InRange = IsInDeltaRange(exp.Sym2LastMaxActionPrice, deltaRange, exp.BarHistory2.Last().Close);
            }
            return sym1InRange | sym2InRange;
        }

        private static bool IsInDeltaRange(double price, double range, double close)
        {
            double diff = Math.Abs(price - close);
            return diff < range;
        }
    }
}

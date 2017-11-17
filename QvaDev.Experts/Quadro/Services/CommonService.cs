using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;
using QvaDev.Common;

namespace QvaDev.Experts.Quadro.Services
{
    public interface ICommonService
    {
        List<Position> GetOpenOrdersList(ExpertSetWrapper exp, string symbol, Sides orderType, int magicNumber);
        List<Position> GetOpenOrdersList(ExpertSetWrapper exp, string symbol1, Sides orderType1,
            string symbol2, Sides orderType2, int magicNumber);
        double BarQuant(ExpertSetWrapper exp, Position p);
        int GetMagicNumberBySpreadOrderType(ExpertSetWrapper exp, Sides spreadOrderType);
        IEnumerable<Position> GetBaseOpenOrdersList(ExpertSetWrapper exp, Sides spreadOrderType);
        void SetLastActionPrice(ExpertSetWrapper exp, Sides side);
        bool IsInDeltaRange(ExpertSetWrapper exp, Sides side);
        double CalculateBaseOrdersProfit(ExpertSetWrapper exp, Sides side);
        double CalculateProfit(ExpertSetWrapper exp, int magicNumber, Sides orderType1, Sides orderType2);
    }

    public class CommonService : ICommonService
    {
        private readonly ILog _log;

        public CommonService(ILog log)
        {
            _log = log;
        }

        public double CalculateBaseOrdersProfit(ExpertSetWrapper exp, Sides side)
        {
            return side == Sides.Sell
                ? CalculateProfit(exp, exp.SpreadSellMagicNumber, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType)
                : CalculateProfit(exp, exp.SpreadBuyMagicNumber, exp.Sym1MinOrderType, exp.Sym2MinOrderType);
        }

        public double CalculateProfit(ExpertSetWrapper exp, int magicNumber, Sides orderType1, Sides orderType2)
        {
            return exp.Connector.CalculateProfit(magicNumber, exp.E.Symbol1, orderType1, exp.E.Symbol2, orderType2);
        }

        public List<Position> GetOpenOrdersList(ExpertSetWrapper exp, string symbol, Sides orderType,
            int magicNumber)
        {
            return exp.OpenPositions
                .Where(p => p.Symbol == symbol && p.Side == orderType && p.MagicNumber == magicNumber)
                .ToList();
        }
        public List<Position> GetOpenOrdersList(ExpertSetWrapper exp, string symbol1, Sides orderType1,
            string symbol2, Sides orderType2, int magicNumber)
        {
            return exp.OpenPositions
                .Where(p => p.MagicNumber == magicNumber &&
                            (p.Symbol == symbol1 && p.Side == orderType1 ||
                             p.Symbol == symbol2 && p.Side == orderType2))
                .ToList();
        }

        public double BarQuant(ExpertSetWrapper exp, Position p)
        {
            var dateTimeKey = p.OpenTime.AddMinutes(-(int)exp.E.TimeFrame).RoundDown(TimeSpan.FromMinutes((int) exp.E.TimeFrame));
            if(!exp.BarQuants.ContainsKey(dateTimeKey) || !exp.BarQuants[dateTimeKey].Quant.HasValue)
                throw new BarMissingException(dateTimeKey);
            return exp.BarQuants[dateTimeKey].Quant.Value;
        }

        public DateTime RoundDown(DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }

        public int GetMagicNumberBySpreadOrderType(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            return spreadOrderType != Sides.Buy ? exp.SpreadSellMagicNumber : exp.SpreadBuyMagicNumber;
        }

        public IEnumerable<Position> GetBaseOpenOrdersList(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            var orders = spreadOrderType != Sides.Buy
                ? GetOpenOrdersList(exp, exp.E.Symbol1, exp.Sym1MaxOrderType, exp.E.Symbol2, exp.Sym2MaxOrderType,
                    exp.SpreadSellMagicNumber)
                : GetOpenOrdersList(exp, exp.E.Symbol1, exp.Sym1MinOrderType, exp.E.Symbol2, exp.Sym2MinOrderType,
                    exp.SpreadBuyMagicNumber);
            return orders;
        }

        public void SetLastActionPrice(ExpertSetWrapper exp, Sides side)
        {
            if (side == Sides.Sell)
            {
                exp.E.Sym1LastMaxActionPrice = exp.Connector.GetLastTick(exp.E.Symbol1)?.Bid ?? 0;
                exp.E.Sym2LastMaxActionPrice = exp.Connector.GetLastTick(exp.E.Symbol2)?.Bid ?? 0;
            }
            else
            {
                exp.E.Sym1LastMinActionPrice = exp.Connector.GetLastTick(exp.E.Symbol1)?.Bid ?? 0;
                exp.E.Sym2LastMinActionPrice = exp.Connector.GetLastTick(exp.E.Symbol2)?.Bid ?? 0;
            }
        }

        public bool IsInDeltaRange(ExpertSetWrapper exp, Sides side)
        {
            bool sym1InRange;
            bool sym2InRange;

            if (side == Sides.Buy)
            {
                sym1InRange = IsInDeltaRange(exp.E.Sym1LastMinActionPrice, exp.DeltaRange, exp.LatestBarQuant.Bar1.Close);
                sym2InRange = IsInDeltaRange(exp.E.Sym2LastMinActionPrice, exp.DeltaRange, exp.LatestBarQuant.Bar2.Close);
            }
            else
            {
                sym1InRange = IsInDeltaRange(exp.E.Sym1LastMaxActionPrice, exp.DeltaRange, exp.LatestBarQuant.Bar1.Close);
                sym2InRange = IsInDeltaRange(exp.E.Sym2LastMaxActionPrice, exp.DeltaRange, exp.LatestBarQuant.Bar2.Close);
            }
            return sym1InRange || sym2InRange;
        }

        private static bool IsInDeltaRange(double price, double range, double close)
        {
            double diff = Math.Abs(price - close);
            return diff < range;
        }
    }
}

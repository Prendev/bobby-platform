using System;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;

namespace QvaDev.Experts.Quadro.Services
{
    public interface ICloseService
    {
        void CheckClose(ExpertSetWrapper expertSet);
    }

    public class CloseService : ICloseService
    {
        public void CheckClose(ExpertSetWrapper expertSet)
        {
            CheckQuantClose(expertSet, Sides.Sell);
            CheckQuantClose(expertSet, Sides.Buy);
        }

        private void CheckQuantClose(ExpertSetWrapper expertSet, Sides side)
        {
            if (IsInDeltaRange(expertSet, side)) return;
            bool closeEnabled = true;
            if (expertSet.BaseTradesForPositiveClose)
            {
                //TODO double hedgeProfit = Hedge.CalculateProfit(side);
                double hedgeProfit = 0;
                double baseProfit = CalculateBaseOrdersProfit(expertSet, side);
                closeEnabled = !expertSet.HedgeTradeForPositiveClose ? baseProfit > 0 : baseProfit + hedgeProfit > 0;
            }
            if (!closeEnabled) return;
            if (side != Sides.Sell)
            {
                //TODO CheckQuantBuyClose(expertSet);
            }
            else
            {
                //TODO CheckQuantSellClose();
            }
        }

        private double CalculateBaseOrdersProfit(ExpertSetWrapper expertSet, Sides side)
        {
            double num = side != Sides.Buy
                ? CalculateProfit(expertSet, Sides.Buy, Sides.Sell, expertSet.SpreadSellMagicNumber)
                : CalculateProfit(expertSet, Sides.Sell, Sides.Buy, expertSet.SpreadBuyMagicNumber);
            return num;
        }

        private bool IsInDeltaRange(ExpertSetWrapper expertSet, Sides side)
        {
            bool sym1InRange;
            bool sym2InRange;
            var deltaRange = expertSet.TradingAccount.MetaTraderAccount.Connector.GetPoint(expertSet.Symbol1);
            if (side != Sides.Sell)
            {
                sym1InRange = IsInDeltaRange(expertSet.Sym1LastMinActionPrice, deltaRange, expertSet.BarHistory1.Last().Close);
                sym2InRange = IsInDeltaRange(expertSet.Sym2LastMinActionPrice, deltaRange, expertSet.BarHistory2.Last().Close);
            }
            else
            {
                sym1InRange = IsInDeltaRange(expertSet.Sym1LastMaxActionPrice, deltaRange, expertSet.BarHistory1.Last().Close);
                sym2InRange = IsInDeltaRange(expertSet.Sym2LastMaxActionPrice, deltaRange, expertSet.BarHistory2.Last().Close);
            }
            return sym1InRange | sym2InRange;
        }

        private bool IsInDeltaRange(double price, double range, double close)
        {
            double diff = Math.Abs(price - close);
            return diff < range;
        }

        public double CalculateProfit(ExpertSetWrapper expertSet, Sides side1, Sides side2, int magicNumber)
        {
            var connector = expertSet.TradingAccount.MetaTraderAccount.Connector;
            var positions = connector.Positions.Where(p => p.Value.MagicNumber == magicNumber &&
                                                           (p.Value.Symbol == expertSet.Symbol1 && p.Value.Side == side1 ||
                                                            p.Value.Symbol == expertSet.Symbol2 && p.Value.Side == side2));
            return positions.Sum(p => p.Value.NetProfit);
        }
    }
}

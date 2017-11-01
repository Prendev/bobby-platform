using System;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;

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

        private void CheckQuantClose(ExpertSetWrapper exp, Sides side)
        {
            if (IsInDeltaRange(exp, side)) return;
            bool closeEnabled = true;
            if (exp.BaseTradesForPositiveClose)
            {
                //TODO double hedgeProfit = Hedge.CalculateProfit(side);
                double hedgeProfit = 0;
                double baseProfit = CalculateBaseOrdersProfit(exp, side);
                closeEnabled = !exp.HedgeTradeForPositiveClose ? baseProfit > 0 : baseProfit + hedgeProfit > 0;
            }
            if (!closeEnabled) return;
            if (side != Sides.Sell)
            {
                CheckQuantBuyClose(exp);
            }
            else
            {
                //TODO CheckQuantSellClose();
            }
        }

        private double CalculateBaseOrdersProfit(ExpertSetWrapper exp, Sides side)
        {
            double num = side != Sides.Buy
                ? CalculateProfit(exp, Sides.Buy, Sides.Sell, exp.SpreadSellMagicNumber)
                : CalculateProfit(exp, Sides.Sell, Sides.Buy, exp.SpreadBuyMagicNumber);
            return num;
        }

        private bool IsInDeltaRange(ExpertSetWrapper exp, Sides side)
        {
            bool sym1InRange;
            bool sym2InRange;
            var deltaRange = exp.Connector.GetPoint(exp.Symbol1);
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

        private bool IsInDeltaRange(double price, double range, double close)
        {
            double diff = Math.Abs(price - close);
            return diff < range;
        }

        public double CalculateProfit(ExpertSetWrapper exp, Sides side1, Sides side2, int magicNumber)
        {
            var positions = exp.Connector.Positions.Where(p => p.Value.MagicNumber == magicNumber &&
                                                           (p.Value.Symbol == exp.Symbol1 && p.Value.Side == side1 ||
                                                            p.Value.Symbol == exp.Symbol2 && p.Value.Side == side2));
            return positions.Sum(p => p.Value.NetProfit);
        }

        private void CheckQuantBuyClose(ExpertSetWrapper exp)
        {
            //double buyAvgPrice = BuyAveragePrice();
            //if (!exp.PartialClose)
            //{
            //    if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp1 * exp.Connector.GetPoint(exp.Symbol1)) &&
            //        buyAvgPrice != 0)
            //    {
            //        AllCloseMinSet();
            //    }
            //}
            //else if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp1 * exp.Connector.GetPoint(exp.Symbol1)) &&
            //         buyAvgPrice != 0 &&
            //         CurrentBuyState == TradeSetState.TRADE_OPENED)
            //{
            //    FirstMinCloseLevel();
            //}
            //else if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp2 * exp.Connector.GetPoint(exp.Symbol1)) &&
            //         buyAvgPrice != 0 &&
            //         CurrentBuyState == TradeSetState.AFTER_FIRST_CLOSE)
            //{
            //    SecondMinCloseLevel();
            //}
            //else if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp3 * exp.Connector.GetPoint(exp.Symbol1)) &&
            //         buyAvgPrice != 0 &&
            //         CurrentBuyState == TradeSetState.AFTER_SECOND_CLOSE)
            //{
            //    AllCloseMinSet();
            //}
        }
    }
}

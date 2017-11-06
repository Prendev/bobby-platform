using System;
using System.Collections.Generic;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Services
{
    public interface IReentriesService
    {
        void CalculateReentries(ExpertSetWrapper expertSet);
    }

    public class ReentriesService : IReentriesService
    {
        public void CalculateReentries(ExpertSetWrapper exp)
        {
            bool sellReentryEnabled = !exp.IsInDeltaRange(Sides.Sell) && exp.SellOpenCount < exp.MaxTradeSetCount &&
                                      EnableLast24Filter(exp, Sides.Sell, 2);
            bool buyReentryEnabled = !exp.IsInDeltaRange(Sides.Buy) && exp.BuyOpenCount < exp.MaxTradeSetCount &&
                                     EnableLast24Filter(exp, Sides.Buy, 2);

            if (sellReentryEnabled && !(exp.QuantStoAvg < exp.StochMaxAvgOpen) && exp.QuantWprAvg > -exp.WprMinAvgOpen)
            {
                var o1 = LastOrder(exp, exp.Symbol1, exp.Sym1MaxOrderType, exp.SpreadSellMagicNumber);
                var o2 = LastOrder(exp, exp.Symbol2, exp.Sym2MaxOrderType, exp.SpreadSellMagicNumber);
                int buyReopenDiff = GetReopenDiff(exp, exp.BuyOpenCount);
                if (o1 != null && o2 != null && !(exp.Quants.Last() < exp.BarQuant(o1) + buyReopenDiff * exp.Point) &&
                    exp.Quants.Last() >= exp.BarQuant(o2) + buyReopenDiff * exp.Point)
                {
                    CorrectLotArrayIfNeeded(exp, Sides.Sell);
                    double lot1 = exp.SellLots[exp.SellOpenCount, 1].CheckLot();
                    double lot2 = exp.SellLots[exp.SellOpenCount, 0].CheckLot();

                    exp.Sym1LastMaxActionPrice =
                        exp.Connector.SendMarketOrderRequest(exp.Symbol1, exp.Sym1MaxOrderType, lot1, exp.SpreadSellMagicNumber);
                    exp.Sym2LastMaxActionPrice =
                        exp.Connector.SendMarketOrderRequest(exp.Symbol2, exp.Sym2MaxOrderType, lot2, exp.SpreadSellMagicNumber);

                    //TODO
                    //Hedge.OnBaseTradesOpened(Sides.Sell, new[] {lot1, lot2});
                    //List<Order> hedgeOpenOrders = Hedge.GetOrdersToOpen();
                    //PostReOpenTradeSetOperation(hedgeOpenOrders, Sides.Sell, order1, order2);
                    //PostCloseHedgePositions(Sides.Sell);
                }
            }
            if (buyReentryEnabled && !(exp.QuantStoAvg > exp.StochMinAvgOpen) && exp.QuantWprAvg < -exp.WprMaxAvgOpen)
            {
                var o1 = LastOrder(exp, exp.Symbol1, exp.Sym1MinOrderType, exp.SpreadBuyMagicNumber);
                var o2 = LastOrder(exp, exp.Symbol2, exp.Sym2MinOrderType, exp.SpreadBuyMagicNumber);
                int sellReopenDiff = GetReopenDiff(exp, exp.SellOpenCount);
                if (o1 != null && o2 != null && !(exp.Quants.Last() > exp.BarQuant(o1) - sellReopenDiff * exp.Point) &&
                    exp.Quants.Last() <= exp.BarQuant(o2) - sellReopenDiff * exp.Point)
                {
                    CorrectLotArrayIfNeeded(exp, Sides.Buy);
                    double lot1 = exp.BuyLots[exp.BuyOpenCount, 1].CheckLot();
                    double lot2 = exp.BuyLots[exp.BuyOpenCount, 0].CheckLot();


                    exp.Sym1LastMinActionPrice =
                        exp.Connector.SendMarketOrderRequest(exp.Symbol1, exp.Sym1MinOrderType, lot1, exp.SpreadBuyMagicNumber);
                    exp.Sym2LastMinActionPrice =
                        exp.Connector.SendMarketOrderRequest(exp.Symbol2, exp.Sym2MinOrderType, lot2, exp.SpreadBuyMagicNumber);

                    //TODO
                    //Hedge.OnBaseTradesOpened(OrderType.Buy, new[] {lot1, lot2});
                    //List<Order> hedgeOpenOrders = Hedge.GetOrdersToOpen();
                    //PostReOpenTradeSetOperation(hedgeOpenOrders, Sides.Buy, order1, order2);
                    //PostCloseHedgePositions(Sides.Buy);
                }
            }
        }

        private bool EnableLast24Filter(ExpertSetWrapper exp, Sides spreadOrderType, int numOfTradePerOpen)
        {
            DateTime dateTime = exp.BarHistory1.Last().OpenTime;
            return GetBaseOpenOrdersList(exp, spreadOrderType).Where(o => (dateTime - o.OpenTime).TotalHours >= 24)
                       .ToList().Count < numOfTradePerOpen * exp.Last24HMaxOpen;
        }

        private IEnumerable<Position> GetBaseOpenOrdersList(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            var orders = spreadOrderType != Sides.Buy
                ? exp.GetOpenOrdersList(exp.Symbol1, exp.Sym1MaxOrderType, exp.Symbol2, exp.Sym2MaxOrderType,
                    exp.SpreadSellMagicNumber)
                : exp.GetOpenOrdersList(exp.Symbol1, exp.Sym1MinOrderType, exp.Symbol2, exp.Sym2MinOrderType,
                    exp.SpreadBuyMagicNumber);
            return orders;
        }

        private Position LastOrder(ExpertSetWrapper exp, string symbol, Sides orderType, int magicNumber)
        {
            return exp.GetOpenOrdersList(symbol, orderType, magicNumber).OrderByDescending(p => p.OpenTime).FirstOrDefault();
        }

        private int GetReopenDiff(ExpertSetWrapper exp, int openCount)
        {
            return openCount >= exp.ReOpenDiffChangeCount ? exp.ReOpenDiff2 : exp.ReOpenDiff;
        }

        private void CorrectLotArrayIfNeeded(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            Sides sym1OrderType;
            double[,] lotArray;
            int magicNumber;
            if (spreadOrderType != Sides.Sell)
            {
                sym1OrderType = exp.Sym1MinOrderType;
                lotArray = exp.BuyLots;
                magicNumber = exp.SpreadBuyMagicNumber;
            }
            else
            {
                sym1OrderType = exp.Sym1MaxOrderType;
                lotArray = exp.SellLots;
                magicNumber = exp.SpreadSellMagicNumber;
            }
            double firstLot = lotArray[0, 1];
            var firstOrder = FirstOrder(exp, exp.Symbol1, sym1OrderType, magicNumber);
            
            if (firstOrder != null && Math.Abs(firstOrder.Lots - firstLot) >= 1E-05)
            {
                MultiplyLotArray(lotArray, firstOrder.Lots / firstLot);
            }
        }

        private Position FirstOrder(ExpertSetWrapper exp, string symbol, Sides side, int magicNumber)
        {
            return exp.Connector.Positions.Where(p => p.Value.Symbol == symbol && p.Value.Side == side &&
                                                      p.Value.MagicNumber == magicNumber)
                .Select(p => p.Value)
                .OrderBy(p => p.OpenTime)
                .FirstOrDefault();
        }

        private void MultiplyLotArray(double[,] lotArray, double multiplier)
        {
            for (int i = 0; i < 120; i++)
            {
                lotArray[i, 0] = (lotArray[i, 0] * multiplier).CheckLot();
                lotArray[i, 1] = (lotArray[i, 1] * multiplier).CheckLot();
            }
        }
    }
}

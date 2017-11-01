using System;
using System.Collections.Generic;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Services
{
    public interface IEntriesService
    {
        void CalculateEntries(ExpertSetWrapper expertSet);
    }
    public class EntriesService : IEntriesService
    {
        public void CalculateEntries(ExpertSetWrapper expertSet)
        {
            CalculateEntriesForMaxAction(expertSet);
            CalculateEntriesForMinAction(expertSet);
        }

        private void CalculateEntriesForMaxAction(ExpertSetWrapper exp)
        {
            if (exp.QuantStoAvg <= exp.StochMaxAvgOpen || !(exp.QuantWprAvg > -exp.WprMinAvgOpen)) return;
            int numOfSym1MaxOrders = MyOrdersCount(exp, exp.Symbol1, exp.Sym1MaxOrderType);
            int numOfSym2MaxOrders = MyOrdersCount(exp, exp.Symbol2, exp.Sym2MaxOrderType);
            if (numOfSym1MaxOrders != 0 || numOfSym2MaxOrders != 0) return;
            var initialLots = exp.InitialLots;
            double lot1 = CheckLot(initialLots[0, 1]);
            double lot2 = CheckLot(initialLots[0, 0]);
            var openPrice1 = exp.Connector.SendMarketOrderRequest(exp.Symbol1, exp.Sym1MaxOrderType, lot1, exp.SpreadSellMagicNumber);
            var openPrice2 = exp.Connector.SendMarketOrderRequest(exp.Symbol2, exp.Sym2MaxOrderType, lot2, exp.SpreadSellMagicNumber);

            exp.Sym1LastMaxActionPrice = openPrice1;
            exp.Sym2LastMaxActionPrice = openPrice2;

            //TODO
            //Order pos1 = OpenBasePosition(Symbol1, Sym1MaxOrderType, lot1, spreadSellMagicNumber);
            //Order pos2 = OpenBasePosition(Symbol2, Sym2MaxOrderType, lot2, spreadSellMagicNumber);
            //List<Order> baseOrders = OrderUtil.GetOrderList(pos1, pos2);
            //Hedge.OnBaseTradesOpened(OrderType.Sell, new[] { lot1, lot2 });
            //if (!(exposureShieldHandler?.EnableOpeningOrder(baseOrders) ?? true)) return;
            //base.SetLastActionPrice(OrderType.Sell, Symbol1, Symbol2);
            //PostOpenTradeSetOperation(Hedge.GetOrdersToOpen(), OrderType.Sell, baseOrders);
        }

        protected void CalculateEntriesForMinAction(ExpertSetWrapper exp)
        {
            if (exp.QuantStoAvg >= exp.StochMinAvgOpen || !(exp.QuantWprAvg < -exp.WprMaxAvgOpen)) return;
            int numOfSym1MaxOrders = MyOrdersCount(exp, exp.Symbol1, exp.Sym1MinOrderType);
            int numOfSym2MaxOrders = MyOrdersCount(exp, exp.Symbol2, exp.Sym2MinOrderType);
            if (numOfSym1MaxOrders != 0 || numOfSym2MaxOrders != 0) return;
            var initialLots = exp.InitialLots;
            double lot1 = CheckLot(initialLots[0, 1]);
            double lot2 = CheckLot(initialLots[0, 0]);
            var openPrice1 = exp.Connector.SendMarketOrderRequest(exp.Symbol1, exp.Sym1MinOrderType, lot1, exp.SpreadBuyMagicNumber);
            var openPrice2 = exp.Connector.SendMarketOrderRequest(exp.Symbol2, exp.Sym2MinOrderType, lot2, exp.SpreadBuyMagicNumber);

            exp.Sym1LastMinActionPrice = openPrice1;
            exp.Sym2LastMinActionPrice = openPrice2;

            //TODO
            //Order pos1 = OpenBasePosition(Symbol1, Sym1MinOrderType, lot1, spreadBuyMagicNumber);
            //Order pos2 = OpenBasePosition(Symbol2, Sym2MinOrderType, lot2, spreadBuyMagicNumber);
            //List<Order> baseOrders = OrderUtil.GetOrderList(pos1, pos2);
            //Hedge.OnBaseTradesOpened(OrderType.Buy, new[] { lot1, lot2 });
            //if (!(exposureShieldHandler?.EnableOpeningOrder(baseOrders) ?? true)) return;
            //base.SetLastActionPrice(OrderType.Buy, Symbol1, Symbol2);
            //PostOpenTradeSetOperation(Hedge.GetOrdersToOpen(), OrderType.Buy, baseOrders);
        }

        public IEnumerable<Position> GetOpenOrdersList(ExpertSetWrapper exp, string symbol, Sides side, int magicNumber)
        {
            return exp.Connector.Positions.Where(p => p.Value.Symbol == symbol && p.Value.Side == side &&
                                                  p.Value.MagicNumber == magicNumber)
                .Select(p => p.Value);
        }

        public int MyOrdersCount(ExpertSetWrapper exp, string symbol, Sides side)
        {
            return exp.Connector.Positions.Count(p => p.Value.Symbol == symbol && p.Value.Side == side &&
                                                  (p.Value.MagicNumber == exp.SpreadBuyMagicNumber ||
                                                   p.Value.MagicNumber == exp.SpreadSellMagicNumber));
        }

        public double CheckLot(double lot)
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
    }
}

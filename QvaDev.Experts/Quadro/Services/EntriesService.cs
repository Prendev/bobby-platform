using System;
using System.Collections.Generic;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;

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
            //Sym1MaxOrderType = OrderType.Buy;
            //Sym2MaxOrderType = OrderType.Sell;
            //Sym1MinOrderType = OrderType.Sell;
            //Sym2MinOrderType = OrderType.Buy;
            CalculateEntriesForMaxAction(expertSet);
            CalculateEntriesForMinAction(expertSet);
        }

        private void CalculateEntriesForMaxAction(ExpertSetWrapper expertSet)
        {
            var connector = (Mt4Integration.Connector)expertSet.TradingAccount.MetaTraderAccount.Connector;
            if (expertSet.QuantStoAvg <= expertSet.StochMaxAvgOpen || !(expertSet.QuantWprAvg > -expertSet.WprMinAvgOpen)) return;
            int numOfSym1MaxOrders = MyOrdersCount(expertSet, expertSet.Symbol1, Sides.Buy);
            int numOfSym2MaxOrders = MyOrdersCount(expertSet, expertSet.Symbol2, Sides.Sell);
            if (numOfSym1MaxOrders != 0 || numOfSym2MaxOrders != 0) return;
            var initialLots = expertSet.InitialLots;
            double lot1 = CheckLot(initialLots[0, 1]);
            double lot2 = CheckLot(initialLots[0, 0]);
            var openPrice1 = connector.SendMarketOrderRequest(expertSet.Symbol1, Sides.Buy, lot1, expertSet.SpreadSellMagicNumber);
            var openPrice2 = connector.SendMarketOrderRequest(expertSet.Symbol2, Sides.Sell, lot2, expertSet.SpreadSellMagicNumber);

            expertSet.Sym1LastMaxActionPrice = openPrice1;
            expertSet.Sym2LastMaxActionPrice = openPrice2;

            //TODO
            //Order pos1 = OpenBasePosition(Symbol1, Sym1MaxOrderType, lot1, spreadSellMagicNumber);
            //Order pos2 = OpenBasePosition(Symbol2, Sym2MaxOrderType, lot2, spreadSellMagicNumber);
            //List<Order> baseOrders = OrderUtil.GetOrderList(pos1, pos2);
            //Hedge.OnBaseTradesOpened(OrderType.Sell, new[] { lot1, lot2 });
            //if (!(exposureShieldHandler?.EnableOpeningOrder(baseOrders) ?? true)) return;
            //base.SetLastActionPrice(OrderType.Sell, Symbol1, Symbol2);
            //PostOpenTradeSetOperation(Hedge.GetOrdersToOpen(), OrderType.Sell, baseOrders);
        }

        protected void CalculateEntriesForMinAction(ExpertSetWrapper expertSet)
        {
            var connector = (Mt4Integration.Connector)expertSet.TradingAccount.MetaTraderAccount.Connector;
            if (expertSet.QuantStoAvg >= expertSet.StochMinAvgOpen || !(expertSet.QuantWprAvg < -expertSet.WprMaxAvgOpen)) return;
            int numOfSym1MaxOrders = MyOrdersCount(expertSet, expertSet.Symbol1, Sides.Sell);
            int numOfSym2MaxOrders = MyOrdersCount(expertSet, expertSet.Symbol2, Sides.Buy);
            if (numOfSym1MaxOrders != 0 || numOfSym2MaxOrders != 0) return;
            var initialLots = expertSet.InitialLots;
            double lot1 = CheckLot(initialLots[0, 1]);
            double lot2 = CheckLot(initialLots[0, 0]);
            var openPrice1 = connector.SendMarketOrderRequest(expertSet.Symbol1, Sides.Sell, lot1, expertSet.SpreadBuyMagicNumber);
            var openPrice2 = connector.SendMarketOrderRequest(expertSet.Symbol2, Sides.Buy, lot2, expertSet.SpreadBuyMagicNumber);

            expertSet.Sym1LastMinActionPrice = openPrice1;
            expertSet.Sym2LastMinActionPrice = openPrice2;

            //TODO
            //Order pos1 = OpenBasePosition(Symbol1, Sym1MinOrderType, lot1, spreadBuyMagicNumber);
            //Order pos2 = OpenBasePosition(Symbol2, Sym2MinOrderType, lot2, spreadBuyMagicNumber);
            //List<Order> baseOrders = OrderUtil.GetOrderList(pos1, pos2);
            //Hedge.OnBaseTradesOpened(OrderType.Buy, new[] { lot1, lot2 });
            //if (!(exposureShieldHandler?.EnableOpeningOrder(baseOrders) ?? true)) return;
            //base.SetLastActionPrice(OrderType.Buy, Symbol1, Symbol2);
            //PostOpenTradeSetOperation(Hedge.GetOrdersToOpen(), OrderType.Buy, baseOrders);
        }

        public IEnumerable<Position> GetOpenOrdersList(ExpertSetWrapper expertSet, string symbol, Sides side, int magicNumber)
        {
            var connector = expertSet.TradingAccount.MetaTraderAccount.Connector;
            return connector.Positions.Where(p => p.Value.Symbol == symbol && p.Value.Side == side &&
                                                  p.Value.MagicNumber == magicNumber)
                .Select(p => p.Value);
        }

        public int MyOrdersCount(ExpertSetWrapper expertSet, string symbol, Sides side)
        {
            var connector = expertSet.TradingAccount.MetaTraderAccount.Connector;
            return connector.Positions.Count(p => p.Value.Symbol == symbol && p.Value.Side == side &&
                                                  (p.Value.MagicNumber == expertSet.SpreadBuyMagicNumber ||
                                                   p.Value.MagicNumber == expertSet.SpreadSellMagicNumber));
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

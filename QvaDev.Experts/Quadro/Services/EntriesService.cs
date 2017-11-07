using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Indexed;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Experts.Quadro.Hedge;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Services
{
    public interface IEntriesService
    {
        void CalculateEntries(ExpertSetWrapper exp);
    }
    public class EntriesService : IEntriesService
    {
        private readonly IIndex<ExpertSet.HedgeModes, IHedgeService> _hedgeServices;

        public EntriesService(IIndex<ExpertSet.HedgeModes, IHedgeService> hedgeServices)
        {
            _hedgeServices = hedgeServices;
        }

        public void CalculateEntries(ExpertSetWrapper exp)
        {
            if (!exp.QuantStoAvg.HasValue || !exp.QuantWprAvg.HasValue) return;
            CalculateEntriesForMaxAction(exp);
            CalculateEntriesForMinAction(exp);
        }

        private void CalculateEntriesForMaxAction(ExpertSetWrapper exp)
        {
            if (exp.QuantStoAvg <= exp.StochMaxAvgOpen || exp.QuantWprAvg <= -exp.WprMinAvgOpen) return;
            if (MyOrdersCount(exp, exp.Symbol1, exp.Sym1MaxOrderType) != 0) return;
            if (MyOrdersCount(exp, exp.Symbol2, exp.Sym2MaxOrderType) != 0) return;
            var initialLots = exp.InitialLots;
            double lot1 = initialLots[0, 1].CheckLot();
            double lot2 = initialLots[0, 0].CheckLot();

            //TODO
            //if (!(exposureShieldHandler?.EnableOpeningOrder(baseOrders) ?? true)) return;

            exp.Sym1LastMaxActionPrice = exp.Connector.SendMarketOrderRequest(exp.Symbol1, exp.Sym1MaxOrderType, lot1, exp.SpreadSellMagicNumber);
            exp.Sym2LastMaxActionPrice = exp.Connector.SendMarketOrderRequest(exp.Symbol2, exp.Sym2MaxOrderType, lot2, exp.SpreadSellMagicNumber);
            _hedgeServices[exp.HedgeMode].OnBaseTradesOpened(exp, Sides.Sell, new[] { lot1, lot2 });
        }

        protected void CalculateEntriesForMinAction(ExpertSetWrapper exp)
        {
            if (exp.QuantStoAvg >= exp.StochMinAvgOpen || exp.QuantWprAvg >= -exp.WprMaxAvgOpen) return;
            if (MyOrdersCount(exp, exp.Symbol1, exp.Sym1MinOrderType) != 0) return;
            if (MyOrdersCount(exp, exp.Symbol2, exp.Sym2MinOrderType) != 0) return;
            var initialLots = exp.InitialLots;
            double lot1 = initialLots[0, 1].CheckLot();
            double lot2 = initialLots[0, 0].CheckLot();

            //TODO
            //if (!(exposureShieldHandler?.EnableOpeningOrder(baseOrders) ?? true)) return;

            exp.Sym1LastMinActionPrice = exp.Connector.SendMarketOrderRequest(exp.Symbol1, exp.Sym1MinOrderType, lot1, exp.SpreadBuyMagicNumber);
            exp.Sym2LastMinActionPrice = exp.Connector.SendMarketOrderRequest(exp.Symbol2, exp.Sym2MinOrderType, lot2, exp.SpreadBuyMagicNumber);
            _hedgeServices[exp.HedgeMode].OnBaseTradesOpened(exp, Sides.Buy, new[] {lot1, lot2});
        }

        public IEnumerable<Position> GetOpenOrdersList(ExpertSetWrapper exp, string symbol, Sides side, int magicNumber)
        {
            return exp.Connector.Positions.Where(p => p.Value.Symbol == symbol && p.Value.Side == side &&
                                                  p.Value.MagicNumber == magicNumber)
                .Select(p => p.Value);
        }

        private int MyOrdersCount(ExpertSetWrapper exp, string symbol, Sides side)
        {
            return exp.Connector.Positions.Count(p => p.Value.Symbol == symbol && p.Value.Side == side &&
                                                  (p.Value.MagicNumber == exp.SpreadBuyMagicNumber ||
                                                   p.Value.MagicNumber == exp.SpreadSellMagicNumber));
        }
    }
}

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
        private readonly ICommonService _commonService;
        private readonly IIndex<ExpertSet.HedgeModes, IHedgeService> _hedgeServices;

        public EntriesService(
            ICommonService commonService,
            IIndex<ExpertSet.HedgeModes, IHedgeService> hedgeServices)
        {
            _commonService = commonService;
            _hedgeServices = hedgeServices;
        }

        public void CalculateEntries(ExpertSetWrapper exp)
        {
            if (!exp.QuantStoAvg.HasValue) return;
            if (!exp.QuantWprAvg.HasValue) return;

            CalculateEntriesForMaxAction(exp);
            CalculateEntriesForMinAction(exp);
        }

        private void CalculateEntriesForMaxAction(ExpertSetWrapper exp)
        {
            if (exp.QuantStoAvg <= exp.StochMaxAvgOpen) return;
            if (exp.QuantWprAvg <= -exp.WprMinAvgOpen) return;
            if (MyOrdersCount(exp, exp.E.Symbol1, exp.Sym1MaxOrderType) != 0) return;
            if (MyOrdersCount(exp, exp.E.Symbol2, exp.Sym2MaxOrderType) != 0) return;

            double lot1 = exp.SellLots[0, 1].CheckLot();
            double lot2 = exp.SellLots[0, 0].CheckLot();

            //TODO
            //if (!(exposureShieldHandler?.EnableOpeningOrder(baseOrders) ?? true)) return;

            _commonService.SetLastActionPrice(exp, Sides.Sell);
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1MaxOrderType, lot1, exp.SpreadSellMagicNumber);
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2MaxOrderType, lot2, exp.SpreadSellMagicNumber);
            _hedgeServices[exp.E.HedgeMode].OnBaseTradesOpened(exp, Sides.Sell, new[] { lot1, lot2 });
        }

        protected void CalculateEntriesForMinAction(ExpertSetWrapper exp)
        {
            if (exp.QuantStoAvg >= exp.StochMinAvgOpen) return;
            if (exp.QuantWprAvg >= -exp.WprMaxAvgOpen) return;
            if (MyOrdersCount(exp, exp.E.Symbol1, exp.Sym1MinOrderType) != 0) return;
            if (MyOrdersCount(exp, exp.E.Symbol2, exp.Sym2MinOrderType) != 0) return;

            double lot1 = exp.BuyLots[0, 1].CheckLot();
            double lot2 = exp.BuyLots[0, 0].CheckLot();

            //TODO
            //if (!(exposureShieldHandler?.EnableOpeningOrder(baseOrders) ?? true)) return;

            _commonService.SetLastActionPrice(exp, Sides.Buy);
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1MinOrderType, lot1, exp.SpreadBuyMagicNumber);
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2MinOrderType, lot2, exp.SpreadBuyMagicNumber);
            _hedgeServices[exp.E.HedgeMode].OnBaseTradesOpened(exp, Sides.Buy, new[] {lot1, lot2});
        }

        private int MyOrdersCount(ExpertSetWrapper exp, string symbol, Sides side)
        {
            return exp.Connector.Positions.Count(p => p.Value.Symbol == symbol && p.Value.Side == side &&
                                                  (p.Value.MagicNumber == exp.SpreadBuyMagicNumber ||
                                                   p.Value.MagicNumber == exp.SpreadSellMagicNumber));
        }
    }
}

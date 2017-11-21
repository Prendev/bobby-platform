using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
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
        private readonly ILog _log;

        public EntriesService(
            ILog log,
            ICommonService commonService)
        {
            _log = log;
            _commonService = commonService;
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
            if (MyOrdersCount(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType) != 0)
            {
                if (exp.E.CurrentSellState == ExpertSet.TradeSetStates.NoTrade)
                    exp.E.CurrentSellState = ExpertSet.TradeSetStates.TradeOpened;
                return;
            }
            _log.Info($"{exp.E.Description}: EntriesService.CalculateEntriesForMaxAction => {exp.E.MagicNumber}");

            double lot1 = exp.SellLots[0, 1].CheckLot();
            double lot2 = exp.SellLots[0, 0].CheckLot();

            exp.E.SellOpenCount++;
            _commonService.SetLastActionPrice(exp, Sides.Sell);
            exp.E.CurrentSellState = ExpertSet.TradeSetStates.TradeOpened;
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1MaxOrderType, lot1, exp.E.MagicNumber, $"{exp.E.Description} {exp.E.MagicNumber}");
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2MaxOrderType, lot2, exp.E.MagicNumber, $"{exp.E.Description} {exp.E.MagicNumber}");
        }

        protected void CalculateEntriesForMinAction(ExpertSetWrapper exp)
        {
            if (exp.QuantStoAvg >= exp.StochMinAvgOpen || exp.QuantWprAvg >= -exp.WprMaxAvgOpen) return;
            if (MyOrdersCount(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType) != 0)
            {
                if (exp.E.CurrentBuyState == ExpertSet.TradeSetStates.NoTrade)
                    exp.E.CurrentBuyState = ExpertSet.TradeSetStates.TradeOpened;
                return;
            }
            _log.Info($"{exp.E.Description}: EntriesService.CalculateEntriesForMinAction => {exp.E.MagicNumber}");

            double lot1 = exp.BuyLots[0, 1].CheckLot();
            double lot2 = exp.BuyLots[0, 0].CheckLot();

            exp.E.BuyOpenCount++;
            _commonService.SetLastActionPrice(exp, Sides.Buy);
            exp.E.CurrentBuyState = ExpertSet.TradeSetStates.TradeOpened;
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1MinOrderType, lot1, exp.E.MagicNumber, $"{exp.E.Description} {exp.E.MagicNumber}");
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2MinOrderType, lot2, exp.E.MagicNumber, $"{exp.E.Description} {exp.E.MagicNumber}");
        }

        private int MyOrdersCount(ExpertSetWrapper exp, Sides side1, Sides side2)
        {
            var ordersCount = exp.OpenPositions
                .Where(p => p.MagicNumber == exp.E.MagicNumber)
                .Count(p => p.Symbol == exp.E.Symbol1 && p.Side == side1 ||
                            p.Symbol == exp.E.Symbol2 && p.Side == side2);
            return ordersCount;
        }
    }
}

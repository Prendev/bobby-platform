using System;
using System.Linq;
using Autofac.Features.Indexed;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Experts.Quadro.Hedge;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Services
{
    public interface IReentriesService
    {
        void CalculateReentries(ExpertSetWrapper expertSet);
    }

    public class ReentriesService : IReentriesService
    {
        private readonly ICommonService _commonService;
        private readonly IIndex<ExpertSet.HedgeModes, IHedgeService> _hedgeServices;
        private readonly ILog _log;

        public ReentriesService(
            ILog log,
            ICommonService commonService,
            IIndex<ExpertSet.HedgeModes, IHedgeService> hedgeServices)
        {
            _log = log;
            _commonService = commonService;
            _hedgeServices = hedgeServices;
        }

        public void CalculateReentries(ExpertSetWrapper exp)
        {
            CalculateReentriesForSell(exp);
            CalculateReentriesForBuy(exp);
        }

        private void CalculateReentriesForSell(ExpertSetWrapper exp)
        {
            if (_commonService.IsInDeltaRange(exp, Sides.Sell)) return;
            if (exp.QuantStoAvg < exp.StochMaxAvgOpen || exp.QuantWprAvg <= -exp.WprMinAvgOpen) return;
            if (exp.SellOpenCount >= exp.E.MaxTradeSetCount || !EnableLast24Filter(exp, Sides.Sell, 2)) return;

            var o1 = LastOrder(exp, exp.E.Symbol1, exp.Sym1MaxOrderType, exp.SpreadSellMagicNumber);
            var o2 = LastOrder(exp, exp.E.Symbol2, exp.Sym2MaxOrderType, exp.SpreadSellMagicNumber);
            if (o1 == null || o2 == null) return;

            int buyReopenDiff = GetReopenDiff(exp, exp.BuyOpenCount);
            if (exp.Quants.Last() < _commonService.BarQuant(exp, o1) + buyReopenDiff * exp.Point) return;
            if (exp.Quants.Last() < _commonService.BarQuant(exp, o2) + buyReopenDiff * exp.Point) return;
            _log.Debug($"{exp.E.Description}: CalculateReentriesForSell => {exp.SpreadSellMagicNumber}");

            CorrectLotArrayIfNeeded(exp, Sides.Sell);
            double lot1 = exp.SellLots[exp.SellOpenCount, 1].CheckLot();
            double lot2 = exp.SellLots[exp.SellOpenCount, 0].CheckLot();

            exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1MaxOrderType, lot1, exp.SpreadSellMagicNumber);
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2MaxOrderType, lot2, exp.SpreadSellMagicNumber);
            _commonService.SetLastActionPrice(exp, Sides.Sell);
            _hedgeServices[exp.E.HedgeMode].OnBaseTradesOpened(exp, Sides.Sell, new[] { lot1, lot2 });
        }

        private void CalculateReentriesForBuy(ExpertSetWrapper exp)
        {
            if (_commonService.IsInDeltaRange(exp, Sides.Buy)) return;
            if (exp.QuantStoAvg > exp.StochMinAvgOpen || exp.QuantWprAvg >= -exp.WprMaxAvgOpen) return;
            if (exp.BuyOpenCount >= exp.E.MaxTradeSetCount || !EnableLast24Filter(exp, Sides.Buy, 2)) return;

            var o1 = LastOrder(exp, exp.E.Symbol1, exp.Sym1MinOrderType, exp.SpreadBuyMagicNumber);
            var o2 = LastOrder(exp, exp.E.Symbol2, exp.Sym2MinOrderType, exp.SpreadBuyMagicNumber);
            if (o1 == null || o2 == null) return;

            int sellReopenDiff = GetReopenDiff(exp, exp.SellOpenCount);
            if (exp.Quants.Last() > _commonService.BarQuant(exp, o1) - sellReopenDiff * exp.Point) return;
            if (exp.Quants.Last() > _commonService.BarQuant(exp, o2) - sellReopenDiff * exp.Point) return;
            _log.Debug($"{exp.E.Description}: CalculateReentriesForBuy => {exp.SpreadBuyMagicNumber}");

            CorrectLotArrayIfNeeded(exp, Sides.Buy);
            double lot1 = exp.BuyLots[exp.BuyOpenCount, 1].CheckLot();
            double lot2 = exp.BuyLots[exp.BuyOpenCount, 0].CheckLot();

            exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1MinOrderType, lot1, exp.SpreadBuyMagicNumber);
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2MinOrderType, lot2, exp.SpreadBuyMagicNumber);
            _commonService.SetLastActionPrice(exp, Sides.Buy);
            _hedgeServices[exp.E.HedgeMode].OnBaseTradesOpened(exp, Sides.Buy, new[] { lot1, lot2 });
        }

        private bool EnableLast24Filter(ExpertSetWrapper exp, Sides spreadOrderType, int numOfTradePerOpen)
        {
            return _commonService.GetBaseOpenOrdersList(exp, spreadOrderType)
                       .Where(o => (exp.BarHistory1.Last().OpenTime - o.OpenTime).TotalHours >= 24)
                       .ToList().Count < numOfTradePerOpen * exp.E.Last24HMaxOpen;
        }

        private Position LastOrder(ExpertSetWrapper exp, string symbol, Sides orderType, int magicNumber)
        {
            return _commonService.GetOpenOrdersList(exp, symbol, orderType, magicNumber).OrderByDescending(p => p.OpenTime).FirstOrDefault();
        }

        private int GetReopenDiff(ExpertSetWrapper exp, int openCount)
        {
            return openCount >= exp.E.ReOpenDiffChangeCount ? exp.E.ReOpenDiff2 : exp.E.ReOpenDiff;
        }

        private void CorrectLotArrayIfNeeded(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            Sides sym1OrderType;
            double[,] lotArray;
            int magicNumber;
            if (spreadOrderType == Sides.Buy)
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
            var firstOrder = FirstOrder(exp, exp.E.Symbol1, sym1OrderType, magicNumber);
            
            if (firstOrder != null && Math.Abs(firstOrder.Lots - firstLot) >= 1E-05)
                MultiplyLotArray(lotArray, firstOrder.Lots / firstLot);
        }

        private Position FirstOrder(ExpertSetWrapper exp, string symbol, Sides side, int magicNumber)
        {
            return exp.Connector.Positions
                .Where(p => p.Value.Symbol == symbol && p.Value.Side == side && p.Value.MagicNumber == magicNumber)
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

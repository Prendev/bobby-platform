﻿using System;
using System.Linq;
using log4net;
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
        private readonly ICommonService _commonService;
        private readonly ILog _log;

        public ReentriesService(
            ILog log,
            ICommonService commonService)
        {
            _log = log;
            _commonService = commonService;
        }

        public void CalculateReentries(ExpertSetWrapper exp)
        {
            if (!exp.QuantStoAvg.HasValue ||  !exp.QuantWprAvg.HasValue) return;
            CalculateReentriesForMaxAction(exp);
            CalculateReentriesForMinAction(exp);
        }

        private void CalculateReentriesForMaxAction(ExpertSetWrapper exp)
        {
            if (_commonService.IsInDeltaRange(exp, Sides.Sell)) return;
            if (exp.QuantStoAvg < exp.StochMaxAvgOpen || exp.QuantWprAvg <= -exp.WprMinAvgOpen) return;
            if (exp.E.SellOpenCount >= exp.E.MaxTradeSetCount || !EnableLast24Filter(exp, Sides.Sell, 2)) return;
            var o1 = LastOrder(exp, exp.E.Symbol1, exp.Sym1MaxOrderType);
            var o2 = LastOrder(exp, exp.E.Symbol2, exp.Sym2MaxOrderType);
            if (o1 == null || o2 == null) return;

            int reopenDiff = GetReopenDiff(exp, exp.E.BuyOpenCount);
            if (exp.LatestBarQuant.Quant < _commonService.BarQuant(exp, o1) + reopenDiff * exp.Point) return;
            if (exp.LatestBarQuant.Quant < _commonService.BarQuant(exp, o2) + reopenDiff * exp.Point) return;
            _log.Debug($"{exp.E.Description}: ReentriesService.CalculateReentriesForMaxAction => {exp.E.MagicNumber}");

            CorrectLotArrayIfNeeded(exp, Sides.Sell);
            double lot1 = exp.SellLots[exp.E.SellOpenCount, 1].CheckLot();
            double lot2 = exp.SellLots[exp.E.SellOpenCount, 0].CheckLot();

            exp.E.SellOpenCount++;
            _commonService.SetLastActionPrice(exp, Sides.Sell);
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1MaxOrderType, lot1, exp.E.MagicNumber, $"{exp.E.Description} {exp.E.MagicNumber}");
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2MaxOrderType, lot2, exp.E.MagicNumber, $"{exp.E.Description} {exp.E.MagicNumber}");
        }

        private void CalculateReentriesForMinAction(ExpertSetWrapper exp)
        {
            if (_commonService.IsInDeltaRange(exp, Sides.Buy)) return;
            if (exp.QuantStoAvg > exp.StochMinAvgOpen || exp.QuantWprAvg >= -exp.WprMaxAvgOpen) return;
            if (exp.E.BuyOpenCount >= exp.E.MaxTradeSetCount || !EnableLast24Filter(exp, Sides.Buy, 2)) return;
            var o1 = LastOrder(exp, exp.E.Symbol1, exp.Sym1MinOrderType);
            var o2 = LastOrder(exp, exp.E.Symbol2, exp.Sym2MinOrderType);
            if (o1 == null || o2 == null) return;

            int reopenDiff = GetReopenDiff(exp, exp.E.SellOpenCount);
            if (exp.LatestBarQuant.Quant > _commonService.BarQuant(exp, o1) - reopenDiff * exp.Point) return;
            if (exp.LatestBarQuant.Quant > _commonService.BarQuant(exp, o2) - reopenDiff * exp.Point) return;
            _log.Debug($"{exp.E.Description}: ReentriesService.CalculateReentriesForMinAction => {exp.E.MagicNumber}");

            CorrectLotArrayIfNeeded(exp, Sides.Buy);
            double lot1 = exp.BuyLots[exp.E.BuyOpenCount, 1].CheckLot();
            double lot2 = exp.BuyLots[exp.E.BuyOpenCount, 0].CheckLot();

            exp.E.BuyOpenCount++;
            _commonService.SetLastActionPrice(exp, Sides.Buy);
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1MinOrderType, lot1, exp.E.MagicNumber, $"{exp.E.Description} {exp.E.MagicNumber}");
            exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2MinOrderType, lot2, exp.E.MagicNumber, $"{exp.E.Description} {exp.E.MagicNumber}");
        }

        private bool EnableLast24Filter(ExpertSetWrapper exp, Sides spreadOrderType, int numOfTradePerOpen)
        {
            return _commonService.GetBaseOpenOrdersList(exp, spreadOrderType)
                       .Where(o => (exp.LatestBarQuant.OpenTime.AddMinutes((int)exp.E.TimeFrame) - o.OpenTime).TotalHours <= 24)
                       .ToList().Count < numOfTradePerOpen * exp.E.Last24HMaxOpen;
        }

        private Position LastOrder(ExpertSetWrapper exp, string symbol, Sides orderType)
        {
            return _commonService.GetOpenOrdersList(exp, symbol, orderType, exp.E.MagicNumber).OrderByDescending(p => p.OpenTime).FirstOrDefault();
        }

        private int GetReopenDiff(ExpertSetWrapper exp, int openCount)
        {
            return openCount >= exp.E.ReOpenDiffChangeCount ? exp.E.ReOpenDiff2 : exp.E.ReOpenDiff;
        }

        private void CorrectLotArrayIfNeeded(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            Sides sym1OrderType;
            double[,] lotArray;
            if (spreadOrderType == Sides.Buy)
            {
                sym1OrderType = exp.Sym1MinOrderType;
                lotArray = exp.BuyLots;
            }
            else
            {
                sym1OrderType = exp.Sym1MaxOrderType;
                lotArray = exp.SellLots;
            }

            double firstLot = lotArray[0, 1];
            var firstOrder = FirstOrder(exp, exp.E.Symbol1, sym1OrderType, exp.E.MagicNumber);
            
            if (firstOrder != null && Math.Abs(firstOrder.Lots - firstLot) >= 1E-05)
                MultiplyLotArray(lotArray, firstOrder.Lots / firstLot);
        }

        private Position FirstOrder(ExpertSetWrapper exp, string symbol, Sides side, int magicNumber)
        {
            return exp.OpenPositions
                .Where(p => p.Symbol == symbol && p.Side == side && p.MagicNumber == magicNumber)
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

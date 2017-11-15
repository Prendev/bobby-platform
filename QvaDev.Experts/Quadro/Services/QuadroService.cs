using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Experts.Quadro.Models;
using QvaDev.Mt4Integration;

namespace QvaDev.Experts.Quadro.Services
{
    public interface IQuadroService
    {
        void Stop();
        void OnBarHistory(Connector connector, ExpertSet expertSet, BarHistoryEventArgs e);
        void OnTick(Connector connector, ExpertSet expertSet);
    }
    public class QuadroService : IQuadroService
    {
        private static readonly ConcurrentDictionary<int, ExpertSetWrapper> ExpertSetWrappers =
            new ConcurrentDictionary<int, ExpertSetWrapper>();

        private readonly ICloseService _closeService;
        private readonly IEntriesService _entriesService;
        private readonly IReentriesService _reentriesService;
        private readonly ILog _log;

        public QuadroService(
            ICloseService closeService,
            IEntriesService entriesService,
            IReentriesService reentriesService,
            ILog log)
        {
            _log = log;
            _reentriesService = reentriesService;
            _entriesService = entriesService;
            _closeService = closeService;
        }

        public void Stop()
        {
            ExpertSetWrappers.Clear();
        }

        public void OnTick(Connector connector, ExpertSet expertSet)
        {
            if (expertSet.ExpertDenied) return;
            var exp = ExpertSetWrappers.GetOrAdd(expertSet.Id, id => new ExpertSetWrapper(expertSet));
            try
            {
                lock (exp)
                {
                    if (exp.E.CloseAllBuy)
                    {
                        _closeService.AllCloseMin(exp);
                        exp.E.CloseAllBuy = false;
                    }
                    else if (exp.E.BisectingCloseBuy)
                    {
                        _closeService.BisectingCloseMin(exp);
                        exp.E.BisectingCloseBuy = false;
                    }
                    if (exp.E.CloseAllSell)
                    {
                        _closeService.AllCloseMax(exp);
                        exp.E.CloseAllSell = false;
                    }
                    else if (exp.E.BisectingCloseSell)
                    {
                        _closeService.BisectingCloseMax(exp);
                        exp.E.BisectingCloseSell = false;
                    }
                    if (exp.E.ProfitCloseBuy)
                    {
                        _closeService.CheckProfitClose(exp, Sides.Buy);
                    }
                    if (exp.E.ProfitCloseSell)
                    {
                        _closeService.CheckProfitClose(exp, Sides.Sell);
                    }
                    if (exp.E.UseTradeSetStopLoss)
                    {
                        if (GetSumProfit(exp) < exp.E.TradeSetStopLossValue)
                        {
                            _closeService.AllCloseMin(exp);
                            _closeService.AllCloseMax(exp);
                            exp.E.TradeOpeningEnabled = false;
                        }
                    }
                    if (connector.GetFloatingProfit() < exp.E.TradeSetFloatingSwitch)
                    {
                        exp.E.TradeOpeningEnabled = false;
                        _log.Debug(
                            $"{exp.E.Description}: TradeOpeningEnabled set to FALSE because of TradeSetFloatingSwitch");
                    }
                }
            }
            catch (BarNotFoundException ex)
            {
                _log.Debug($"{exp.E.Description}: ExpertDenied");
                exp.E.ExpertDenied = true;
                if (exp.OpenPositions.Any())
                {
                    _closeService.AllCloseMin(exp);
                    _closeService.AllCloseMax(exp);
                }
            }
        }

        public void OnBarHistory(Connector connector, ExpertSet expertSet, BarHistoryEventArgs e)
        {
            if (expertSet.ExpertDenied) return;
            var exp = ExpertSetWrappers.GetOrAdd(expertSet.Id, id => new ExpertSetWrapper(expertSet));
            try
            {
                lock (exp)
                {
                    PreCheckOrders(exp);
                    if (!IsBarUpdating(exp, e)) return;
                    if (!AreBarsInSynchron(exp)) return;
                    exp.CalculateQuants();
                    _log.Debug(
                        $"{exp.E.Description}: quants ({exp.E.M}) => {exp.Quant:F} | stoch avg ({exp.E.StochMultiplication}) => {exp.QuantStoAvg:F}");
                    OnBar(exp);
                }
            }
            catch (BarNotFoundException ex)
            {
                _log.Debug($"{exp.E.Description}: ExpertDenied");
                exp.E.ExpertDenied = true;
                if (exp.OpenPositions.Any())
                {
                    _closeService.AllCloseMin(exp);
                    _closeService.AllCloseMax(exp);
                }
            }
        }

        private void PreCheckOrders(ExpertSetWrapper exp)
        {
            if (exp.OpenPositions.All(p => p.MagicNumber != exp.SpreadBuyMagicNumber))
            {
                exp.E.BuyOpenCount = 0;
                exp.E.CurrentBuyState = ExpertSet.TradeSetStates.NoTrade;
                exp.E.Sym1LastMinActionPrice = 0;
                exp.E.Sym2LastMinActionPrice = 0;
            }
            if (exp.OpenPositions.All(p => p.MagicNumber != exp.SpreadSellMagicNumber))
            {
                exp.E.SellOpenCount = 0;
                exp.E.CurrentSellState = ExpertSet.TradeSetStates.NoTrade;
                exp.E.Sym1LastMaxActionPrice = 0;
                exp.E.Sym2LastMaxActionPrice = 0;
            }
        }

        private bool IsBarUpdating(ExpertSetWrapper exp, BarHistoryEventArgs e)
        {
            if (e.BarHistory.First().OpenTime <= DateTime.UtcNow.AddMinutes(-2 * (int)exp.E.TimeFrame)) return false;
            if (e.BarHistory.Count < exp.E.GetMaxBarCount()) return false;

            var barHistory = exp.E.Symbol1 == e.Symbol ? exp.BarHistory1 : exp.BarHistory2;
            if (barHistory.Any() && e.BarHistory.First().OpenTime <= barHistory.First().OpenTime) return false;

            if (exp.E.Symbol1 == e.Symbol) exp.BarHistory1 = new List<Bar>(e.BarHistory);
            else exp.BarHistory2 = new List<Bar>(e.BarHistory);
            return true;
        }

        private bool AreBarsInSynchron(ExpertSetWrapper exp)
        {
            if (!exp.BarHistory1.Any() || !exp.BarHistory2.Any()) return false;
            if (exp.BarHistory1.First().OpenTime != exp.BarHistory2.First().OpenTime) return false;
            return true;
        }

        private void OnBar(ExpertSetWrapper exp)
        {
            if (!IsCurrentTimeEnabledForTrade()) return;

            _closeService.CheckClose(exp);
            _reentriesService.CalculateReentries(exp);

            if (!exp.E.TradeOpeningEnabled) return;
            _entriesService.CalculateEntries(exp);
        }

        private bool IsCurrentTimeEnabledForTrade()
        {
            var utcNow = DateTime.UtcNow;
            //if (utcNow.DayOfWeek == DayOfWeek.Friday && utcNow.Hour >= 20) return false;
            if (utcNow.DayOfWeek == DayOfWeek.Saturday) return false;
            if (utcNow.DayOfWeek == DayOfWeek.Sunday && (utcNow.Hour < 23 || utcNow.Minute < 59)) return false;
            return true;
        }

        private double GetSumProfit(ExpertSetWrapper exp)
        {
            return exp.Connector.CalculateProfit(exp.E.Symbol1, exp.E.Symbol2,
                exp.SpreadBuyMagicNumber, exp.SpreadSellMagicNumber,
                exp.HedgeBuyMagicNumber, exp.HedgeSellMagicNumber);
        }
    }
}

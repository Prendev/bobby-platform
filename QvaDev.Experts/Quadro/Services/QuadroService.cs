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
            var exp = ExpertSetWrappers.GetOrAdd(expertSet.Id, id => new ExpertSetWrapper(expertSet));
            //if (exp.BarMissingOpenTime.HasValue) return;
            try
            {
                lock (exp)
                {

                    if (exp.E.UseTradeSetStopLoss && GetSumProfit(exp) < exp.E.TradeSetStopLossValue)
                    {
                        _closeService.AllCloseMin(exp);
                        _closeService.AllCloseMax(exp);
                        exp.E.TradeOpeningEnabled = false;
                        return;
                    }

                    if (exp.E.SyncBuyState)
                    {
                        var sym1Count = exp.OpenPositions.Count(p => p.MagicNumber == exp.E.MagicNumber && p.Symbol == exp.E.Symbol1 && p.Side == exp.Sym1MinOrderType);
                        var sym2Count = exp.OpenPositions.Count(p => p.MagicNumber == exp.E.MagicNumber && p.Symbol == exp.E.Symbol2 && p.Side == exp.Sym2MinOrderType);
                        exp.E.BuyOpenCount = Math.Max(sym1Count, sym2Count);
                        exp.E.Sym1LastMinActionPrice = exp.Connector.GetLastActionPrice(exp.E.Symbol1, exp.Sym1MinOrderType, exp.E.MagicNumber);
                        exp.E.Sym2LastMinActionPrice = exp.Connector.GetLastActionPrice(exp.E.Symbol2, exp.Sym2MinOrderType, exp.E.MagicNumber);
                        if (exp.E.CurrentBuyState == ExpertSet.TradeSetStates.NoTrade && exp.E.BuyOpenCount > 0)
                            exp.E.CurrentBuyState = ExpertSet.TradeSetStates.TradeOpened;
                        exp.E.SyncBuyState = false;
                    }
                    if (exp.E.CloseAllBuy) _closeService.AllCloseMin(exp);
                    else if (exp.E.ProfitCloseBuy) _closeService.CheckProfitClose(exp, Sides.Buy);
                    else if (exp.E.BisectingCloseBuy) _closeService.BisectingCloseMin(exp);

                    if (exp.E.SyncSellState)
                    {
                        var sym1Count = exp.OpenPositions.Count(p => p.MagicNumber == exp.E.MagicNumber && p.Symbol == exp.E.Symbol1 && p.Side == exp.Sym1MaxOrderType);
                        var sym2Count = exp.OpenPositions.Count(p => p.MagicNumber == exp.E.MagicNumber && p.Symbol == exp.E.Symbol2 && p.Side == exp.Sym2MaxOrderType);
                        exp.E.Sym1LastMaxActionPrice = exp.Connector.GetLastActionPrice(exp.E.Symbol1, exp.Sym1MaxOrderType, exp.E.MagicNumber);
                        exp.E.Sym2LastMaxActionPrice = exp.Connector.GetLastActionPrice(exp.E.Symbol2, exp.Sym2MaxOrderType, exp.E.MagicNumber);
                        exp.E.SellOpenCount = Math.Max(sym1Count, sym2Count);
                        if (exp.E.CurrentSellState == ExpertSet.TradeSetStates.NoTrade && exp.E.SellOpenCount > 0)
                            exp.E.CurrentSellState = ExpertSet.TradeSetStates.TradeOpened;
                        exp.E.SyncSellState = false;
                    }
                    if (exp.E.CloseAllSell) _closeService.AllCloseMax(exp);
                    else if (exp.E.ProfitCloseSell) _closeService.CheckProfitClose(exp, Sides.Sell);
                    else if (exp.E.BisectingCloseSell) _closeService.BisectingCloseMax(exp);
                }
            }
            catch (BarMissingException ex)
            {
                exp.BarMissingOpenTime = ex.OpenTime;
                _log.Info($"{exp.E.Description}: ExpertDenied");
            }
        }

        public void OnBarHistory(Connector connector, ExpertSet expertSet, BarHistoryEventArgs e)
        {
            var exp = ExpertSetWrappers.GetOrAdd(expertSet.Id, id => new ExpertSetWrapper(expertSet));
            try
            {
                lock (exp)
                {
                    if (!CheckBarUpdate(exp, e)) return;
                    _log.Debug($"{exp.E.Description}: quants ({exp.E.M}) => {exp.LatestBarQuant.Quant:F5} " +
                               $"| stoch avg ({exp.E.StochMultiplication}) => {exp.QuantStoAvg:F5}");
                    PreCheckOrders(exp);
                    OnBar(exp);
                }
            }
            catch (BarMissingException ex)
            {
                exp.BarMissingOpenTime = ex.OpenTime;
                exp.GetSpecificBars(ex.OpenTime);
                _log.Info($"{exp.E.Description}: BarMissing => {ex.OpenTime}");
                //if (exp.OpenPositions.Any())
                //{
                //    _closeService.AllCloseMin(exp);
                //    _closeService.AllCloseMax(exp);
                //}
            }
        }

        private void PreCheckOrders(ExpertSetWrapper exp)
        {
            if (!exp.OpenPositions.Any(p => p.MagicNumber == exp.E.MagicNumber &&
                                            (p.Symbol == exp.E.Symbol1 && p.Side == exp.Sym1MinOrderType ||
                                             p.Symbol == exp.E.Symbol2 && p.Side == exp.Sym2MinOrderType)))
            {
                exp.E.BuyOpenCount = 0;
                exp.E.CurrentBuyState = ExpertSet.TradeSetStates.NoTrade;
                //exp.E.Sym1LastMinActionPrice = 0;
                //exp.E.Sym2LastMinActionPrice = 0;
            }

            if (!exp.OpenPositions.Any(p => p.MagicNumber == exp.E.MagicNumber &&
                                            (p.Symbol == exp.E.Symbol1 && p.Side == exp.Sym1MaxOrderType ||
                                             p.Symbol == exp.E.Symbol2 && p.Side == exp.Sym2MaxOrderType)))
            {
                exp.E.SellOpenCount = 0;
                exp.E.CurrentSellState = ExpertSet.TradeSetStates.NoTrade;
                //exp.E.Sym1LastMaxActionPrice = 0;
                //exp.E.Sym2LastMaxActionPrice = 0;
            }
        }

        private bool CheckBarUpdate(ExpertSetWrapper exp, BarHistoryEventArgs e)
        {
            var prevCount = exp.BarQuants.Count(b => b.Value.Bar1 != null && b.Value.Bar2 != null);
            exp.UpdateBarQuants(e.Symbol, e.BarHistory);
            var newCount = exp.BarQuants.Count(b => b.Value.Quant.HasValue);
            if (newCount < exp.E.GetMaxBarCount()) return false;
            if (prevCount == newCount) return false;
            if (exp.LatestBarQuant.OpenTime <= DateTime.UtcNow.AddMinutes(-2 * (int) exp.E.TimeFrame)) return false;
            if (!exp.BarMissingOpenTime.HasValue) return exp.LastBarOpenTime != exp.LatestBarQuant.OpenTime;
            if (!exp.BarQuants.ContainsKey(exp.BarMissingOpenTime.Value)) return false;
            if (!exp.BarQuants[exp.BarMissingOpenTime.Value].Quant.HasValue) return false;
            _log.Info($"{exp.E.Description}: Bar found => {exp.BarMissingOpenTime.Value}");
            exp.BarMissingOpenTime = null;
            return true;
        }

        private void OnBar(ExpertSetWrapper exp)
        {
            exp.LastBarOpenTime = exp.LatestBarQuant.OpenTime;
            if (!IsCurrentTimeEnabledForTrade()) return;

            _closeService.CheckClose(exp);
            _reentriesService.CalculateReentries(exp);

            if (!exp.E.TradeOpeningEnabled) return;
            _entriesService.CalculateEntries(exp);
        }

        private bool IsCurrentTimeEnabledForTrade()
        {
            var utcNow = DateTime.UtcNow;
            if (utcNow.DayOfWeek == DayOfWeek.Friday && utcNow.Hour >= 20) return false;
            if (utcNow.DayOfWeek == DayOfWeek.Saturday) return false;
            if (utcNow.DayOfWeek == DayOfWeek.Sunday && (utcNow.Hour < 23 || utcNow.Minute < 59)) return false;
            return true;
        }

        private double GetSumProfit(ExpertSetWrapper exp)
        {
            return exp.Connector.CalculateProfit(exp.E.Symbol1, exp.E.Symbol2,
                exp.E.MagicNumber, exp.HedgeMagicNumber);
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Indexed;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Experts.Quadro.Hedge;
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

        private readonly ICommonService _commonService;
        private readonly ICloseService _closeService;
        private readonly IEntriesService _entriesService;
        private readonly IReentriesService _reentriesService;
        private readonly IIndex<ExpertSet.HedgeModes, IHedgeService> _hedgeServices;
        private readonly ILog _log;

        public QuadroService(
            ICommonService commonService,
            ICloseService closeService,
            IEntriesService entriesService,
            IReentriesService reentriesService,
            IIndex<ExpertSet.HedgeModes, IHedgeService> hedgeServices,
            ILog log)
        {
            _commonService = commonService;
            _log = log;
            _hedgeServices = hedgeServices;
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
            lock (exp)
            {
                if (exp.E.CloseAllBuy)
                {
                    _closeService.AllCloseMin(exp);
                    exp.E.CloseAllBuy = false;
                }
                if (exp.E.CloseAllSell)
                {
                    _closeService.AllCloseMax(exp);
                    exp.E.CloseAllSell = false;
                }
                if (exp.E.ProfitCloseBuy)
                {
                    _closeService.CheckProfitClose(exp, Sides.Buy);
                }
                if (exp.E.ProfitCloseSell)
                {
                    _closeService.CheckProfitClose(exp, Sides.Sell);
                }
                if (exp.E.HedgeProfitClose)
                {
                    _hedgeServices[exp.E.HedgeMode].CheckHedgeProfitClose(exp);
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
                    _log.Debug($"{exp.E.Description}: TradeOpeningEnabled set to FALSE because of TradeSetFloatingSwitch");
                }
            }
        }

        public void OnBarHistory(Connector connector, ExpertSet expertSet, BarHistoryEventArgs e)
        {
            var exp = ExpertSetWrappers.GetOrAdd(expertSet.Id, id => new ExpertSetWrapper(expertSet));
            lock (exp)
            {
                if (!IsBarUpdating(exp, e)) return;
                if (!AreBarsInSynchron(exp)) return;
                if (exp.E.ExpertDenied) return;
                exp.CalculateQuants();
                _log.Debug($"{exp.E.Description}: quants ({exp.E.M}) => {exp.Quants.First():F} | stoch avg ({exp.E.StochMultiplication}) => {exp.QuantStoAvg:F}");
                OnBar(exp);
            }
        }

        private bool IsBarUpdating(ExpertSetWrapper exp, BarHistoryEventArgs e)
        {
            if (exp.E.Symbol1 != e.Symbol && exp.E.Symbol2 != e.Symbol) return false;
            if (e.BarHistory.First().OpenTime <= DateTime.UtcNow.AddMinutes(-2 * exp.E.TimeFrame)) return false;
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
            _hedgeServices[exp.E.HedgeMode].CheckHedgeStopByQuant(exp);

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
            return _commonService.CalculateBaseOrdersProfit(exp, Sides.Buy) +
                   _commonService.CalculateBaseOrdersProfit(exp, Sides.Sell) +
                   _hedgeServices[exp.E.HedgeMode].CalculateProfit(exp, Sides.Buy) +
                   _hedgeServices[exp.E.HedgeMode].CalculateProfit(exp, Sides.Sell);
        }
    }
}

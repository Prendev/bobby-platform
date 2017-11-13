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
        void OnTick(Connector connector, ExpertSet expertSet, TickEventArgs e);
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

        public void OnTick(Connector connector, ExpertSet expertSet, TickEventArgs e)
        {
            lock (expertSet)
            {
                var exp = ExpertSetWrappers.GetOrAdd(expertSet.Id, id => new ExpertSetWrapper(expertSet));
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
                    _log.Debug($"Trade opening disabled for {exp.E.Symbol1} | {exp.E.Symbol2}");
                }
            }
        }

        public void OnBarHistory(Connector connector, ExpertSet expertSet, BarHistoryEventArgs e)
        {
            lock (expertSet)
            {
                if (expertSet.Symbol1 != e.Symbol && expertSet.Symbol2 != e.Symbol) return;

                var exp = ExpertSetWrappers.GetOrAdd(expertSet.Id, id => new ExpertSetWrapper(expertSet));
                if (exp.ExpertDenied) return;
                if (!IsBarUpdating(exp, e)) return;
                if (!AreBarsInSynchron(exp)) return;
                var quants = new List<double>();
                for (var i = 0; i < exp.BarHistory1.Count; i++)
                {
                    if (exp.BarHistory1[i].OpenTime != exp.BarHistory2[i].OpenTime) return;
                    var price1Close = connector.MyRoundToDigits(expertSet.Symbol1, exp.BarHistory1[i].Close);
                    var price2Close = connector.MyRoundToDigits(expertSet.Symbol2, exp.BarHistory2[i].Close);
                    var quant = connector.MyRoundToDigits(expertSet.Symbol1, price2Close - expertSet.M * price1Close);
                    quants.Add(quant);
                }
                exp.Quants = quants;
                _log.Debug($"Quants for {exp.E.Symbol1} | {exp.E.Symbol2} => {exp.Quants.AsString()}");
                OnBar(exp);
            }
        }

        private bool IsBarUpdating(ExpertSetWrapper exp, BarHistoryEventArgs e)
        {
            if (exp.E.Symbol1 == e.Symbol)
            {
                if (exp.BarHistory1.Any() && e.BarHistory.Last().OpenTime <= exp.BarHistory1.Last().OpenTime)
                    return false;
                exp.BarHistory1 = e.BarHistory;
            }
            else
            {
                if (exp.BarHistory2.Any() && e.BarHistory.Last().OpenTime <= exp.BarHistory2.Last().OpenTime)
                    return false;
                exp.BarHistory2 = e.BarHistory;
            }
            return true;
        }

        private bool AreBarsInSynchron(ExpertSetWrapper exp)
        {
            if (exp.BarHistory1.Count <= 1 || exp.BarHistory2.Count <= 1) return false;
            if (exp.BarHistory1.Last().OpenTime != exp.BarHistory2.Last().OpenTime) return false;
            if (exp.BarHistory1.First().OpenTime != exp.BarHistory2.First().OpenTime) return false;
            return true;
        }


        private void OnBar(ExpertSetWrapper exp)
        {
            if (exp.BarHistory1.Last().OpenTime <= DateTime.UtcNow.AddMinutes(-2 * exp.E.TimeFrame)) return;
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

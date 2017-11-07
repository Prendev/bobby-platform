using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Experts.Quadro.Hedge;
using QvaDev.Experts.Quadro.Models;
using QvaDev.Mt4Integration;

namespace QvaDev.Experts.Quadro.Services
{
    public interface IQuadroService
    {
        void OnBarHistory(Connector connector, ExpertSet expertSet, BarHistoryEventArgs e);
    }
    public class QuadroService : IQuadroService
    {
        private readonly ICloseService _closeService;
        private readonly IEntriesService _entriesService;
        private readonly IReentriesService _reentriesService;
        private readonly IIndex<ExpertSet.HedgeModes, IHedgeService> _hedgeServices;

        private static readonly ConcurrentDictionary<int, ExpertSetWrapper> ExpertSetWrappers =
            new ConcurrentDictionary<int, ExpertSetWrapper>();

        public QuadroService(
            ICloseService closeService,
            IEntriesService entriesService,
            IReentriesService reentriesService,
            IIndex<ExpertSet.HedgeModes, IHedgeService> hedgeServices)
        {
            _hedgeServices = hedgeServices;
            _reentriesService = reentriesService;
            _entriesService = entriesService;
            _closeService = closeService;
        }

        public void OnBarHistory(Connector connector, ExpertSet expertSet, BarHistoryEventArgs e)
        {
            lock (expertSet)
            {
                var exp = ExpertSetWrappers.GetOrAdd(expertSet.Id, id => new ExpertSetWrapper(expertSet));
                if (!AreBarsInSynchron(exp, e)) return;
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
                OnBar(exp);
            }
        }

        private bool AreBarsInSynchron(ExpertSetWrapper exp, BarHistoryEventArgs e)
        {
            if (e.BarHistory.Last().OpenTime < DateTime.UtcNow.AddHours(-0.5)) return false;
            if (exp.Symbol1 != e.Symbol && exp.Symbol2 != e.Symbol) return false;
            if (exp.Symbol1 == e.Symbol) exp.BarHistory1 = e.BarHistory;
            else exp.BarHistory2 = e.BarHistory;
            if ((exp.BarHistory1?.Count ?? 0) <= 1 || (exp.BarHistory2?.Count ?? 0) <= 1) return false;
            if (exp.BarHistory1?.Last().OpenTime != exp.BarHistory2?.Last().OpenTime) return false;
            if (exp.BarHistory1?.First().OpenTime != exp.BarHistory2?.First().OpenTime) return false;
            return true;
        }


        private void OnBar(ExpertSetWrapper exp)
        {
            bool isCurrentTimeEnabledForTrade = IsCurrentTimeEnabledForTrade();
            if (isCurrentTimeEnabledForTrade)
            {
                _closeService.CheckClose(exp);
                _reentriesService.CalculateReentries(exp);
                CheckHedgeStopByQuant(exp);
            }
            if (!isCurrentTimeEnabledForTrade || !exp.TradeOpeningEnabled) return;
            _entriesService.CalculateEntries(exp);
        }

        private bool IsCurrentTimeEnabledForTrade()
        {
            DateTime lastTickTime = DateTime.UtcNow;
            bool flag = !(IsTodaySundayCorrection(lastTickTime) | (lastTickTime.DayOfWeek == DayOfWeek.Friday && lastTickTime.Hour >= 20));
            return flag;
        }

        private bool IsTodaySundayCorrection(DateTime lastTickTime)
        {
            bool second;
            if (lastTickTime.DayOfWeek != DayOfWeek.Sunday)
            {
                second = false;
            }
            else if (lastTickTime.Hour < 23 || lastTickTime.Minute < 59)
            {
                second = true;
            }
            else
            {
                second = lastTickTime.Second < 0;
            }
            return second;
        }

        private void CheckHedgeStopByQuant(ExpertSetWrapper exp)
        {
            if (exp.HedgeStopPositionCount < 0 || exp.HedgeMode == ExpertSet.HedgeModes.NoHedge) return;
            if (exp.SellHedgeOpenCount > 0)
            {
                CheckHedgeStopByQuant(exp, Sides.Sell, barQuant => exp.Quants.Last() < barQuant);
            }
            if (exp.BuyHedgeOpenCount > 0)
            {
                CheckHedgeStopByQuant(exp, Sides.Buy, barQuant => exp.Quants.Last() > barQuant);
            }
        }

        private void CheckHedgeStopByQuant(ExpertSetWrapper exp, Sides spreadOrderType, Predicate<double> predicate)
        {
            if (!predicate(exp.BarQuant(exp.GetBaseOpenOrdersList(spreadOrderType).Where(o => o.Symbol == exp.Symbol1)
                .OrderBy(o => o.OpenTime).ToList()[exp.HedgeStopPositionCount]))) return;
            _hedgeServices[exp.HedgeMode].OnCloseAll(exp, spreadOrderType);
        }
    }
}

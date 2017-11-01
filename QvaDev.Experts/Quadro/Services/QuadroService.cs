using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
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

        private static readonly ConcurrentDictionary<int, ExpertSetWrapper> ExpertSetWrappers =
            new ConcurrentDictionary<int, ExpertSetWrapper>();

        public QuadroService(
            ICloseService closeService,
            IEntriesService entriesService)
        {
            _entriesService = entriesService;
            _closeService = closeService;
        }

        public void OnBarHistory(Connector connector, ExpertSet expertSet, BarHistoryEventArgs e)
        {
            lock (expertSet)
            {
                var exp = ExpertSetWrappers.GetOrAdd(expertSet.Id, id => new ExpertSetWrapper(expertSet));
                if (!IsBarsInSynchron(exp, e)) return;
                Task.Factory.StartNew(() =>
                {
                    lock (expertSet)
                    {
                        exp.Quants = new List<double>();
                        for (var i = 0; i < exp.BarHistory1.Count; i++)
                        {
                            var price1Close = connector.MyRoundToDigits(expertSet.Symbol1, exp.BarHistory1[i].Close);
                            var price2Close = connector.MyRoundToDigits(expertSet.Symbol2, exp.BarHistory2[i].Close);
                            var quant = connector.MyRoundToDigits(expertSet.Symbol1, price2Close - expertSet.M * price1Close);
                            exp.Quants.Add(quant);
                            OnBar(exp);
                        }
                    }
                });
            }
        }

        private bool IsBarsInSynchron(ExpertSetWrapper exp, BarHistoryEventArgs e)
        {
            if (exp.Symbol1 != e.Symbol && exp.Symbol2 != e.Symbol) return false;
            if (exp.Symbol1 == e.Symbol) exp.BarHistory1 = e.BarHistory;
            else exp.BarHistory2 = e.BarHistory;
            if ((exp.BarHistory1?.Count ?? 0) <= 1 || (exp.BarHistory2?.Count ?? 0) <= 1) return false;
            if (exp.BarHistory1?.Last().OpenTime != exp.BarHistory2?.Last().OpenTime) return false;
            return true;
        }


        private void OnBar(ExpertSetWrapper exp)
        {
            bool isCurrentTimeEnabledForTrade = IsCurrentTimeEnabledForTrade();
            if (isCurrentTimeEnabledForTrade)
            {
                _closeService.CheckClose(exp);
                //CalculateReEntries();
                //CheckHedgeStopByQuant();
            }
            if (isCurrentTimeEnabledForTrade && exp.TradeOpeningEnabled)
            {
                _entriesService.CalculateEntries(exp);
            }
            //CheckOrdersConsistency();
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
    }
}

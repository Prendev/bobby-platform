using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Mt4Integration;

namespace QvaDev.Experts.Quadro.Services
{
    public interface IQuadroService
    {
        void OnBarHistory(Connector connector, ExpertSetWrapper expertSet, BarHistoryEventArgs e);
    }
    public class QuadroService : IQuadroService
    {
        private readonly ICloseService _closeService;
        private readonly IEntriesService _entriesService;

        public QuadroService(
            ICloseService closeService,
            IEntriesService entriesService)
        {
            _entriesService = entriesService;
            _closeService = closeService;
        }

        public void OnBarHistory(Connector connector, ExpertSetWrapper expertSet, BarHistoryEventArgs e)
        {
            lock (expertSet)
            {
                if (!IsBarsInSynchron(expertSet, e)) return;
                Task.Factory.StartNew(() =>
                {
                    lock (expertSet)
                    {
                        expertSet.Quants = new List<double>();
                        for (var i = 0; i < expertSet.BarHistory1.Count; i++)
                        {
                            var price1Close = connector.MyRoundToDigits(expertSet.Symbol1, expertSet.BarHistory1[i].Close);
                            var price2Close = connector.MyRoundToDigits(expertSet.Symbol2, expertSet.BarHistory2[i].Close);
                            var quant = connector.MyRoundToDigits(expertSet.Symbol1, price2Close - expertSet.M * price1Close);
                            expertSet.Quants.Add(quant);
                            OnBar(expertSet);
                        }
                    }
                });
            }
        }

        private bool IsBarsInSynchron(ExpertSetWrapper expertSet, BarHistoryEventArgs e)
        {
            if (expertSet.Symbol1 != e.Symbol && expertSet.Symbol2 != e.Symbol) return false;
            if (expertSet.Symbol1 == e.Symbol) expertSet.BarHistory1 = e.BarHistory;
            else expertSet.BarHistory2 = e.BarHistory;
            if ((expertSet.BarHistory1?.Count ?? 0) <= 1 || (expertSet.BarHistory2?.Count ?? 0) <= 1) return false;
            if (expertSet.BarHistory1?.Last().OpenTime != expertSet.BarHistory2?.Last().OpenTime) return false;
            return true;
        }


        private void OnBar(ExpertSetWrapper expertSet)
        {
            bool isCurrentTimeEnabledForTrade = IsCurrentTimeEnabledForTrade();
            if (isCurrentTimeEnabledForTrade)
            {
                _closeService.CheckClose(expertSet);
                //CalculateReEntries();
                //CheckHedgeStopByQuant();
            }
            if (isCurrentTimeEnabledForTrade && expertSet.TradeOpeningEnabled)
            {
                _entriesService.CalculateEntries(expertSet);
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

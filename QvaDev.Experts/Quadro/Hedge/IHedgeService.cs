using System.Collections.Generic;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Hedge
{
    public interface IHedgeService
    {
        void CheckHedgeStopByQuant(ExpertSetWrapper exp);
        double CalculateProfit(ExpertSetWrapper exp, Sides spreadOrderType);
        void OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType);
        void OnPartialClose(ExpertSetWrapper exp, Sides spreadOrderType, double lotRatio);
        void OnBaseTradesOpened(ExpertSetWrapper exp, Sides spreadOrderType, double[] sourceLots);
    }
}

using System.Collections.Generic;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Hedge
{
    public interface IHedgeService
    {
        double CalculateProfit(ExpertSetWrapper exp, Sides spreadOrderType);
        List<Position> OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType);
        List<Position> OnPartialClose(ExpertSetWrapper exp, Sides spreadOrderType, double lotRatio);
        List<Position> OnBaseTradesOpened(ExpertSetWrapper exp, Sides spreadOrderType, double[] sourceLots);
    }
}

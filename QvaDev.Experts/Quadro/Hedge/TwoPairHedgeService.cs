using System.Collections.Generic;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Hedge
{
    public class TwoPairHedgeService : IHedgeService
    {
        public double CalculateProfit(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            throw new System.NotImplementedException();
        }

        public List<Position> OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            throw new System.NotImplementedException();
        }

        public List<Position> OnPartialClose(ExpertSetWrapper exp, Sides spreadOrderType, double lotRatio)
        {
            throw new System.NotImplementedException();
        }

        public List<Position> OnBaseTradesOpened(ExpertSetWrapper exp, Sides spreadOrderType, double[] sourceLots)
        {
            throw new System.NotImplementedException();
        }
    }
}

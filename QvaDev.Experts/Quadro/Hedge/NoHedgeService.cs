using System.Collections.Generic;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Hedge
{
    public class NoHedgeService : IHedgeService
    {
        public double CalculateProfit(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            return 0;
        }

        public List<Position> OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            return new List<Position>();
        }

        public List<Position> OnPartialClose(ExpertSetWrapper exp, Sides spreadOrderType, double lotRatio)
        {
            return new List<Position>();
        }

        public List<Position> OnBaseTradesOpened(ExpertSetWrapper exp, Sides spreadOrderType, double[] sourceLots)
        {
            return new List<Position>();
        }
    }
}

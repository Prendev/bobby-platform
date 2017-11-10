using System.Collections.Generic;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;
using QvaDev.Experts.Quadro.Services;

namespace QvaDev.Experts.Quadro.Hedge
{
    public class NoHedgeService : BaseHedgeService, IHedgeService
    {
        public NoHedgeService(ICommonService commonService) : base(commonService)
        {
        }

        public double CalculateProfit(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            return 0;
        }

        public override List<Position> OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType)
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

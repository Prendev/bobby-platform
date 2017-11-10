using System.Collections.Generic;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;
using QvaDev.Experts.Quadro.Services;

namespace QvaDev.Experts.Quadro.Hedge
{
    public class TwoPairHedgeService : BaseHedgeService, IHedgeService
    {
        public TwoPairHedgeService(ICommonService commonService) : base(commonService)
        {
        }

        public double CalculateProfit(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            throw new System.NotImplementedException();
        }

        public override List<Position> OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType)
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

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

        public void CheckHedgeStopByQuant(ExpertSetWrapper exp)
        {
        }

        public void CheckHedgeProfitClose(ExpertSetWrapper exp)
        {
        }

        public double CalculateProfit(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            return 0;
        }

        public override void OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType)
        {
        }

        public void OnPartialClose(ExpertSetWrapper exp, Sides spreadOrderType, double lotRatio)
        {
        }

        public void OnBaseTradesOpened(ExpertSetWrapper exp, Sides spreadOrderType, double[] sourceLots)
        {
        }
    }
}

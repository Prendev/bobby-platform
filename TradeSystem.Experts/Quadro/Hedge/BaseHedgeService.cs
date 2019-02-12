using System;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;
using QvaDev.Experts.Quadro.Services;

namespace QvaDev.Experts.Quadro.Hedge
{
    public abstract class BaseHedgeService
    {
        private readonly ICommonService _commonService;

        protected BaseHedgeService(ICommonService commonService)
        {
            _commonService = commonService;
        }

        protected void CheckHedgeStopByQuant(ExpertSetWrapper exp, Sides spreadOrderType, Predicate<double> predicate)
        {
            var orders = _commonService.GetBaseOpenOrdersList(exp, spreadOrderType)
                .Where(o => o.Symbol == exp.E.Symbol1)
                .OrderBy(o => o.OpenTime).ToList();
            if (!predicate(_commonService.BarQuant(exp, orders[exp.E.HedgeStopPositionCount]))) return;
            OnCloseAll(exp, spreadOrderType);
        }

        public abstract void OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType);
    }
}

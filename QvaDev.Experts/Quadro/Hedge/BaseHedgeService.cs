using System;
using System.Collections.Generic;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
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

        public void CheckHedgeStopByQuant(ExpertSetWrapper exp)
        {
            if (exp.E.HedgeStopPositionCount < 0 || exp.E.HedgeMode == ExpertSet.HedgeModes.NoHedge) return;
            if (exp.SellHedgeOpenCount > 0)
                CheckHedgeStopByQuant(exp, Sides.Sell, barQuant => exp.Quants.First() < barQuant);
            if (exp.BuyHedgeOpenCount > 0)
                CheckHedgeStopByQuant(exp, Sides.Buy, barQuant => exp.Quants.First() > barQuant);
        }

        private void CheckHedgeStopByQuant(ExpertSetWrapper exp, Sides spreadOrderType, Predicate<double> predicate)
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

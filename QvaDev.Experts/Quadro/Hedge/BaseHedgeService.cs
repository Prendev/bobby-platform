using System;
using System.Collections.Generic;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Hedge
{
    public abstract class BaseHedgeService
    {
        public void CheckHedgeStopByQuant(ExpertSetWrapper exp)
        {
            if (exp.E.HedgeStopPositionCount < 0 || exp.E.HedgeMode == ExpertSet.HedgeModes.NoHedge) return;
            if (exp.SellHedgeOpenCount > 0)
                CheckHedgeStopByQuant(exp, Sides.Sell, barQuant => exp.Quants.Last() < barQuant);
            if (exp.BuyHedgeOpenCount > 0)
                CheckHedgeStopByQuant(exp, Sides.Buy, barQuant => exp.Quants.Last() > barQuant);
        }

        private void CheckHedgeStopByQuant(ExpertSetWrapper exp, Sides spreadOrderType, Predicate<double> predicate)
        {
            if (!predicate(exp.BarQuant(exp.GetBaseOpenOrdersList(spreadOrderType).Where(o => o.Symbol == exp.E.Symbol1)
                .OrderBy(o => o.OpenTime).ToList()[exp.E.HedgeStopPositionCount]))) return;
            OnCloseAll(exp, spreadOrderType);
        }

        public abstract List<Position> OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType);
    }
}

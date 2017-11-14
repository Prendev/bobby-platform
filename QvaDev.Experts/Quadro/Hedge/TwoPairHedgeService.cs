using System;
using System.Collections.Generic;
using QvaDev.Common.Integration;
using QvaDev.Experts.Quadro.Models;
using QvaDev.Experts.Quadro.Services;

namespace QvaDev.Experts.Quadro.Hedge
{
    public class TwoPairHedgeService : BaseHedgeService, IHedgeService
    {
        private readonly ICommonService _commonService;

        public TwoPairHedgeService(ICommonService commonService) : base(commonService)
        {
            _commonService = commonService;
        }

        public void CheckHedgeProfitClose(ExpertSetWrapper exp)
        {
            CheckProfitStop(exp, Sides.Sell, CalculateProfit(exp, Sides.Sell));
            CheckProfitStop(exp, Sides.Buy, CalculateProfit(exp, Sides.Buy));
        }

        public double CalculateProfit(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            return spreadOrderType == Sides.Buy
                ? _commonService.CalculateProfit(exp, exp.HedgeBuyMagicNumber, exp.Sym1HedgeMinOrderType, exp.Sym2HedgeMinOrderType)
                : _commonService.CalculateProfit(exp, exp.HedgeSellMagicNumber, exp.Sym1HedgeMaxOrderType, exp.Sym2HedgeMaxOrderType);
        }

        private void CheckProfitStop(ExpertSetWrapper exp, Sides spreadOrderType, double profit)
        {
            if (profit >= exp.E.HedgeProfitStop || Math.Abs(profit) >= exp.E.HedgeLossStop)
                OnCloseAll(exp, spreadOrderType);
        }

        public override void OnCloseAll(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            if (spreadOrderType == Sides.Sell)
            {
                foreach (var position in GetMaxHedgeOrders(exp))
                    exp.Connector.SendClosePositionRequests(position);
                exp.SellHedgeOpenCount = 0;
            }
            else if (spreadOrderType == Sides.Buy)
            {
                foreach (var position in GetMinHedgeOrders(exp))
                    exp.Connector.SendClosePositionRequests(position);
                exp.BuyHedgeOpenCount = 0;
            }
        }

        public void OnPartialClose(ExpertSetWrapper exp, Sides spreadOrderType, double lotRatio)
        {
            if (spreadOrderType == Sides.Sell)
            {
                foreach (var position in GetMaxHedgeOrders(exp))
                    exp.Connector.SendClosePositionRequests(position);
            }
            else if (spreadOrderType == Sides.Buy)
            {
                foreach (var position in GetMinHedgeOrders(exp))
                    exp.Connector.SendClosePositionRequests(position);
            }
        }

        public void OnBaseTradesOpened(ExpertSetWrapper exp, Sides spreadOrderType, double[] sourceLots)
        {
            double lot1 = (sourceLots[0] * exp.E.HedgeRatio).CheckLot();
            double lot2 = (sourceLots[1] * exp.E.HedgeRatio).CheckLot();
            if (spreadOrderType == Sides.Sell && exp.SellOpenCount > exp.E.HedgeStart && exp.SellOpenCount < exp.E.HedgeStop - 1)
            {
                exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1HedgeMaxOrderType, lot1, exp.HedgeSellMagicNumber, exp.E.Description);
                exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2HedgeMaxOrderType, lot2, exp.HedgeSellMagicNumber, exp.E.Description);
                exp.SellHedgeOpenCount++;
            }
            else if (spreadOrderType == Sides.Sell && exp.SellOpenCount >= exp.E.HedgeStop - 1)
            {
                OnCloseAll(exp, spreadOrderType);
            }
            else if (spreadOrderType == Sides.Buy && exp.BuyOpenCount > exp.E.HedgeStart && exp.BuyOpenCount < exp.E.HedgeStop - 1)
            {
                exp.Connector.SendMarketOrderRequest(exp.E.Symbol1, exp.Sym1HedgeMinOrderType, lot1, exp.HedgeBuyMagicNumber, exp.E.Description);
                exp.Connector.SendMarketOrderRequest(exp.E.Symbol2, exp.Sym2HedgeMinOrderType, lot2, exp.HedgeBuyMagicNumber, exp.E.Description);
                exp.BuyHedgeOpenCount++;
            }
            else if (spreadOrderType == Sides.Buy && exp.BuyOpenCount > exp.E.HedgeStop)
            {
                OnCloseAll(exp, spreadOrderType);
            }
        }

        private List<Position> GetMaxHedgeOrders(ExpertSetWrapper exp)
        {
            return _commonService.GetOpenOrdersList(exp, exp.E.Symbol1, exp.Sym1HedgeMaxOrderType, exp.E.Symbol2,
                exp.Sym2HedgeMaxOrderType, exp.HedgeSellMagicNumber);
        }

        private List<Position> GetMinHedgeOrders(ExpertSetWrapper exp)
        {
            return _commonService.GetOpenOrdersList(exp, exp.E.Symbol1, exp.Sym1HedgeMinOrderType, exp.E.Symbol2,
                exp.Sym2HedgeMinOrderType, exp.HedgeBuyMagicNumber);
        }
    }
}

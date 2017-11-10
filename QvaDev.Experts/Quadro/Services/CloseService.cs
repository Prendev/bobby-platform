using System;
using System.Linq;
using Autofac.Features.Indexed;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Experts.Quadro.Hedge;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Services
{
    public interface ICloseService
    {
        void CheckClose(ExpertSetWrapper expertSet);
        void AllCloseMin(ExpertSetWrapper exp);
        void AllCloseMax(ExpertSetWrapper exp);
    }

    public class CloseService : ICloseService
    {
        private readonly IIndex<ExpertSet.HedgeModes, IHedgeService> _hedgeServices;
        private readonly ICommonService _commonService;

        public CloseService(
            ICommonService commonService,
            IIndex<ExpertSet.HedgeModes, IHedgeService> hedgeServices)
        {
            _commonService = commonService;
            _hedgeServices = hedgeServices;
        }

        public void CheckClose(ExpertSetWrapper expertSet)
        {
            CheckQuantClose(expertSet, Sides.Sell);
            CheckQuantClose(expertSet, Sides.Buy);
        }

        private void CheckQuantClose(ExpertSetWrapper exp, Sides side)
        {
            if (_commonService.IsInDeltaRange(exp, side)) return;
            if (exp.E.BaseTradesForPositiveClose)
            {
                double hedgeProfit = _hedgeServices[exp.E.HedgeMode].CalculateProfit(exp, side);
                double baseProfit = CalculateBaseOrdersProfit(exp, side);
                if (baseProfit + (exp.E.HedgeTradeForPositiveClose ? hedgeProfit : 0) <= 0) return;
            }
            if (side == Sides.Buy) CheckQuantBuyClose(exp);
            else CheckQuantSellClose(exp);
        }

        private double CalculateBaseOrdersProfit(ExpertSetWrapper exp, Sides side)
        {
            double num = side == Sides.Sell
                ? CalculateProfit(exp, Sides.Buy, Sides.Sell, exp.SpreadSellMagicNumber)
                : CalculateProfit(exp, Sides.Sell, Sides.Buy, exp.SpreadBuyMagicNumber);
            return num;
        }

        public double CalculateProfit(ExpertSetWrapper exp, Sides side1, Sides side2, int magicNumber)
        {
            var positions = exp.Connector.Positions.Where(p => p.Value.MagicNumber == magicNumber &&
                                                           (p.Value.Symbol == exp.E.Symbol1 && p.Value.Side == side1 ||
                                                            p.Value.Symbol == exp.E.Symbol2 && p.Value.Side == side2));
            return positions.Sum(p => p.Value.NetProfit);
        }

        private void CheckQuantBuyClose(ExpertSetWrapper exp)
        {
            double buyAvgPrice = _commonService.BuyAveragePrice(exp);
            if (Math.Abs(buyAvgPrice) < exp.Point) return;
            if (!exp.E.PartialClose)
            {
                if (exp.Quants.Last() >= buyAvgPrice + exp.E.Tp1 * exp.Point)
                    AllCloseMin(exp);
            }
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.TradeOpened && exp.Quants.Last() >= buyAvgPrice + exp.E.Tp1 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterFirstClose && exp.Quants.Last() >= buyAvgPrice + exp.E.Tp2 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterSecondClose && exp.Quants.Last() >= buyAvgPrice + exp.E.Tp3 * exp.Point)
                AllCloseMin(exp);
        }

        private void CheckQuantSellClose(ExpertSetWrapper exp)
        {
            double sellAvgPrice = _commonService.SellAveragePrice(exp);
            if (Math.Abs(sellAvgPrice) < exp.Point) return;
            if (!exp.E.PartialClose)
            {
                if (exp.Quants.Last() <= sellAvgPrice - exp.E.Tp1 * exp.Point)
                    AllCloseMax(exp);
            }
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.TradeOpened && exp.Quants.Last() <= sellAvgPrice - exp.E.Tp1 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterFirstClose && exp.Quants.Last() <= sellAvgPrice - exp.E.Tp2 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterSecondClose && exp.Quants.Last() <= sellAvgPrice - exp.E.Tp3 * exp.Point)
                AllCloseMax(exp);
        }

        public void AllCloseMin(ExpertSetWrapper exp)
        {
            AllCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            exp.InitBuyLotArray();
        }
        public void AllCloseMax(ExpertSetWrapper exp)
        {
            AllCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            exp.InitSellLotArray();
        }

        private void AllCloseLevel(ExpertSetWrapper exp, Sides orderType1, Sides orderType2, Sides spreadOrderType)
        {
            int magicNumber = _commonService.GetMagicNumberBySpreadOrderType(exp, spreadOrderType);
            var sym1Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol1, orderType1, magicNumber);
            var sym2Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol2, orderType2, magicNumber);
            _commonService.SetLastActionPrice(exp, spreadOrderType);
            var hedgeOrders = _hedgeServices[exp.E.HedgeMode].OnCloseAll(exp, spreadOrderType);
            foreach (var position in sym1Orders.Union(sym2Orders).Union(hedgeOrders))
                exp.Connector.SendClosePositionRequests(position);
        }

        private void FirstAndSecondCloseLevel(ExpertSetWrapper exp, Sides orderType1, Sides orderType2,
            Sides spreadOrderType)
        {
            int magicNumber = _commonService.GetMagicNumberBySpreadOrderType(exp, spreadOrderType);
            var sym1Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol1, orderType1, magicNumber);
            var sym2Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol2, orderType2, magicNumber);
            _commonService.SetLastActionPrice(exp, spreadOrderType);
            var hedgeOrders = _hedgeServices[exp.E.HedgeMode].OnPartialClose(exp, spreadOrderType, 0.5);
            foreach (var position in sym1Orders.Union(sym2Orders).Union(hedgeOrders))
                exp.Connector.SendClosePositionRequests(position, (position.Lots / 2).CheckLot());
            var oldtradeSetState = spreadOrderType == Sides.Buy ? exp.CurrentBuyState : exp.CurrentSellState;
            var newTradeSetState = oldtradeSetState == ExpertSetWrapper.TradeSetStates.TradeOpened
                ? ExpertSetWrapper.TradeSetStates.AfterFirstClose
                : ExpertSetWrapper.TradeSetStates.AfterSecondClose;
            if (spreadOrderType == Sides.Buy) exp.CurrentBuyState = newTradeSetState;
            if (spreadOrderType == Sides.Sell) exp.CurrentSellState = newTradeSetState;
        }
    }
}

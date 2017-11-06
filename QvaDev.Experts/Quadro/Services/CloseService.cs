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
    }

    public class CloseService : ICloseService
    {
        private readonly IIndex<ExpertSet.HedgeModes, IHedgeService> _hedgeServices;

        public CloseService(IIndex<ExpertSet.HedgeModes, IHedgeService> hedgeServices)
        {
            _hedgeServices = hedgeServices;
        }

        public void CheckClose(ExpertSetWrapper expertSet)
        {
            CheckQuantClose(expertSet, Sides.Sell);
            CheckQuantClose(expertSet, Sides.Buy);
        }

        private void CheckQuantClose(ExpertSetWrapper exp, Sides side)
        {
            if (exp.IsInDeltaRange(side)) return;
            bool closeEnabled = true;
            if (exp.BaseTradesForPositiveClose)
            {
                double hedgeProfit = _hedgeServices[exp.HedgeMode].CalculateProfit(exp, side);
                double baseProfit = CalculateBaseOrdersProfit(exp, side);
                closeEnabled = !exp.HedgeTradeForPositiveClose ? baseProfit > 0 : baseProfit + hedgeProfit > 0;
            }
            if (!closeEnabled) return;
            if (side != Sides.Sell)
            {
                CheckQuantBuyClose(exp);
            }
            else
            {
                CheckQuantSellClose(exp);
            }
        }

        private double CalculateBaseOrdersProfit(ExpertSetWrapper exp, Sides side)
        {
            double num = side != Sides.Buy
                ? CalculateProfit(exp, Sides.Buy, Sides.Sell, exp.SpreadSellMagicNumber)
                : CalculateProfit(exp, Sides.Sell, Sides.Buy, exp.SpreadBuyMagicNumber);
            return num;
        }

        public double CalculateProfit(ExpertSetWrapper exp, Sides side1, Sides side2, int magicNumber)
        {
            var positions = exp.Connector.Positions.Where(p => p.Value.MagicNumber == magicNumber &&
                                                           (p.Value.Symbol == exp.Symbol1 && p.Value.Side == side1 ||
                                                            p.Value.Symbol == exp.Symbol2 && p.Value.Side == side2));
            return positions.Sum(p => p.Value.NetProfit);
        }

        private void CheckQuantBuyClose(ExpertSetWrapper exp)
        {
            double buyAvgPrice = exp.BuyAveragePrice();
            if (!exp.PartialClose)
            {
                if (exp.Quants.Last() < buyAvgPrice + exp.Tp1 * exp.Point ||
                    buyAvgPrice == 0) return;
                AllCloseMin(exp);
            }
            else if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp1 * exp.Point) &&
                     buyAvgPrice != 0 && exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.TradeOpened)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp2 * exp.Point) &&
                     buyAvgPrice != 0 && exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterFirstClose)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp3 * exp.Point) &&
                     buyAvgPrice != 0 && exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterSecondClose)
                AllCloseMin(exp);
        }

        private void CheckQuantSellClose(ExpertSetWrapper exp)
        {
            double buyAvgPrice = exp.BuyAveragePrice();
            if (!exp.PartialClose)
            {
                if (exp.Quants.Last() < buyAvgPrice + exp.Tp1 * exp.Point ||
                    buyAvgPrice == 0) return;
                AllCloseMin(exp);
            }
            else if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp1 * exp.Point) &&
                     buyAvgPrice != 0 && exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.TradeOpened)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp2 * exp.Point) &&
                     buyAvgPrice != 0 && exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterFirstClose)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (!(exp.Quants.Last() < buyAvgPrice + exp.Tp3 * exp.Point) &&
                     buyAvgPrice != 0 && exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterSecondClose)
                AllCloseMin(exp);

            double sellAvgPrice = exp.SellAveragePrice();
            if (!exp.PartialClose)
            {
                if (exp.Quants.Last() > sellAvgPrice - exp.Tp1 * exp.Point ||
                    sellAvgPrice == 0) return;
                AllCloseMax(exp);
            }
            else if (!(exp.Quants.Last() > sellAvgPrice - exp.Tp1 * exp.Point) &&
                     sellAvgPrice != 0 && exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.TradeOpened)
                FirstAndSecondCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            else if (!(exp.Quants.Last() > sellAvgPrice - exp.Tp2 * exp.Point) &&
                     sellAvgPrice != 0 && exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterFirstClose)
                FirstAndSecondCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            else if (!(exp.Quants.Last() > sellAvgPrice - exp.Tp3 * exp.Point) &&
                     sellAvgPrice != 0 && exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterSecondClose)
                AllCloseMax(exp);
        }

        private void AllCloseMin(ExpertSetWrapper exp)
        {
            AllCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            exp.InitBuyLotArray();
        }
        private void AllCloseMax(ExpertSetWrapper exp)
        {
            AllCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            exp.InitSellLotArray();
        }

        private void AllCloseLevel(ExpertSetWrapper exp, Sides orderType1, Sides orderType2, Sides spreadOrderType)
        {
            int magicNumber = exp.GetMagicNumberBySpreadOrderType(spreadOrderType);
            var sym1Orders = exp.GetOpenOrdersList(exp.Symbol1, orderType1, magicNumber);
            var sym2Orders = exp.GetOpenOrdersList(exp.Symbol2, orderType2, magicNumber);
            var hedgeOrders = _hedgeServices[exp.HedgeMode].OnCloseAll(exp, spreadOrderType);
            foreach (var position in sym1Orders.Union(sym2Orders).Union(hedgeOrders))
                exp.Connector.SendClosePositionRequests(position);
        }

        private void FirstAndSecondCloseLevel(ExpertSetWrapper exp, Sides orderType1, Sides orderType2,
            Sides spreadOrderType)
        {
            int magicNumber = exp.GetMagicNumberBySpreadOrderType(spreadOrderType);
            var sym1Orders = exp.GetOpenOrdersList(exp.Symbol1, orderType1, magicNumber);
            var sym2Orders = exp.GetOpenOrdersList(exp.Symbol2, orderType2, magicNumber);
            var hedgeOrders = _hedgeServices[exp.HedgeMode].OnPartialClose(exp, spreadOrderType, 0.5);
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

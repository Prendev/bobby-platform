using System;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Experts.Quadro.Models;

namespace QvaDev.Experts.Quadro.Services
{
    public interface ICloseService
    {
        void CheckClose(ExpertSetWrapper expertSet);
        void AllCloseMin(ExpertSetWrapper exp);
        void AllCloseMax(ExpertSetWrapper exp);
        void BisectingCloseMin(ExpertSetWrapper exp);
        void BisectingCloseMax(ExpertSetWrapper exp);
        void CheckProfitClose(ExpertSetWrapper exp, Sides spreadOrderType);
    }

    public class CloseService : ICloseService
    {
        private readonly ICommonService _commonService;
        private readonly ILog _log;

        public CloseService(
            ILog log,
            ICommonService commonService)
        {
            _log = log;
            _commonService = commonService;
        }

        public void CheckClose(ExpertSetWrapper expertSet)
        {
            CheckQuantClose(expertSet, Sides.Sell);
            CheckQuantClose(expertSet, Sides.Buy);
        }

        private void CheckQuantClose(ExpertSetWrapper exp, Sides side)
        {
            if (_commonService.IsInDeltaRange(exp, side)) return;
            if (exp.E.BaseTradesForPositiveClose && _commonService.CalculateBaseOrdersProfit(exp, side) <= 0) return;
            if (side == Sides.Buy) CheckQuantBuyClose(exp);
            else CheckQuantSellClose(exp);
        }

        private void CheckQuantBuyClose(ExpertSetWrapper exp)
        {
            double buyAvgPrice = BuyAveragePrice(exp);
            if (Math.Abs(buyAvgPrice) < exp.Point) return;
            if (!exp.E.PartialClose)
            {
                if (exp.LatestBarQuant.Quant >= buyAvgPrice + exp.E.Tp1 * exp.Point)
                    AllCloseMin(exp);
            }
            else if (exp.E.CurrentBuyState == ExpertSet.TradeSetStates.TradeOpened && exp.LatestBarQuant.Quant >= buyAvgPrice + exp.E.Tp1 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (exp.E.CurrentBuyState == ExpertSet.TradeSetStates.AfterFirstClose && exp.LatestBarQuant.Quant >= buyAvgPrice + exp.E.Tp2 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (exp.E.CurrentBuyState == ExpertSet.TradeSetStates.AfterSecondClose && exp.LatestBarQuant.Quant >= buyAvgPrice + exp.E.Tp3 * exp.Point)
                AllCloseMin(exp);
        }

        private void CheckQuantSellClose(ExpertSetWrapper exp)
        {
            double sellAvgPrice = SellAveragePrice(exp);
            if (Math.Abs(sellAvgPrice) < exp.Point) return;
            if (!exp.E.PartialClose)
            {
                if (exp.LatestBarQuant.Quant <= sellAvgPrice - exp.E.Tp1 * exp.Point)
                    AllCloseMax(exp);
            }
            else if (exp.E.CurrentBuyState == ExpertSet.TradeSetStates.TradeOpened && exp.LatestBarQuant.Quant <= sellAvgPrice - exp.E.Tp1 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            else if (exp.E.CurrentBuyState == ExpertSet.TradeSetStates.AfterFirstClose && exp.LatestBarQuant.Quant <= sellAvgPrice - exp.E.Tp2 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            else if (exp.E.CurrentBuyState == ExpertSet.TradeSetStates.AfterSecondClose && exp.LatestBarQuant.Quant <= sellAvgPrice - exp.E.Tp3 * exp.Point)
                AllCloseMax(exp);
        }

        private double BuyAveragePrice(ExpertSetWrapper exp)
        {
            return AveragePrice(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, exp.SpreadBuyMagicNumber);
        }
        private double SellAveragePrice(ExpertSetWrapper exp)
        {
            return AveragePrice(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, exp.SpreadSellMagicNumber);
        }
        private double AveragePrice(ExpertSetWrapper exp, Sides orderType1, Sides orderType2, int magicNumber)
        {
            double avgPrice = 0;
            double[] sumLotSum1 = GetSumAndLotSum(exp, exp.E.Symbol1, orderType1, magicNumber);
            double[] sumLotSum2 = GetSumAndLotSum(exp, exp.E.Symbol2, orderType2, magicNumber);
            double multiSum = sumLotSum1[0] + sumLotSum2[0];
            double lotSum = sumLotSum1[1] + sumLotSum2[1];
            if (lotSum > 0)
            {
                avgPrice = exp.Connector.MyRoundToDigits(exp.E.Symbol1, multiSum / lotSum);
            }
            return avgPrice;
        }
        private double[] GetSumAndLotSum(ExpertSetWrapper exp, string symbol, Sides orderType, int magicNumber)
        {
            double multiSum = 0;
            double lotSum = 0;
            foreach (var p in _commonService.GetOpenOrdersList(exp, symbol, orderType, magicNumber))
            {
                double sellMultiplication = exp.Connector.MyRoundToDigits(p.Symbol, _commonService.BarQuant(exp, p) * p.Lots);
                multiSum += sellMultiplication;
                lotSum += p.Lots;
            }
            return new[] { multiSum, lotSum };
        }

        public void AllCloseMin(ExpertSetWrapper exp)
        {
            _log.Debug($"{exp.E.Description}: CloseService.AllCloseMin");
            AllCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            exp.InitBuyLotArray();
            exp.E.CloseAllBuy = false;
        }
        public void AllCloseMax(ExpertSetWrapper exp)
        {
            _log.Debug($"{exp.E.Description}: CloseService.AllCloseMax");
            AllCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            exp.InitSellLotArray();
            exp.E.CloseAllSell = false;
        }

        public void BisectingCloseMin(ExpertSetWrapper exp)
        {
            _log.Debug($"{exp.E.Description}: CloseService.BisectingCloseMin");
            BisectingClose(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            exp.E.BisectingCloseBuy = false;
        }
        public void BisectingCloseMax(ExpertSetWrapper exp)
        {
            _log.Debug($"{exp.E.Description}: CloseService.BisectingCloseMax");
            BisectingClose(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            exp.E.BisectingCloseSell = false;
        }
        private void BisectingClose(ExpertSetWrapper exp, Sides orderType1, Sides orderType2, Sides spreadOrderType)
        {
            int magicNumber = _commonService.GetMagicNumberBySpreadOrderType(exp, spreadOrderType);
            var sym1Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol1, orderType1, magicNumber);
            var sym2Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol2, orderType2, magicNumber);
            var orders = sym1Orders.Union(sym2Orders).ToList();
            if (orders.Count(o => o.Lots - (o.Lots / 2).CheckLot() > 0) == 1)
            {
                AllCloseLevel(exp, orderType1, orderType2, spreadOrderType);
                return;
            }
            foreach (var position in orders)
                exp.Connector.SendClosePositionRequests(position, (position.Lots / 2).CheckLot());
        }

        public void CheckProfitClose(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            bool isBuy = spreadOrderType == Sides.Buy;
            double num = isBuy ? exp.E.ProfitCloseValueBuy : exp.E.ProfitCloseValueSell;
            if (!(GetCurrentProfit(exp, spreadOrderType) >= num)) return;
            if (isBuy) AllCloseMin(exp);
            else AllCloseMax(exp);
        }

        private double GetCurrentProfit(ExpertSetWrapper exp, Sides spreadOrderType)
        {
            return _commonService.CalculateBaseOrdersProfit(exp, spreadOrderType);
        }

        private void AllCloseLevel(ExpertSetWrapper exp, Sides orderType1, Sides orderType2, Sides spreadOrderType)
        {
            int magicNumber = _commonService.GetMagicNumberBySpreadOrderType(exp, spreadOrderType);
            var sym1Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol1, orderType1, magicNumber);
            var sym2Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol2, orderType2, magicNumber);
            _commonService.SetLastActionPrice(exp, spreadOrderType);
            foreach (var position in sym1Orders.Union(sym2Orders))
                exp.Connector.SendClosePositionRequests(position);
        }

        private void FirstAndSecondCloseLevel(ExpertSetWrapper exp, Sides orderType1, Sides orderType2,
            Sides spreadOrderType)
        {
            int magicNumber = _commonService.GetMagicNumberBySpreadOrderType(exp, spreadOrderType);
            var sym1Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol1, orderType1, magicNumber);
            var sym2Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol2, orderType2, magicNumber);
            _commonService.SetLastActionPrice(exp, spreadOrderType);
            foreach (var position in sym1Orders.Union(sym2Orders))
                exp.Connector.SendClosePositionRequests(position, (position.Lots / 2).CheckLot());
            var oldtradeSetState = spreadOrderType == Sides.Buy ? exp.E.CurrentBuyState : exp.E.CurrentSellState;
            var newTradeSetState = oldtradeSetState == ExpertSet.TradeSetStates.TradeOpened
                ? ExpertSet.TradeSetStates.AfterFirstClose
                : ExpertSet.TradeSetStates.AfterSecondClose;
            if (spreadOrderType == Sides.Buy) exp.E.CurrentBuyState = newTradeSetState;
            if (spreadOrderType == Sides.Sell) exp.E.CurrentSellState = newTradeSetState;
        }
    }
}

using System;
using System.Linq;
using Autofac.Features.Indexed;
using log4net;
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
        void CheckProfitClose(ExpertSetWrapper exp, Sides spreadOrderType);
    }

    public class CloseService : ICloseService
    {
        private readonly IIndex<ExpertSet.HedgeModes, IHedgeService> _hedgeServices;
        private readonly ICommonService _commonService;
        private readonly ILog _log;

        public CloseService(
            ILog log,
            ICommonService commonService,
            IIndex<ExpertSet.HedgeModes, IHedgeService> hedgeServices)
        {
            _log = log;
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
                double baseProfit = _commonService.CalculateBaseOrdersProfit(exp, side);
                if (baseProfit + (exp.E.HedgeTradeForPositiveClose ? hedgeProfit : 0) <= 0) return;
                _log.Debug($"{exp.E.Description}: CloseService.CheckQuantClose({side:F}) => hedgeProfit = {hedgeProfit} | baseProfit = {baseProfit}");
            }
            if (side == Sides.Buy) CheckQuantBuyClose(exp);
            else CheckQuantSellClose(exp);
        }

        private void CheckQuantBuyClose(ExpertSetWrapper exp)
        {
            double buyAvgPrice = BuyAveragePrice(exp);
            if (Math.Abs(buyAvgPrice) < exp.Point) return;
            if (!exp.E.PartialClose)
            {
                if (exp.Quant >= buyAvgPrice + exp.E.Tp1 * exp.Point)
                    AllCloseMin(exp);
            }
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.TradeOpened && exp.Quant >= buyAvgPrice + exp.E.Tp1 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterFirstClose && exp.Quant >= buyAvgPrice + exp.E.Tp2 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MinOrderType, exp.Sym2MinOrderType, Sides.Buy);
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterSecondClose && exp.Quant >= buyAvgPrice + exp.E.Tp3 * exp.Point)
                AllCloseMin(exp);
        }

        private void CheckQuantSellClose(ExpertSetWrapper exp)
        {
            double sellAvgPrice = SellAveragePrice(exp);
            if (Math.Abs(sellAvgPrice) < exp.Point) return;
            if (!exp.E.PartialClose)
            {
                if (exp.Quant <= sellAvgPrice - exp.E.Tp1 * exp.Point)
                    AllCloseMax(exp);
            }
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.TradeOpened && exp.Quant <= sellAvgPrice - exp.E.Tp1 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterFirstClose && exp.Quant <= sellAvgPrice - exp.E.Tp2 * exp.Point)
                FirstAndSecondCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            else if (exp.CurrentBuyState == ExpertSetWrapper.TradeSetStates.AfterSecondClose && exp.Quant <= sellAvgPrice - exp.E.Tp3 * exp.Point)
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
        }
        public void AllCloseMax(ExpertSetWrapper exp)
        {
            _log.Debug($"{exp.E.Description}: CloseService.AllCloseMax");
            AllCloseLevel(exp, exp.Sym1MaxOrderType, exp.Sym2MaxOrderType, Sides.Sell);
            exp.InitSellLotArray();
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
            return _commonService.CalculateBaseOrdersProfit(exp, spreadOrderType) +
                   _hedgeServices[exp.E.HedgeMode].CalculateProfit(exp, spreadOrderType);
        }

        private void AllCloseLevel(ExpertSetWrapper exp, Sides orderType1, Sides orderType2, Sides spreadOrderType)
        {
            int magicNumber = _commonService.GetMagicNumberBySpreadOrderType(exp, spreadOrderType);
            var sym1Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol1, orderType1, magicNumber);
            var sym2Orders = _commonService.GetOpenOrdersList(exp, exp.E.Symbol2, orderType2, magicNumber);
            _commonService.SetLastActionPrice(exp, spreadOrderType);
            _hedgeServices[exp.E.HedgeMode].OnCloseAll(exp, spreadOrderType);
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
            _hedgeServices[exp.E.HedgeMode].OnPartialClose(exp, spreadOrderType, 0.5);
            foreach (var position in sym1Orders.Union(sym2Orders))
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

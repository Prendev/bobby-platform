using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Mt4Integration;

namespace QvaDev.Experts.Quadro.Models
{
    public class ExpertSetWrapper
    {
        public enum TradeSetStates
        {
            NoTrade,
            TradeOpened,
            AfterFirstClose,
            AfterSecondClose
        }

        public ExpertSet ExpertSet { get; }

        public ExpertSetWrapper(ExpertSet expertSet)
        {
            ExpertSet = expertSet;
            InitByVariant();
            InitBuyLotArray();
            InitSellLotArray();
            InitLotArray(InitialLots, ExpertSet.LotSize);
        }

        public Connector Connector => ExpertSet.TradingAccount.MetaTraderAccount.Connector as Connector;
        public ConcurrentDictionary<long, Position> Positions => ExpertSet.TradingAccount.MetaTraderAccount.Connector.Positions;
        public string Symbol1 => ExpertSet.Symbol1;
        public string Symbol2 => ExpertSet.Symbol2;
        public bool BaseTradesForPositiveClose => ExpertSet.BaseTradesForPositiveClose;
        public bool HedgeTradeForPositiveClose => ExpertSet.HedgeTradeForPositiveClose;
        public bool PartialClose => ExpertSet.PartialClose;

        public double M => ExpertSet.M;
        public int SpreadBuyMagicNumber => ExpertSet.MagicNumber;
        public int SpreadSellMagicNumber => ExpertSet.MagicNumber + 1;
        public int HedgeBuyHedgicNumber => ExpertSet.HedgicNumber + 9;
        public int HedgeSellHedgicNumber => ExpertSet.HedgicNumber + 10;

        public int Tp1 => ExpertSet.Tp1;
        public int Tp2 => ExpertSet.Tp2;
        public int Tp3 => ExpertSet.Tp3;

        public double ExposureShieldValue => ExpertSet.ExposureShieldValue;

        public List<Bar> BarHistory1 { get; set; }
        public List<Bar> BarHistory2 { get; set; }
        public List<double> Quants { get; set; }
        public int BuyHedgeOpenCount { get; set; }
        public int SellHedgeOpenCount { get; set; }

        public double? QuantSto => CalcSto(ExpertSet.StochMultiplication);
        public double? QuantSto1 => CalcSto(ExpertSet.StochMultiplication * ExpertSet.StochMultiplier1);
        public double? QuantSto2 => CalcSto(ExpertSet.StochMultiplication * ExpertSet.StochMultiplier2);
        public double? QuantSto3 => CalcSto(ExpertSet.StochMultiplication * ExpertSet.StochMultiplier3);
        public double? QuantStoAvg => CalcAvg(QuantSto, QuantSto1, QuantSto2, QuantSto3);
        public double StochMinAvgOpen => ExpertSet.Diff;
        public double StochMaxAvgOpen => 100 - ExpertSet.Diff;

        public double? QuantWpr => CalcWpr(ExpertSet.WprMultiplication);
        public double? QuantWpr1 => CalcWpr(ExpertSet.WprMultiplication * ExpertSet.WprMultiplier1);
        public double? QuantWpr2 => CalcWpr(ExpertSet.WprMultiplication * ExpertSet.WprMultiplier2);
        public double? QuantWpr3 => CalcWpr(ExpertSet.WprMultiplication * ExpertSet.WprMultiplier3);
        public double? QuantWprAvg => CalcAvg(QuantWpr, QuantWpr1, QuantWpr2, QuantWpr3);
        public double WprMinAvgOpen => ExpertSet.Diff;
        public double WprMaxAvgOpen => 100 - ExpertSet.Diff;

        public Sides Sym1MaxOrderType { get; private set; }
        public Sides Sym1MinOrderType { get; private set; }
        public Sides Sym2MaxOrderType { get; private set; }
        public Sides Sym2MinOrderType { get; private set; }
        protected bool Sym1QuantInverse { get; private set; }
        protected bool Sym2QuantInverse { get; private set; }

        public bool TradeOpeningEnabled { get; set; } = true;

        public double[,] BuyLots { get; } = new double[120, 2];
        public double[,] SellLots { get; } = new double[120, 2];
        public double[,] InitialLots { get; } = new double[120, 2];

        public double Sym1LastMaxActionPrice { get; set; }
        public double Sym1LastMinActionPrice { get; set; }
        public double Sym2LastMaxActionPrice { get; set; }
        public double Sym2LastMinActionPrice { get; set; }
        public int MaxTradeSetCount => ExpertSet.MaxTradeSetCount;
        public int Last24HMaxOpen => ExpertSet.Last24HMaxOpen;

        public TradeSetStates CurrentSellState = TradeSetStates.NoTrade;
        public TradeSetStates CurrentBuyState = TradeSetStates.NoTrade;

        public int BuyOpenCount => ExpertSet.TradingAccount.MetaTraderAccount.Connector.Positions.Count(
            p => p.Value.MagicNumber == SpreadBuyMagicNumber || p.Value.MagicNumber == HedgeBuyHedgicNumber);
        public int SellOpenCount => ExpertSet.TradingAccount.MetaTraderAccount.Connector.Positions.Count(
            p => p.Value.MagicNumber == SpreadSellMagicNumber || p.Value.MagicNumber == HedgeSellHedgicNumber);

        public int ReOpenDiffChangeCount => ExpertSet.ReOpenDiffChangeCount;
        public int ReOpenDiff => ExpertSet.ReOpenDiff;
        public int ReOpenDiff2 => ExpertSet.ReOpenDiff2;
        public double Point => Connector.GetPoint(Symbol1);

        public ExpertSet.HedgeModes HedgeMode => ExpertSet.HedgeMode;
        public int HedgeStopPositionCount => ExpertSet.HedgeStopPositionCount;

        private double? CalcSto(int period)
        {
            if (Quants.Count < period) return null;
            var range = Quants.GetRange(Quants.Count - period, period);
            double diff = range.Max() - range.Min();
            if (diff <= -1) return null;
            return (Quants.Last() - range.Min()) / diff * 100;
        }
        private double? CalcWpr(int period)
        {
            if (Quants.Count < period) return null;
            var range = Quants.GetRange(Quants.Count - period, period);
            double diff = range.Max() - range.Min();
            if (diff <= -1) return null;
            return (range.Max() - Quants.Last()) / diff * -100;
        }
        private double? CalcAvg(double? value, double? value1, double? value2, double? value3)
        {
            if (!value.HasValue) return null;
            if (!value1.HasValue) return null;
            if (!value2.HasValue) return null;
            if (!value3.HasValue) return null;
            return (value + value1 + value2 + value3) / 4;
        }

        public void InitBuyLotArray()
        {
            InitLotArray(BuyLots, ExpertSet.LotSize);
        }
        public void InitSellLotArray()
        {
            InitLotArray(SellLots, ExpertSet.LotSize);
        }

        private void InitLotArray(double[,] lotArray, double initialLot)
        {
            lotArray[0, 0] = initialLot.CheckLot();
            for (int i = 1; i < 120; i++)
            {
                lotArray[i, 0] = (lotArray[i - 1, 0] + 2 * lotArray[0, 0]).CheckLot();
            }
            for (int i = 0; i < 120; i++)
            {
                lotArray[i, 1] = (lotArray[i, 0] * M).CheckLot();
            }
        }
        
        private void InitByVariant()
        {
            switch (ExpertSet.Variant)
            {
                case ExpertSet.Variants.NormalNormalBase:
                {
                    Sym1MaxOrderType = Sides.Buy;
                    Sym2MaxOrderType = Sides.Sell;
                    Sym1MinOrderType = Sides.Sell;
                    Sym2MinOrderType = Sides.Buy;
                    break;
                }
                case ExpertSet.Variants.NormalNormalReversed:
                {
                    Sym1MaxOrderType = Sides.Sell;
                    Sym2MaxOrderType = Sides.Buy;
                    Sym1MinOrderType = Sides.Buy;
                    Sym2MinOrderType = Sides.Sell;
                    break;
                }
                case ExpertSet.Variants.NormalInverseBase:
                {
                    Sym1MaxOrderType = Sides.Buy;
                    Sym2MaxOrderType = Sides.Buy;
                    Sym1MinOrderType = Sides.Sell;
                    Sym2MinOrderType = Sides.Sell;
                    Sym2QuantInverse = true;
                    break;
                }
                case ExpertSet.Variants.NormalInverseReversed:
                {
                    Sym1MaxOrderType = Sides.Sell;
                    Sym2MaxOrderType = Sides.Sell;
                    Sym1MinOrderType = Sides.Buy;
                    Sym2MinOrderType = Sides.Buy;
                    Sym2QuantInverse = true;
                    break;
                }
                case ExpertSet.Variants.InverseNormalBase:
                {
                    Sym1MaxOrderType = Sides.Buy;
                    Sym2MaxOrderType = Sides.Buy;
                    Sym1MinOrderType = Sides.Sell;
                    Sym2MinOrderType = Sides.Sell;
                    Sym1QuantInverse = true;
                    break;
                }
                case ExpertSet.Variants.InverseNormalReversed:
                {
                    Sym1MaxOrderType = Sides.Sell;
                    Sym2MaxOrderType = Sides.Sell;
                    Sym1MinOrderType = Sides.Buy;
                    Sym2MinOrderType = Sides.Buy;
                    Sym1QuantInverse = true;
                    break;
                }
                default:
                {
                    throw new InvalidOperationException(string.Concat("Unknown Variant type: ", ExpertSet.Variant));
                }
            }
        }
    }
}

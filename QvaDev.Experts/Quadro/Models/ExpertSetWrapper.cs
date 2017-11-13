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

        public ExpertSet E { get; }

        public ExpertSetWrapper(ExpertSet expertSet)
        {
            E = expertSet;
            InitByVariant();
            InitBuyLotArray();
            InitSellLotArray();
            InitLotArray(InitialLots, E.LotSize);
        }
        public bool ExpertDenied { get; set; }
        public Connector Connector => E.TradingAccount.MetaTraderAccount.Connector as Connector;
        public ConcurrentDictionary<long, Position> Positions => E.TradingAccount.MetaTraderAccount.Connector.Positions;

        public int SpreadBuyMagicNumber => E.MagicNumber;
        public int SpreadSellMagicNumber => E.MagicNumber + 1;
        public int HedgeBuyHedgicNumber => E.HedgicNumber + 9;
        public int HedgeSellHedgicNumber => E.HedgicNumber + 10;

        public List<Bar> BarHistory1 { get; set; } = new List<Bar>();
        public List<Bar> BarHistory2 { get; set; } = new List<Bar>();
        public List<double> Quants { get; set; } = new List<double>();
        public int BuyHedgeOpenCount { get; set; }
        public int SellHedgeOpenCount { get; set; }

        public double? QuantSto => CalcSto(E.StochMultiplication);
        public double? QuantSto1 => CalcSto(E.StochMultiplication * E.StochMultiplier1);
        public double? QuantSto2 => CalcSto(E.StochMultiplication * E.StochMultiplier2);
        public double? QuantSto3 => CalcSto(E.StochMultiplication * E.StochMultiplier3);
        public double? QuantStoAvg => CalcAvg(QuantSto, QuantSto1, QuantSto2, QuantSto3);
        public int StochMinAvgOpen => E.Diff;
        public int StochMaxAvgOpen => 100 - E.Diff;

        public double? QuantWpr => CalcWpr(E.WprMultiplication);
        public double? QuantWpr1 => CalcWpr(E.WprMultiplication * E.WprMultiplier1);
        public double? QuantWpr2 => CalcWpr(E.WprMultiplication * E.WprMultiplier2);
        public double? QuantWpr3 => CalcWpr(E.WprMultiplication * E.WprMultiplier3);
        public double? QuantWprAvg => CalcAvg(QuantWpr, QuantWpr1, QuantWpr2, QuantWpr3);
        public int WprMinAvgOpen => E.Diff;
        public int WprMaxAvgOpen => 100 - E.Diff;

        public Sides Sym1MaxOrderType { get; private set; }
        public Sides Sym1MinOrderType { get; private set; }
        public Sides Sym2MaxOrderType { get; private set; }
        public Sides Sym2MinOrderType { get; private set; }
        public bool Sym1QuantInverse { get; private set; }
        public bool Sym2QuantInverse { get; private set; }
        public Sides Sym1HedgeMaxOrderType => Sym1MaxOrderType.GetInverseOrder();
        public Sides Sym1HedgeMinOrderType => Sym1MinOrderType.GetInverseOrder();
        public Sides Sym2HedgeMaxOrderType => Sym2MaxOrderType.GetInverseOrder();
        public Sides Sym2HedgeMinOrderType => Sym2MinOrderType.GetInverseOrder();

        public double[,] BuyLots { get; } = new double[120, 2];
        public double[,] SellLots { get; } = new double[120, 2];
        public double[,] InitialLots { get; } = new double[120, 2];

        public double DeltaRange => E.Delta * Point;

        public double Sym1LastMaxActionPrice { get; set; }
        public double Sym1LastMinActionPrice { get; set; }
        public double Sym2LastMaxActionPrice { get; set; }
        public double Sym2LastMinActionPrice { get; set; }

        public TradeSetStates CurrentSellState = TradeSetStates.NoTrade;
        public TradeSetStates CurrentBuyState = TradeSetStates.NoTrade;

        public int BuyOpenCount => E.TradingAccount.MetaTraderAccount.Connector.Positions.Count(
            p => p.Value.MagicNumber == SpreadBuyMagicNumber || p.Value.MagicNumber == HedgeBuyHedgicNumber);
        public int SellOpenCount => E.TradingAccount.MetaTraderAccount.Connector.Positions.Count(
            p => p.Value.MagicNumber == SpreadSellMagicNumber || p.Value.MagicNumber == HedgeSellHedgicNumber);

        public double Point => Connector.GetPoint(E.Symbol1);

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

        public void InitBuyLotArray() => InitLotArray(BuyLots, E.LotSize);

        public void InitSellLotArray() => InitLotArray(SellLots, E.LotSize);

        private void InitLotArray(double[,] lotArray, double initialLot)
        {
            lotArray[0, 0] = initialLot.CheckLot();
            for (int i = 1; i < 120; i++)
            {
                lotArray[i, 0] = (lotArray[i - 1, 0] + 2 * lotArray[0, 0]).CheckLot();
            }
            for (int i = 0; i < 120; i++)
            {
                lotArray[i, 1] = (lotArray[i, 0] * E.M).CheckLot();
            }
        }
        
        private void InitByVariant()
        {
            switch (E.Variant)
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
                    throw new InvalidOperationException(string.Concat("Unknown Variant type: ", E.Variant));
                }
            }
        }
    }
}

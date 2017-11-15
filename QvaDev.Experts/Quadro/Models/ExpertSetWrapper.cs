using System;
using System.Collections.Generic;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Mt4Integration;

namespace QvaDev.Experts.Quadro.Models
{
    public class ExpertSetWrapper
    {
        public ExpertSet E { get; }

        public ExpertSetWrapper(ExpertSet expertSet)
        {
            E = expertSet;
            InitByVariant();
            InitBuyLotArray();
            InitSellLotArray();
            InitLotArray(InitialLots, E.LotSize);
            InitLastActionPrices();
        }
        public Connector Connector => E.TradingAccount.MetaTraderAccount.Connector as Connector;

        public IEnumerable<Position> OpenPositions =>
            E.TradingAccount.MetaTraderAccount.Connector.Positions.Select(p => p.Value)
                .Where(p => p.Symbol == E.Symbol1 || p.Symbol == E.Symbol2)
                .Where(p => p.MagicNumber == SpreadBuyMagicNumber ||
                            p.MagicNumber == SpreadSellMagicNumber ||
                            p.MagicNumber == HedgeBuyMagicNumber ||
                            p.MagicNumber == HedgeSellMagicNumber)
                .Where(p => !p.IsClosed);

        public int SpreadBuyMagicNumber => E.MagicNumber;
        public int SpreadSellMagicNumber => E.MagicNumber + 1;
        public int HedgeBuyMagicNumber => E.MagicNumber + 2;
        public int HedgeSellMagicNumber => E.MagicNumber + 3;

        public List<Bar> BarHistory1 { get; set; } = new List<Bar>();
        public List<Bar> BarHistory2 { get; set; } = new List<Bar>();
        public List<double> Quants { get; set; } = new List<double>();
        public int BuyHedgeOpenCount { get; set; }
        public int SellHedgeOpenCount { get; set; }


        public double Quant => Quants.First();
        public double? QuantStoAvg { get; private set; }
        public int StochMinAvgOpen => E.Diff;
        public int StochMaxAvgOpen => 100 - E.Diff;
        public double? QuantWprAvg { get; private set; }
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

        public int BuyOpenCount => E.TradingAccount.MetaTraderAccount.Connector.Positions.Count(
            p => p.Value.MagicNumber == SpreadBuyMagicNumber || p.Value.MagicNumber == HedgeBuyMagicNumber);
        public int SellOpenCount => E.TradingAccount.MetaTraderAccount.Connector.Positions.Count(
            p => p.Value.MagicNumber == SpreadSellMagicNumber || p.Value.MagicNumber == HedgeSellMagicNumber);

        public double Point => Connector.GetPoint(E.Symbol1);

        public void CalculateQuants()
        {
            var quants = new List<double>();
            for (var i = 0; i < E.GetMaxBarCount(); i++)
            {
                var price1Close = Connector.MyRoundToDigits(E.Symbol1, BarHistory1[i].Close);
                var price2Close = Connector.MyRoundToDigits(E.Symbol2, BarHistory2[i].Close);
                var quant = Connector.MyRoundToDigits(E.Symbol1, price2Close - E.M * price1Close);
                quants.Add(quant);
            }
            Quants = quants;

            double? quantSto = CalcSto(E.StochMultiplication);
            double? quantSto1 = CalcSto(E.StochMultiplication * E.StochMultiplier1);
            double? quantSto2 = CalcSto(E.StochMultiplication * E.StochMultiplier2);
            double? quantSto3 = CalcSto(E.StochMultiplication * E.StochMultiplier3);
            QuantStoAvg = CalcAvg(quantSto, quantSto1, quantSto2, quantSto3);

            double? quantWpr = CalcWpr(E.WprMultiplication);
            double? quantWpr1 = CalcWpr(E.WprMultiplication * E.WprMultiplier1);
            double? quantWpr2 = CalcWpr(E.WprMultiplication * E.WprMultiplier2);
            double? quantWpr3 = CalcWpr(E.WprMultiplication * E.WprMultiplier3);
            QuantWprAvg = CalcAvg(quantWpr, quantWpr1, quantWpr2, quantWpr3);
        }
        private double? CalcSto(int period, int index = 0)
        {
            if (Quants.Count < index + period) return null;
            var range = Quants.GetRange(index, period);
            double diff = range.Max() - range.Min();
            if (diff <= 0) return CalcSto(period, index + 1);
            return (Quants.First() - range.Min()) / diff * 100;
        }
        private double? CalcWpr(int period, int index = 0)
        {
            if (Quants.Count < index + period) return null;
            var range = Quants.GetRange(index, period);
            double diff = range.Max() - range.Min();
            if (diff <= 0) return CalcWpr(period, index + 1);
            return (range.Max() - Quants.First()) / diff * -100;
        }
        private double? CalcAvg(params double?[] values)
        {
            return values?.Any(v => !v.HasValue) == true ? null : values?.Average();
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

        private void InitLastActionPrices()
        {
            Sym1LastMinActionPrice = Connector.GetLastActionPrice(E.Symbol1, Sym1MinOrderType, SpreadBuyMagicNumber);
            Sym2LastMinActionPrice = Connector.GetLastActionPrice(E.Symbol2, Sym2MinOrderType, SpreadBuyMagicNumber);
            Sym1LastMaxActionPrice = Connector.GetLastActionPrice(E.Symbol1, Sym1MaxOrderType, SpreadSellMagicNumber);
            Sym2LastMaxActionPrice = Connector.GetLastActionPrice(E.Symbol2, Sym2MaxOrderType, SpreadSellMagicNumber);
        }
    }
}

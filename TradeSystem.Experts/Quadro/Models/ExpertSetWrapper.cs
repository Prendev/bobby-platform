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
        public class BarQuant
        {
            public DateTime OpenTime { get; set; }
            public Bar Bar1 { get; set; }
            public Bar Bar2 { get; set; }
            public double? Quant { get; set; }
        }

        public QuadroSet E { get; }

        public ExpertSetWrapper(QuadroSet quadroSet)
        {
            E = quadroSet;
            InitByVariant();
            InitBuyLotArray();
            InitSellLotArray();
            InitLotArray(InitialLots, E.LotSize);
        }
        public Connector Connector => E.TradingAccount.MetaTraderAccount.Connector as Connector;

        public IEnumerable<Position> OpenPositions =>
            E.TradingAccount.MetaTraderAccount.Connector.Positions.Select(p => p.Value)
                .Where(p => p.Symbol == E.Symbol1 || p.Symbol == E.Symbol2)
                .Where(p => p.MagicNumber == E.MagicNumber ||
                            p.MagicNumber == HedgeMagicNumber)
                .Where(p => !p.IsClosed);

        public int HedgeMagicNumber => E.MagicNumber + 2;

        public SortedDictionary<DateTime, BarQuant> BarQuants { get; set; } = new SortedDictionary<DateTime, BarQuant>();
        public int BuyHedgeOpenCount { get; set; }
        public int SellHedgeOpenCount { get; set; }

        public int QuantCount => BarQuants.Count(b => b.Value.Quant.HasValue);
        public BarQuant LatestBarQuant => BarQuants.Any()
            ? BarQuants.LastOrDefault(b => b.Value.Quant.HasValue).Value
            : null;
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
        public double Point => Connector.GetPoint(E.Symbol1);

        public DateTime? BarMissingOpenTime { get; set; }
        public DateTime LastBarOpenTime { get; set; }

        //[InvisibleColumn] public int BuyOpenCount => OpenPositions.Count(p => p.MagicNumber == SpreadBuyMagicNumber) / 2;
        //[InvisibleColumn] public int SellOpenCount => OpenPositions.Count(p => p.MagicNumber == SpreadSellMagicNumber) / 2;


        public void GetSpecificBars(DateTime time)
        {
            Connector.GetSpecificBars(time, (int)E.TimeFrame, E.Symbol1, E.Symbol2);
        }

        public void UpdateBarQuants(string symbol, ConcurrentDictionary<DateTime, Bar> barHistory)
        {
            foreach (var bar in barHistory.Select(b => b.Value))
            {
                if (!BarQuants.ContainsKey(bar.OpenTime))
                    BarQuants[bar.OpenTime] = new BarQuant { OpenTime = bar.OpenTime };
                var barQuant = BarQuants[bar.OpenTime];
                if (E.Symbol1 == symbol) barQuant.Bar1 = bar;
                else barQuant.Bar2 = bar;
                if (barQuant.Bar1 != null && barQuant.Bar2 != null)
                    BarQuants[bar.OpenTime].Quant =
                        Connector.MyRoundToDigits(E.Symbol1,
                            barQuant.Bar2.Close - E.M * barQuant.Bar1.Close);
            }
            CalculateIndicators();
        }
        private void CalculateIndicators()
        {
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
            if (QuantCount < index + period) return null;
            var range = BarQuants.OrderByDescending(bq => bq.Key)
                .Where(bq => bq.Value.Quant.HasValue)
                .Select(bq => bq.Value.Quant.Value).ToList()
                .GetRange(index, period);
            double diff = range.Max() - range.Min();
            if (diff <= 0) return CalcSto(period, index + 1);
            return (LatestBarQuant.Quant.Value - range.Min()) / diff * 100;
        }
        private double? CalcWpr(int period, int index = 0)
        {
            if (QuantCount < index + period) return null;
            var range = BarQuants.OrderByDescending(bq => bq.Key)
                .Where(bq => bq.Value.Quant.HasValue)
                .Select(bq => bq.Value.Quant.Value).ToList()
                .GetRange(index, period);
            double diff = range.Max() - range.Min();
            if (diff <= 0) return CalcWpr(period, index + 1);
            return (range.Max() - LatestBarQuant.Quant.Value) / diff * -100;
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
                case QuadroSet.Variants.NormalNormalBase:
                {
                    Sym1MaxOrderType = Sides.Buy;
                    Sym2MaxOrderType = Sides.Sell;
                    Sym1MinOrderType = Sides.Sell;
                    Sym2MinOrderType = Sides.Buy;
                    break;
                }
                case QuadroSet.Variants.NormalNormalReversed:
                {
                    Sym1MaxOrderType = Sides.Sell;
                    Sym2MaxOrderType = Sides.Buy;
                    Sym1MinOrderType = Sides.Buy;
                    Sym2MinOrderType = Sides.Sell;
                    break;
                }
                case QuadroSet.Variants.NormalInverseBase:
                {
                    Sym1MaxOrderType = Sides.Buy;
                    Sym2MaxOrderType = Sides.Buy;
                    Sym1MinOrderType = Sides.Sell;
                    Sym2MinOrderType = Sides.Sell;
                    Sym2QuantInverse = true;
                    break;
                }
                case QuadroSet.Variants.NormalInverseReversed:
                {
                    Sym1MaxOrderType = Sides.Sell;
                    Sym2MaxOrderType = Sides.Sell;
                    Sym1MinOrderType = Sides.Buy;
                    Sym2MinOrderType = Sides.Buy;
                    Sym2QuantInverse = true;
                    break;
                }
                case QuadroSet.Variants.InverseNormalBase:
                {
                    Sym1MaxOrderType = Sides.Buy;
                    Sym2MaxOrderType = Sides.Buy;
                    Sym1MinOrderType = Sides.Sell;
                    Sym2MinOrderType = Sides.Sell;
                    Sym1QuantInverse = true;
                    break;
                }
                case QuadroSet.Variants.InverseNormalReversed:
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

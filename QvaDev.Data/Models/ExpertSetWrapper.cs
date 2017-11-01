using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
    public class ExpertSetWrapper : ExpertSet
    {
        [NotMapped]
        [InvisibleColumn]
        public int SpreadBuyMagicNumber => MagicNumber;
        [NotMapped]
        [InvisibleColumn]
        public int SpreadSellMagicNumber => MagicNumber + 1;
        [NotMapped]
        [InvisibleColumn]
        public int HedgeBuyHedgicNumber => HedgicNumber + 9;
        [NotMapped]
        [InvisibleColumn]
        public int HedgeSellHedgicNumber => HedgicNumber + 10;

        [NotMapped]
        [InvisibleColumn]
        public bool IsFiltered { get => Get<bool>(); set => Set(value); }

        [NotMapped]
        [InvisibleColumn]
        public List<Bar> BarHistory1 { get; set; }
        [NotMapped]
        [InvisibleColumn]
        public List<Bar> BarHistory2 { get; set; }
        [NotMapped]
        [InvisibleColumn]
        public List<double> Quants { get; set; }

        [NotMapped]
        [InvisibleColumn]
        public double? QuantSto => CalcSto(StochMultiplication);
        [NotMapped]
        [InvisibleColumn]
        public double? QuantSto1 => CalcSto(StochMultiplication * StochMultiplier1);
        [NotMapped]
        [InvisibleColumn]
        public double? QuantSto2 => CalcSto(StochMultiplication * StochMultiplier2);
        [NotMapped]
        [InvisibleColumn]
        public double? QuantSto3 => CalcSto(StochMultiplication * StochMultiplier3);
        [NotMapped]
        [InvisibleColumn]
        public double? QuantStoAvg => CalcAvg(QuantSto, QuantSto1, QuantSto2, QuantSto3);
        [NotMapped]
        [InvisibleColumn]
        public double StochMinAvgOpen => Diff;
        [NotMapped]
        [InvisibleColumn]
        public double StochMaxAvgOpen => 100 - Diff;

        [NotMapped]
        [InvisibleColumn]
        public double? QuantWpr => CalcWpr(WprMultiplication);
        [NotMapped]
        [InvisibleColumn]
        public double? QuantWpr1 => CalcWpr(WprMultiplication * WprMultiplier1);
        [NotMapped]
        [InvisibleColumn]
        public double? QuantWpr2 => CalcWpr(WprMultiplication * WprMultiplier2);
        [NotMapped]
        [InvisibleColumn]
        public double? QuantWpr3 => CalcWpr(WprMultiplication * WprMultiplier3);
        [NotMapped]
        [InvisibleColumn]
        public double? QuantWprAvg => CalcAvg(QuantWpr, QuantWpr1, QuantWpr2, QuantWpr3);
        [NotMapped]
        [InvisibleColumn]
        public double WprMinAvgOpen => Diff;
        [NotMapped]
        [InvisibleColumn]
        public double WprMaxAvgOpen => 100 - Diff;

        [NotMapped]
        [InvisibleColumn]
        public bool TradeOpeningEnabled { get; set; } = true;

        [NotMapped]
        [InvisibleColumn]
        public double[,] BuyLots => InitLotArray(LotSize);
        [NotMapped]
        [InvisibleColumn]
        public double[,] SellLots => InitLotArray(LotSize);
        [NotMapped]
        [InvisibleColumn]
        public double[,] InitialLots => InitLotArray(LotSize);

        [NotMapped]
        [InvisibleColumn]
        public double Sym1LastMaxActionPrice { get; set; }
        [NotMapped]
        [InvisibleColumn]
        public double Sym1LastMinActionPrice { get; set; }
        [NotMapped]
        [InvisibleColumn]
        public double Sym2LastMaxActionPrice { get; set; }
        [NotMapped]
        [InvisibleColumn]
        public double Sym2LastMinActionPrice { get; set; }

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

        private double[,] InitLotArray(double initialLot)
        {
            var lots = new double[120, 2];
            lots[0, 0] = CheckLot(initialLot);
            for (int i = 1; i < 120; i++)
            {
                lots[i, 0] = CheckLot(lots[i - 1, 0] + 2 * lots[0, 0]);
            }
            for (int i = 0; i < 120; i++)
            {
                lots[i, 1] = CheckLot(lots[i, 0] * M);
            }
            return lots;
        }

        public double CheckLot(double lot)
        {
            decimal d = Convert.ToDecimal(lot);
            d = Math.Round(d, 2, MidpointRounding.AwayFromZero);
            lot = Convert.ToDouble(d);
            if (lot < 0.01)
            {
                lot = 0.01;
            }
            return lot;
        }
    }
}

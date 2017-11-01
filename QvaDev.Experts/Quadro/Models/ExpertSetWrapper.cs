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
        public ExpertSet ExpertSet { get; }

        public ExpertSetWrapper(ExpertSet expertSet)
        {
            ExpertSet = expertSet;
        }

        public Connector Connector => ExpertSet.TradingAccount.MetaTraderAccount.Connector as Connector;
        public string Symbol1 => ExpertSet.Symbol1;
        public string Symbol2 => ExpertSet.Symbol2;
        public bool BaseTradesForPositiveClose => ExpertSet.BaseTradesForPositiveClose;
        public bool HedgeTradeForPositiveClose => ExpertSet.HedgeTradeForPositiveClose;
        public double M => ExpertSet.M;
        public int SpreadBuyMagicNumber => ExpertSet.MagicNumber;
        public int SpreadSellMagicNumber => ExpertSet.MagicNumber + 1;
        public int HedgeBuyHedgicNumber => ExpertSet.HedgicNumber + 9;
        public int HedgeSellHedgicNumber => ExpertSet.HedgicNumber + 10;

        public List<Bar> BarHistory1 { get; set; }
        public List<Bar> BarHistory2 { get; set; }
        public List<double> Quants { get; set; }

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

        public bool TradeOpeningEnabled { get; set; } = true;

        public double[,] BuyLots => InitLotArray(ExpertSet.LotSize);
        public double[,] SellLots => InitLotArray(ExpertSet.LotSize);
        public double[,] InitialLots => InitLotArray(ExpertSet.LotSize);

        public double Sym1LastMaxActionPrice { get; set; }
        public double Sym1LastMinActionPrice { get; set; }
        public double Sym2LastMaxActionPrice { get; set; }
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

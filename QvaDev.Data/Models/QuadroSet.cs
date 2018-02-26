﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class QuadroSet : BaseDescriptionEntity, IFilterableEntity
    {
        public enum TradeSetStates
        {
            NoTrade,
            TradeOpened,
            AfterFirstClose,
            AfterSecondClose
        }

        public enum TimeFrames
        {
            M1 = 1,
            M5 = 5,
            M15 = 15,
            H1 = 60
        }

        public enum Variants
        {
            NormalNormalBase = 1,
            NormalNormalReversed = 2,
            NormalInverseBase = 3,
            NormalInverseReversed = 4,
            InverseNormalBase = 5,
            InverseNormalReversed = 6
        }

        public bool ShouldRun { get; set; }
        public bool TradeOpeningEnabled { get; set; }

        public TimeFrames TimeFrame { get; set; }
        public string Symbol1 { get; set; }
        public string Symbol2 { get; set; }

        public Variants Variant { get; set; }

        public int Diff { get; set; }
        public int Period { get; set; }
        public double LotSize { get; set; }

        public int Tp1 { get; set; }

        public int ReOpenDiff { get; set; }
        public int ReOpenDiffChangeCount { get; set; }
        public int ReOpenDiff2 { get; set; }
        public int Delta { get; set; }
        public double M { get; set; }

        public int MagicNumber { get; set; }

        public int MaxTradeSetCount { get; set; }
        public int Last24HMaxOpen { get; set; }

        [NotMapped] public bool CloseAllBuy { get; set; }
        [NotMapped] public bool CloseAllSell { get; set; }
        [NotMapped] public bool BisectingCloseBuy { get; set; }
        [NotMapped] public bool BisectingCloseSell { get; set; }
        public bool ProfitCloseBuy { get; set; }
        public double ProfitCloseValueBuy { get; set; }
        public bool ProfitCloseSell { get; set; }
        public double ProfitCloseValueSell { get; set; }

        [NotMapped] public bool SyncBuyState { get; set; }
        public TradeSetStates CurrentBuyState { get; set; }
        public int BuyOpenCount { get; set; }
        public double Sym1LastMinActionPrice { get; set; }
        public double Sym2LastMinActionPrice { get; set; }
        [NotMapped] public bool SyncSellState { get; set; }
        public TradeSetStates CurrentSellState { get; set; }
        public int SellOpenCount { get; set; }
        public double Sym1LastMaxActionPrice { get; set; }
        public double Sym2LastMaxActionPrice { get; set; }

        [Required] public int TradingAccountId { get; set; }
        [Required] public TradingAccount TradingAccount { get; set; }

        [NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }

        public int GetMaxBarCount()
        {
            var barCounts = new List<int>
            {
                StochMultiplication * StochMultiplier1,
                StochMultiplication * StochMultiplier2,
                StochMultiplication * StochMultiplier3,
                WprMultiplication * WprMultiplier1,
                WprMultiplication * WprMultiplier2,
                WprMultiplication * WprMultiplier3
            };
            return (int) (barCounts.Max() * 1.1);
        }

        #region Unused

        public enum HedgeModes
        {
            NoHedge,
            TwoPairHedge,
            ThirdPairHedge,
            ThreePairHedge
        }

        [NotMapped]
        [InvisibleColumn]
        public bool PartialClose = false;
        [NotMapped]
        [InvisibleColumn]
        public int Tp2 => 0;
        [NotMapped]
        [InvisibleColumn]
        public int Tp3 => 0;

        [NotMapped]
        [InvisibleColumn]
        public bool HedgeProfitClose { get; set; }
        [NotMapped]
        [InvisibleColumn]
        public double HedgeProfitStop { get; set; }
        [NotMapped]
        [InvisibleColumn]
        public double HedgeLossStop { get; set; }

        [NotMapped]
        [InvisibleColumn]
        public double HedgeRatio => 1;
        [NotMapped]
        [InvisibleColumn]
        public int HedgeStart => 1;
        [NotMapped]
        [InvisibleColumn]
        public int HedgeStop => 3;

        [NotMapped]
        [InvisibleColumn]
        public bool BaseTradesForPositiveClose => true;
        [NotMapped]
        [InvisibleColumn]
        public bool HedgeTradeForPositiveClose = false;

        [NotMapped]
        [InvisibleColumn]
        public int HedgeStopPositionCount => 3;

        //public double ExposureShieldValue { get; set; }
        [NotMapped]
        [InvisibleColumn]
        public bool UseTradeSetStopLoss => false;
        [NotMapped]
        [InvisibleColumn]
        public double TradeSetStopLossValue => -50000;

        [NotMapped]
        [InvisibleColumn]
        public int StochMultiplication => Period;
        [NotMapped]
        [InvisibleColumn]
        public int StochMultiplier1 => 3;
        [NotMapped]
        [InvisibleColumn]
        public int StochMultiplier2 => 6;
        [NotMapped]
        [InvisibleColumn]
        public int StochMultiplier3 => 12;
        [NotMapped]
        [InvisibleColumn]
        public int WprMultiplication => Period;
        [NotMapped]
        [InvisibleColumn]
        public int WprMultiplier1 => 3;
        [NotMapped]
        [InvisibleColumn]
        public int WprMultiplier2 => 6;
        [NotMapped]
        [InvisibleColumn]
        public int WprMultiplier3 => 12;

        #endregion
    }
}
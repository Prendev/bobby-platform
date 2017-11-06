using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class ExpertSet : BaseEntity, IFilterableEntity
    {
        public enum Variants
        {
            NormalNormalBase = 1,
            NormalNormalReversed = 2,
            NormalInverseBase = 3,
            NormalInverseReversed = 4,
            InverseNormalBase = 5,
            InverseNormalReversed = 6
        }

        public enum HedgeModes
        {
            NoHedge,
            TwoPairHedge,
            ThirdPairHedge,
            ThreePairHedge
        }

        [Required]
        public int TradingAccountId { get; set; }
        [Required]
        public TradingAccount TradingAccount { get; set; }

        [Required]
        public string Symbol1 { get; set; }
        [Required]
        public string Symbol2 { get; set; }

        public double M { get; set; }
        public int Diff { get; set; }
        public int Delta { get; set; }

        public int StochMultiplication { get; set; }
        public int StochMultiplier1 { get; set; }
        public int StochMultiplier2 { get; set; }
        public int StochMultiplier3 { get; set; }

        public int WprMultiplication { get; set; }
        public int WprMultiplier1 { get; set; }
        public int WprMultiplier2 { get; set; }
        public int WprMultiplier3 { get; set; }

        public int MagicNumber { get; set; }
        public int HedgicNumber { get; set; }

        public double LotSize { get; set; }
        public bool BaseTradesForPositiveClose { get; set; }
        public bool HedgeTradeForPositiveClose { get; set; }
        public bool PartialClose { get; set; }

        public int Tp1 { get; set; }
        public int Tp2 { get; set; }
        public int Tp3 { get; set; }

        public Variants Variant { get; set; }
        public HedgeModes HedgeMode { get; set; }
        public int MaxTradeSetCount { get; set; }
        public int Last24HMaxOpen { get; set; }
        public int HedgeStopPositionCount { get; set; }

        public int ReOpenDiffChangeCount { get; set; }
        public int ReOpenDiff { get; set; }
        public int ReOpenDiff2 { get; set; }

        public double ExposureShieldValue { get; set; }

        [NotMapped]
        [InvisibleColumn]
        public bool IsFiltered { get => Get<bool>(); set => Set(value); }
    }
}

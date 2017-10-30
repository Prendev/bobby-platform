using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
    public class ExpertSet : BaseEntity, IFilterableEntity
    {
        [Required]
        public int TradingAccountId { get; set; }
        [Required]
        public TradingAccount TradingAccount { get; set; }

        [Required]
        public string Symbol1 { get; set; }
        [Required]
        public string Symbol2 { get; set; }

        public double M { get; set; }

        public int StochMultiplication { get; set; }
        public int StochMultiplier1 { get; set; }
        public int StochMultiplier2 { get; set; }
        public int StochMultiplier3 { get; set; }

        public int WprMultiplication { get; set; }
        public int WprMultiplier1 { get; set; }
        public int WprMultiplier2 { get; set; }
        public int WprMultiplier3 { get; set; }

        public int Diff { get; set; }

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
    }
}

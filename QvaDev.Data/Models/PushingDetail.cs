using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class PushingDetail : BaseEntity, IFilterableEntity
    {
        [NotMapped]
        public double? PriceLimit { get => Get<double?>(); set => Set(value); }
        public int SmallContractSize { get; set; }
        public int BigContractSize { get; set; }
        public int BigPercentage { get; set; }
        public int FutureOpenDelayInMs { get; set; }
        public int MinIntervalInMs { get; set; }
        public int MaxIntervalInMs { get; set; }
        public int HedgeSignalContractLimit { get; set; }
        public int MasterSignalContractLimit { get; set; }
        public int FullContractSize { get; set; }
        public double MasterLots { get; set; }
        public double HedgeLots { get; set; }

        [NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
    }
}

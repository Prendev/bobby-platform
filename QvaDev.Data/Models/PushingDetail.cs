using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace QvaDev.Data.Models
{
    public class PushingDetail : BaseEntity
	{
		[NotMapped] public decimal OpenedFutures { get => Get<decimal>(); set => Set(value); }
		[NotMapped] public decimal? PriceLimit { get => Get<decimal?>(); set => Set(value); }

	    [DisplayName("Small")] public int SmallContractSize { get; set; }
	    [DisplayName("Big")] public int BigContractSize { get; set; }
        public int BigPercentage { get; set; }
		[DisplayName("PullContract")] public int PullContractSize { get; set; }
		[DisplayName("FutureOpenDelay")] public int FutureOpenDelayInMs { get; set; }
	    [DisplayName("MinInterval")] public int MinIntervalInMs { get; set; }
	    [DisplayName("MaxInterval")] public int MaxIntervalInMs { get; set; }
		[DisplayName("HedgeSignal")] public int HedgeSignalContractLimit { get; set; }
		[DisplayName("MasterSignal")] public int MasterSignalContractLimit { get; set; }
		[DisplayName("EndingMinInterval")] public int EndingMinIntervalInMs { get; set; }
		[DisplayName("EndingMaxInterval")] public int EndingMaxIntervalInMs { get; set; }
		[DisplayName("FullContract")] public int FullContractSize { get; set; }
		[DisplayName("Close %")] public int PartialClosePercentage { get; set; }
		public double AlphaLots { get; set; }
	    public double BetaLots { get; set; }
		public double HedgeLots { get; set; }
		public int MaxRetryCount { get; set; }
	    [DisplayName("RetryPeriod")] public int RetryPeriodInMs { get; set; }
    }
}

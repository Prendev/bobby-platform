using System;
using System.ComponentModel.DataAnnotations;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public class StratDealingArb : BaseDescriptionEntity
	{
		public int ProfileId { get; set; }
		public Profile Profile { get; set; }

		public int ContractSize { get; set; }
		public double Lots { get; set; }

		public double SignalDiffInPip { get; set; }
		public double SignalStepInPip { get; set; }
		public double TargetInPip { get; set; }
		public double PipSize { get; set; }

		public int MinOpenTimeInMinutes { get; set; }

		public TimeSpan EarliestOpenTime { get; set; }
		public TimeSpan LatestOpenTime { get; set; }
		public TimeSpan LatestCloseTime { get; set; }

		public int FtAccountId { get; set; }
		public FixTraderAccount FtAccount { get; set; }
		[Required]
		public string FtSymbol { get; set; }

		public int MtAccountId { get; set; }
		public MetaTraderAccount MtAccount { get; set; }
		[Required]
		public string MtSymbol { get; set; }

		public int MagicNumber { get; set; }

		public int MaxRetryCount { get; set; }
		public int RetryPeriodInMilliseconds { get; set; }
	}
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public class StratDealingArb : BaseDescriptionEntity
	{
		public int ProfileId { get; set; }
		public Profile Profile { get; set; }

		public int ContractSize { get; set; }
		public double Lots { get; set; }

		public int MaxNumberOfPositions { get; set; }

		public decimal SignalDiffInPip { get; set; }
		public decimal SignalStepInPip { get; set; }
		public decimal TargetInPip { get; set; }

		public int MinOpenTimeInMinutes { get; set; }

		public TimeSpan EarliestOpenTime { get; set; }
		public TimeSpan LatestOpenTime { get; set; }
		public TimeSpan LatestCloseTime { get; set; }

		public int FtAccountId { get; set; }
		public Account FtAccount { get; set; }
		[Required] public string FtSymbol { get; set; }

		public int MtAccountId { get; set; }
		public Account MtAccount { get; set; }
		[Required] public string MtSymbol { get; set; }

		public decimal? ShiftInPip { get; set; }
		public TimeSpan ShiftCalcInterval { get; set; }

		public decimal PipSize { get; set; }
		public int MagicNumber { get; set; }

		public int MaxRetryCount { get; set; }
		public int RetryPeriodInMilliseconds { get; set; }

		[NotMapped] [InvisibleColumn] public int ShiftTickCount { get; set; }
		[NotMapped] [InvisibleColumn] public decimal ShiftDiffSumInPip { get; set; }
		[NotMapped] [InvisibleColumn] public Stopwatch ShiftCalcStopwatch { get; set; }

	}
}

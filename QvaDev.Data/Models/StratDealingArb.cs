using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
	public class StratDealingArb : BaseDescriptionEntity
	{
		public int ProfileId { get; set; }
		public Profile Profile { get; set; }

		public int ContractSize { get; set; }
		public double Lots { get; set; }

		public int MaxNumberOfPositions { get; set; }

		public double SignalDiffInPip { get; set; }
		public double SignalStepInPip { get; set; }
		public double TargetInPip { get; set; }

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

		public double? ShiftInPip { get; set; }
		public TimeSpan ShiftCalcInterval { get; set; }

		public double PipSize { get; set; }
		public int MagicNumber { get; set; }

		public int MaxRetryCount { get; set; }
		public int RetryPeriodInMilliseconds { get; set; }

		[NotMapped] [InvisibleColumn] public int ShiftTickCount { get; set; }
		[NotMapped] [InvisibleColumn] public double ShiftDiffSumInPip { get; set; }
		[NotMapped] [InvisibleColumn] public Stopwatch ShiftCalcStopwatch { get; set; }

	}
}

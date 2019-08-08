﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class LatencyArb : BaseDescriptionEntity
	{
		public enum LatencyArbStates
		{
			None,
			Opening,
			ReopeningShort,
			ReopeningLong,
			Closing,
			Sync,
			Reset,
			Error
		}

		public enum LatencyArbFirstSides
		{
			Any,
			Short,
			Long
		}

		public enum LatencyArbOrderTypes
		{
			Market,
			Aggressive
		}

		[DisplayPriority(-1)] public bool Run { get; set; }

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public LatencyArbStates State { get => Get<LatencyArbStates>(); set => Set(value); }
		public bool Rotating { get; set; }
		[DisplayName("AvgPeriod")] public int AveragingPeriodInSeconds { get; set; }
		[DisplayName("ReopenDaysOld")] public int ReopenThresholdInDay { get; set; } = 5;
		[DisplayName("R Count")] public int ReopenCount { get => Get<int>(); set => Set(value); }

		public int EmergencyOff { get; set; }
		[DisplayName("E Open tr.")] public decimal EmergencyOpenThresholdInPip { get; set; }
		[DisplayName("E Close tr.")] public decimal EmergencyCloseThresholdInPip { get; set; }
		[DisplayName("E Count")] public int EmergencyCount { get => Get<int>(); set => Set(value); }

		public int FastFeedAccountId { get; set; }
		public Account FastFeedAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("FF Sym")] [Required] public string FastFeedSymbol { get; set; }

		public int ShortAccountId { get; set; }
		public Account ShortAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("S Sym")] [Required] public string ShortSymbol { get; set; }
		[DisplayName("S Size")] public decimal ShortSize { get; set; } = 1;
		[DisplayName("S Spread")] public decimal ShortSpreadFilterInPip { get; set; }
		[DisplayName("S Signal")] public decimal ShortSignalDiffInPip { get; set; }

		public int LongAccountId { get; set; }
		public Account LongAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("L Sym")] [Required] public string LongSymbol { get; set; }
		[DisplayName("L Size")] public decimal LongSize { get; set; } = 1;
		[DisplayName("L Spread")] public decimal LongSpreadFilterInPip { get; set; }
		[DisplayName("L Signal")] public decimal LongSignalDiffInPip { get; set; }

		public LatencyArbFirstSides FirstSide { get => Get<LatencyArbFirstSides>(); set => Set(value); }
		public int MaxCount { get; set; } = 5;
		[DisplayName("Trail dist.")] public decimal TrailingDistanceInPip { get; set; }
		[DisplayName("Trail switch")] public decimal TrailingSwitchInPip { get; set; }
		[DisplayName("SL")] public decimal SlInPip { get; set; }
		[DisplayName("TP")] public decimal TpInPip { get; set; }

		[DisplayName("Earliest")] public TimeSpan? EarliestTradeTime { get; set; }
		[DisplayName("Latest")] public TimeSpan? LatestTradeTime { get; set; }

		public LatencyArbOrderTypes FirstOrderType { get; set; }
		public LatencyArbOrderTypes HedgeOrderType { get; set; }
		[DisplayName("MaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("RetryPeriod")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("Slippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("Correction")]
		public decimal CorrectionInPip { get; set; }
		[DisplayName("TimeWindow")]
		public int TimeWindowInMs { get; set; }

		public decimal PipSize { get; set; }
		public string Comment { get; set; }

		public int? CopierId { get; set; }
		public Copier Copier { get; set; }
		public int? FixApiCopierId { get; set; }
		public FixApiCopier FixApiCopier { get; set; }

		public List<LatencyArbPosition> LatencyArbPositions { get; } = new List<LatencyArbPosition>();
	}
}

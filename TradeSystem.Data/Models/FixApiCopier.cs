using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class FixApiCopier : BaseEntity
	{
		public enum FixApiCopierModes
		{
			Both,
			CloseOnly,
			OpenOnly
		}
		public enum FixApiOrderTypes
		{
			Market,
			Aggressive,
			GtcLimit
		}
		public enum BasePriceTypes
		{
			Slave,
			Master
		}

		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }
		public FixApiCopierModes Mode { get; set; }
		public bool ValueCopy { get; set; }
		[DisplayName("Ratio/Value")] public decimal CopyRatio { get; set; }
		public FixApiOrderTypes OrderType { get; set; } = FixApiOrderTypes.GtcLimit;
		public BasePriceTypes BasePriceType { get; set; }
		[DisplayName("Delay")] public int DelayInMilliseconds { get; set; }
		[DisplayName("Spread")] public decimal SpreadFilterInPips { get; set; }

		[DisplayName("FallbackMarketOpen")] public bool FallbackToMarketOrderType { get; set; }
		[DisplayName("F Retry")] public int FallbackMaxRetryCount { get; set; }
		[DisplayName("F Period")] public int FallbackRetryPeriodInMs { get; set; }
		[DisplayName("F Window")] public int FallbackTimeWindowInMs { get; set; }

		[DisplayName("MarketRetry")] public int MarketMaxRetryCount { get; set; } = 5;
		[DisplayName("M Period")] public int MarketRetryPeriodInMs { get; set; } = 25;
		[DisplayName("M Window")] public int MarketTimeWindowInMs { get; set; } = 5000;

		[DisplayName("LimitRetry")] public int MaxRetryCount { get; set; } = 10;
		[DisplayName("L Period")] public int RetryPeriodInMs { get; set; } = 25;
		[DisplayName("L Window")] public int TimeWindowInMs { get; set; } = 1000;
		[DisplayName("L Slippage")] public decimal SlippageInPip { get; set; } = 2;
		[DisplayName("L Diff")] public decimal LimitDiffInPip { get; set; }

		public decimal PipSize { get; set; } = 1;
		public string Comment { get; set; }

		public List<FixApiCopierPosition> FixApiCopierPositions { get; } = new List<FixApiCopierPosition>();

		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;
		[NotMapped] [InvisibleColumn] public decimal LimitDiff => LimitDiffInPip * PipSize;
		[NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }

		public override string ToString()
		{
			return $"{(Id == 0 ? "UNSAVED - " : "")}{Slave} - {Id}";
		}
	}
}

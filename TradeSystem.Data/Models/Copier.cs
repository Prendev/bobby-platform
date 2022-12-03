using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class Copier : BaseEntity
	{
		public enum CopierModes
		{
			Both,
			CloseOnly,
			OpenOnly
		}
		public enum CopierOrderTypes
		{
			Market,
			MarketRange,
			Hedge
		}
		public enum BasePriceTypes
		{
			Slave,
			Master
		}

		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }
		public CopierModes Mode { get; set; }
		public bool ValueCopy { get; set; }
		[DisplayName("Ratio/Value")] public decimal CopyRatio { get; set; }
		public CopierOrderTypes OrderType { get; set; }
		public BasePriceTypes BasePriceType { get; set; }
		[DisplayName("Delay")] public int DelayInMilliseconds { get; set; }
		[DisplayName("Spread")] public decimal SpreadFilterInPips { get; set; }
		[DisplayName("Retry")] public int MaxRetryCount { get; set; } = 5;
		[DisplayName("Period")] public int RetryPeriodInMs { get; set; } = 25;
		[DisplayName("Slippage")] public int SlippageInPips { get; set; }
		public decimal PipSize { get; set; } = 1;
		[DisplayName("FilterComment")] public string Comment { get; set; }
		public string TradeComment { get; set; }

		public List<CopierPosition> CopierPositions { get; } = new List<CopierPosition>();

		public int? CrossCopierId { get; set; }
		public Copier CrossCopier { get => Get<Copier>(); set => Set(value); }
		[InverseProperty("CrossCopier")]
		public List<Copier> ParentCopiers { get; } = new List<Copier>();

		public override string ToString()
		{
			return $"{(Id == 0 ? "UNSAVED - " : "")}{Slave} - {Id}";
		}
	}
}

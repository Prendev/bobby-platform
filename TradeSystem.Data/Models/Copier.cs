using System.Collections.Generic;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class Copier : BaseEntity
	{
		public enum CopierOrderTypes
		{
			Market,
			MarketRange,
			Hedge
		}

		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }
		public bool CloseOnly { get; set; }
		public decimal CopyRatio { get; set; }
		public CopierOrderTypes OrderType { get; set; }
		public int SlippageInPips { get; set; }
        public int MaxRetryCount { get; set; }
        public int RetryPeriodInMs { get; set; }
        public int DelayInMilliseconds { get; set; }

		public List<CopierPosition> CopierPositions { get; } = new List<CopierPosition>();

		public override string ToString()
		{
			return $"{(Id == 0 ? "UNSAVED - " : "")}{Slave} - {Id}";
		}
	}
}

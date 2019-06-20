using System.Collections.Generic;
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

		[InvisibleColumn] public int SlaveId { get; set; }
		[InvisibleColumn] public Slave Slave { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }
		public CopierModes Mode { get; set; }
		public decimal CopyRatio { get; set; }
		public CopierOrderTypes OrderType { get; set; }
		public int SlippageInPips { get; set; }
		public int MaxRetryCount { get; set; } = 5;
		public int RetryPeriodInMs { get; set; } = 25;
        public int DelayInMilliseconds { get; set; }

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

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class BacktesterInstrumentConfig : BaseEntity
	{
		[InvisibleColumn] public int BacktesterAccountId { get; set; }
		[InvisibleColumn] public BacktesterAccount BacktesterAccount { get; set; }

		[Required] public string Symbol { get; set; }
		[DisplayName("Reject %")] public int RejectPercentage { get; set; }
		[DisplayName("MinSlippage")] public int MinSlippageInMs { get; set; }
		[DisplayName("MaxSlippage")] public int MaxSlippageInMs { get; set; }

		[Required] public string Folder { get; set; }
		public string DateTimeFormat { get; set; }
		public string Delimeter { get; set; }
		public int DateTimeColumn { get; set; } = 0;
		public int AskColumn { get; set; } = 1;
		public int BidColumn { get; set; } = 2;

		public string GetDateTimeFormat() =>
			String.IsNullOrWhiteSpace(DateTimeFormat) ? "yyyy.MM.dd HH:mm:ss.fff" : DateTimeFormat;
		public string GetDelimeter() =>
			String.IsNullOrWhiteSpace(Delimeter) ? ", " : Delimeter;
	}
}

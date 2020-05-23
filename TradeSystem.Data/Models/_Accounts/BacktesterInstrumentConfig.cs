using System;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class BacktesterInstrumentConfig : BaseEntity
	{
		[InvisibleColumn] public int BacktesterAccountId { get; set; }
		[InvisibleColumn] public BacktesterAccount BacktesterAccount { get; set; }

		[Required] public string Symbol { get; set; }
		[Required] public string TickerFilePath { get; set; }

		public string DateTimeFormat { get; set; }
		public string Delimeter { get; set; }
		public string Extension { get; set; }

		public string GetDateTimeFormat() =>
			String.IsNullOrWhiteSpace(DateTimeFormat) ? "yyyy/MM/dd HH:mm:ss.fff" : DateTimeFormat;
		public string GetDelimeter() =>
			String.IsNullOrWhiteSpace(Delimeter) ? ", " : Delimeter;
		public string GetExtension() =>
			String.IsNullOrWhiteSpace(Extension) ? "txt" : Extension;
	}
}

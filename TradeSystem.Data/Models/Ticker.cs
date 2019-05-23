using System;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class Ticker : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public int MainAccountId { get; set; }
		public Account MainAccount { get; set; }
		public string MainSymbol { get; set; }

		public int? PairAccountId { get; set; }
        public Account PairAccount { get; set; }
		public string PairSymbol { get; set; }

		public int MarketDepth { get; set; }

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

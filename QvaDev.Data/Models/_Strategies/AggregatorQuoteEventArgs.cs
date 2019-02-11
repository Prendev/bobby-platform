using System;
using System.Collections.Generic;
using TradeSystem.Communication;

namespace TradeSystem.Data.Models
{
	public class AggregatorQuoteEventArgs
	{
		public class Quote
		{
			public GroupQuoteEntry GroupQuoteEntry { get; set; }
			public AggregatorAccount AggAccount { get; set; }
		}

		public List<Quote> Quotes { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}

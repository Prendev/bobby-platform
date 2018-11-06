using System;
using System.Collections.Generic;

namespace QvaDev.Data.Models
{
	public class StratHubArbQuoteEventArgs
	{
		public class Quote : AggregatorQuoteEventArgs.Quote
		{
			public decimal Sum { get; set; }
		}

		public List<Quote> Quotes { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}

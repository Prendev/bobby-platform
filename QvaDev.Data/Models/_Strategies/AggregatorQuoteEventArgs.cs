using System.Collections.Generic;
using QvaDev.Communication.FixApi;

namespace QvaDev.Data.Models
{
	public class AggregatorQuoteEventArgs
	{
		public class Quote
		{
			public GroupQuoteEntry GroupQuoteEntry { get; set; }
			public Account Account { get; set; }
		}

		public List<Quote> Quotes { get; set; }
	}
}

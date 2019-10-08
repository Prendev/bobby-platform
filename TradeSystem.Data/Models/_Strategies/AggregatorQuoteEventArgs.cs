using System;
using System.Collections.Generic;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;

namespace TradeSystem.Data.Models
{
	public class AggregatorQuoteEventArgs
	{
		public class Quote
		{
			public GroupQuoteEntry GroupQuoteEntry { get; set; }
			public Tick Tick { get; set; }
			public AggregatorAccount AggAccount { get; set; }

			public string Symbol => GroupQuoteEntry?.Symbol.ToString() ?? Tick?.Symbol;
			public decimal? Ask => GroupQuoteEntry?.Ask ?? Tick?.Ask;
			public decimal? Bid => GroupQuoteEntry?.Bid ?? Tick?.Bid;
			public decimal? AskVolume => GroupQuoteEntry?.AskVolume ?? Tick?.AskVolume;
			public decimal? BidVolume => GroupQuoteEntry?.BidVolume ?? Tick?.BidVolume;
		}

		public List<Quote> Quotes { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;
using TradeSystem.Communication;
using TradeSystem.Communication.Interfaces;

namespace TradeSystem.Data.Models
{
	public partial class Aggregator
	{
		public event EventHandler<AggregatorQuoteEventArgs> AggregatedQuote;

		[NotMapped] [InvisibleColumn] public IQuoteAggregator QuoteAggregator { get => Get<IQuoteAggregator>(); set => Set(value); }

		public Aggregator()
		{
			SetAction<IQuoteAggregator>(nameof(QuoteAggregator),
				a => { if (a != null) a.GroupQuote -= QuoteAggregator_GroupQuote; },
				a => { if (a != null) a.GroupQuote += QuoteAggregator_GroupQuote; });
		}

		private void QuoteAggregator_GroupQuote(object sender, GroupQuoteEventArgs e)
		{
			//Logger.Trace(cb =>
			//	cb($"QuoteAggregator_GroupQuote {e.TriggeringSymbol} {string.Join("|", e.BookTops.Select(bt => $"({bt.Ask}, {bt.Bid}, {bt.AskVolume}, {bt.BidVolume}, {bt.TimeStamp:yyyy-MM-dd HH:mm:ss.ffff})"))}"));

			var aggQuote =
				new AggregatorQuoteEventArgs {Quotes = new List<AggregatorQuoteEventArgs.Quote>(), TimeStamp = HiResDatetime.UtcNow};
			foreach (var bookTop in e.BookTops)
			{
				if (bookTop.Ask == null) continue;
				if (bookTop.Bid == null) continue;
				if (bookTop.AskVolume == null) continue;
				if (bookTop.BidVolume == null) continue;

				var account = GetAccount(bookTop.Connector);
				if (account == null) continue;

				aggQuote.Quotes.Add(new AggregatorQuoteEventArgs.Quote()
				{
					AggAccount = account,
					GroupQuoteEntry = bookTop
				});
			}

			AggregatedQuote?.Invoke(this, aggQuote);
		}

		private AggregatorAccount GetAccount(IConnector fixConnector)
		{
			foreach (var aggregatorAccount in Accounts)
			{
				if (aggregatorAccount.Account.Connector?.Is(fixConnector) != true) continue;
				return aggregatorAccount;
			}

			return null;
		}
	}
}

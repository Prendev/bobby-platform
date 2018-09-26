using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Communication.FixApi;
using QvaDev.Communication.FixApi.Interfaces;

namespace QvaDev.Data.Models
{
	public class Aggregator : BaseDescriptionEntity
	{
		public event EventHandler<AggregatorQuoteEventArgs> AggregatedQuote;

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[InvisibleColumn] public List<AggregatorAccount> Accounts { get => Get(() => new List<AggregatorAccount>()); set => Set(value, false); }

		private IQuoteAggregator _quoteAggregator;
		[NotMapped]
		[InvisibleColumn]
		public IQuoteAggregator QuoteAggregator
		{
			get => _quoteAggregator;
			set
			{
				if (_quoteAggregator != null)
					_quoteAggregator.GroupQuote -= QuoteAggregator_GroupQuote;

				if (value != null)
					value.GroupQuote += QuoteAggregator_GroupQuote;

				_quoteAggregator = value;
			}
		}

		private void QuoteAggregator_GroupQuote(object sender, GroupQuoteEventArgs e)
		{
			var aggQuote = new AggregatorQuoteEventArgs() {Quotes = new List<AggregatorQuoteEventArgs.Quote>()};
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
					Account = account,
					GroupQuoteEntry = bookTop
				});
			}

			AggregatedQuote?.Invoke(this, aggQuote);
		}

		private Account GetAccount(FixConnectorBase fixConnector)
		{
			foreach (var aggregatorAccount in Accounts)
			{
				if (!aggregatorAccount.Account.Connector.Is(fixConnector)) continue;
				return aggregatorAccount.Account;
			}

			return null;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Communication.FixApi;
using QvaDev.Communication.FixApi.Interfaces;

namespace QvaDev.Data.Models
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
			var aggQuote = new AggregatorQuoteEventArgs { Quotes = new List<AggregatorQuoteEventArgs.Quote>() };
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
				if (aggregatorAccount.Account.Connector?.Is(fixConnector) != true) continue;
				return aggregatorAccount.Account;
			}

			return null;
		}
	}
}

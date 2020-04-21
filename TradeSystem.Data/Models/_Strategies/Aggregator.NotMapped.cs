using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;
using TradeSystem.Communication.Interfaces;

namespace TradeSystem.Data.Models
{
	public partial class Aggregator
	{
		public event EventHandler<AggregatorQuoteEventArgs> AggregatedQuote;

		[NotMapped] [InvisibleColumn] public IQuoteAggregator QuoteAggregator { get => Get<IQuoteAggregator>(); set => Set(value); }

		private readonly ConcurrentDictionary<AggregatorAccount, Tick> _lastTicks =
			new ConcurrentDictionary<AggregatorAccount, Tick>();
		private readonly List<AggregatorQuoteEventArgs.Quote> _lastQuotes =
			new List<AggregatorQuoteEventArgs.Quote>();

		public Aggregator()
		{
			SetAction<IQuoteAggregator>(nameof(QuoteAggregator),
				a => { if (a != null) a.GroupQuote -= QuoteAggregator_GroupQuote; },
				a => { if (a != null) a.GroupQuote += QuoteAggregator_GroupQuote; });
		}

		public void SubscribeEvents()
		{
			foreach (var aggAccount in Accounts)
			{
				if (aggAccount.Account.MetaTraderAccount == null) continue;
				aggAccount.NewTick -= Account_NewTick;
				aggAccount.NewTick += Account_NewTick;
			}
		}

		public void UnsubscribeEvents()
		{
			foreach (var aggAccount in Accounts)
				aggAccount.NewTick -= Account_NewTick;
			_lastTicks.Clear();
		}

		private void Account_NewTick(object sender, NewTick e)
		{
			if(!(sender is AggregatorAccount account)) return;
			_lastTicks.AddOrUpdate(account, a => e.Tick, (a, tick) => tick);

			var aggQuote =
				new AggregatorQuoteEventArgs { Quotes = new List<AggregatorQuoteEventArgs.Quote>(), TimeStamp = HiResDatetime.UtcNow };

			var lastTicks = _lastTicks
				.Select(t => new AggregatorQuoteEventArgs.Quote() {AggAccount = t.Key, Tick = t.Value}).ToList();
			aggQuote.Quotes.AddRange(lastTicks);
			lock (_lastQuotes) aggQuote.Quotes.AddRange(_lastQuotes);
			AggregatedQuote?.Invoke(this, aggQuote);
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
				lock (_lastQuotes)
				{
					_lastQuotes.Clear();
					_lastQuotes.AddRange(aggQuote.Quotes);
				}
			}

			var lastTicks = _lastTicks
				.Select(t => new AggregatorQuoteEventArgs.Quote() { AggAccount = t.Key, Tick = t.Value }).ToList();
			aggQuote.Quotes.AddRange(lastTicks);
			AggregatedQuote?.Invoke(this, aggQuote);
		}

		private AggregatorAccount GetAccount(object connector)
		{
			foreach (var aggregatorAccount in Accounts)
			{
				if (aggregatorAccount.Account.Connector?.Is(connector) != true) continue;
				return aggregatorAccount;
			}

			return null;
		}
	}
}

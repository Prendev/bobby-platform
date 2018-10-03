﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public partial class StratHubArb
	{
		public event EventHandler<StratHubArbQuoteEventArgs> ArbQuote;

		[NotMapped]
		[InvisibleColumn]
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}
		private volatile bool _isBusy;

		[NotMapped] [InvisibleColumn] public bool HasTiming => EarliestOpenTime.HasValue && LatestOpenTime.HasValue && LatestCloseTime.HasValue;
		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;

		public StratHubArb()
		{
			SetAction<Aggregator>(nameof(Aggregator),
				a => { if (a != null) a.AggregatedQuote -= Aggregator_AggregatedQuote; },
				a => { if (a != null) a.AggregatedQuote += Aggregator_AggregatedQuote; });
		}

		public IList CalculateStatistics()
		{
			var buys = StratHubArbPositions.Where(e => e.Position.Side == StratPosition.Sides.Buy).ToList();
			var sells = StratHubArbPositions.Where(e => e.Position.Side == StratPosition.Sides.Sell).ToList();

			var buyTotal = buys.Sum(p => p.Position.Size);
			var buyAvg = buys.Sum(p => p.Position.Size * p.Position.AvgPrice);
			if (buyTotal > 0) buyAvg /= buyTotal;

			var sellTotal = sells.Sum(p => p.Position.Size);
			var sellAvg = sells.Sum(p => p.Position.Size * p.Position.AvgPrice);
			if (sellTotal > 0) sellAvg /= sellTotal;



			var stat = new Dictionary<string, decimal>
			{
				["BuyTotal"] = buyTotal,
				["BuyAvg"] = buyAvg,
				["SellTotal"] = sellTotal,
				["SellAvg"] = sellAvg,
				["Total"] = buyTotal - sellTotal,
				["Pip"] = (sellAvg - buyAvg) / PipSize,
			};

			return stat.Select(v => new {Name = v.Key, Value = v.Value}).ToList();
		}

		private void Aggregator_AggregatedQuote(object sender, AggregatorQuoteEventArgs e)
		{
			var arbQuote = new StratHubArbQuoteEventArgs() { Quotes = new List<StratHubArbQuoteEventArgs.Quote>() };
			foreach (var aggQuote in e.Quotes)
			{
				arbQuote.Quotes.Add(new StratHubArbQuoteEventArgs.Quote()
				{
					GroupQuoteEntry = aggQuote.GroupQuoteEntry,
					Account = aggQuote.Account,
					Sum = PositionSum(aggQuote.Account)
				});
			}
			ArbQuote?.Invoke(this, arbQuote);
		}

		private decimal PositionSum(Account account)
		{
			return StratHubArbPositions?.Where(e => e.Position.AccountId == account.Id).Sum(e => e.Position.SignedSize) ?? 0;
		}
	}
}

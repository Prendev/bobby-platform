using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public partial class StratHubArb
	{
		public class Statistics
		{
			public Account Account { get; set; }
			public decimal SellTotal { get; set; }
			public decimal BuyTotal { get; set; }
			public decimal SellAvg { get; set; }
			public decimal BuyAvg { get; set; }
			public decimal Total { get; set; }
			public decimal Pip { get; set; }
		}

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

			var statistics = new List<Statistics>
			{
				new Statistics()
				{
					SellTotal = sellTotal,
					BuyTotal = buyTotal,
					SellAvg = sellAvg,
					BuyAvg = buyAvg,
					Total = buyTotal - sellTotal,
					Pip = (sellAvg - buyAvg) / PipSize,
				}
			};

			var accounts = StratHubArbPositions
				.GroupBy(p => p.Position.Account)
				.OrderBy(x => x.Key.ToString())
				.ToList();

			foreach (var account in accounts)
			{
				var stat = new Statistics()
				{
					Account = account.Key,
					SellTotal = account.Where(e => e.Position.Side == StratPosition.Sides.Sell).Sum(p => p.Position.Size),
					BuyTotal = account.Where(e => e.Position.Side == StratPosition.Sides.Buy).Sum(p => p.Position.Size),
					SellAvg = account.Where(e => e.Position.Side == StratPosition.Sides.Sell)
						.Sum(p => p.Position.Size * p.Position.AvgPrice),
					BuyAvg = account.Where(e => e.Position.Side == StratPosition.Sides.Buy)
						.Sum(p => p.Position.Size * p.Position.AvgPrice),
				};
				if (stat.SellTotal > 0) stat.SellAvg /= stat.SellTotal;
				if (stat.BuyTotal > 0) stat.BuyAvg /= stat.BuyTotal;
				stat.Total = stat.BuyTotal - stat.SellTotal;
				stat.Pip = (stat.SellAvg - stat.BuyAvg) / PipSize;

				statistics.Add(stat);
			}

			return statistics.Select(s => new
			{
				Account = s.Account?.ToString() ?? "*",
				SellTotal = s.SellTotal.ToString("0"),
				BuyTotal = s.BuyTotal.ToString("0"),
				SellAvg = s.SellAvg.ToString("F5"),
				BuyAvg = s.BuyAvg.ToString("F5"),
				Total = s.Total.ToString("0"),
				Pip = s.Pip.ToString("F2"),
			}).ToList();
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

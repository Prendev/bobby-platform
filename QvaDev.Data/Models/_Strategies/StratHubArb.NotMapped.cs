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
			public decimal ClosedPip { get; set; }
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
		[NotMapped] [InvisibleColumn] public decimal Correction => CorrectionInPip * PipSize;

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

			var sumStat = new Statistics()
			{
				SellTotal = sellTotal,
				BuyTotal = buyTotal,
				SellAvg = sellAvg,
				BuyAvg = buyAvg,
				Total = buyTotal - sellTotal,
				Pip = (sellAvg - buyAvg) / PipSize,
			};

			var statistics = new List<Statistics>();

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
				statistics.Add(stat);

				if (stat.SellTotal > 0) stat.SellAvg /= stat.SellTotal;
				if (stat.BuyTotal > 0) stat.BuyAvg /= stat.BuyTotal;
				stat.Total = stat.BuyTotal - stat.SellTotal;

				// Calculate non valid pip with exposure inbalance
				stat.ClosedPip = (stat.SellAvg - stat.BuyAvg) / PipSize;
				if (stat.Total == 0) continue;

				// Calculate ClosedPip if there is inbalance
				var positions = stat.Total > 0
					? account.Where(e => e.Position.Side == StratPosition.Sides.Buy).OrderByDescending(p => p.PositionId)
					: account.Where(e => e.Position.Side == StratPosition.Sides.Sell).OrderByDescending(p => p.PositionId);

				var total = stat.Total > 0 ? stat.BuyTotal : stat.SellTotal;
				var avg = stat.Total > 0 ? stat.BuyAvg : stat.SellAvg;
				var diff = Math.Abs(stat.Total);
				foreach (var pos in positions)
				{
					avg = avg * total;
					var size = Math.Min(diff, pos.Position.Size);
					avg -= pos.Position.AvgPrice * size;
					total -= size;
					if (total > 0) avg /= total;
					diff -= size;
					if (diff == 0) break;
				}
				stat.ClosedPip = stat.Total > 0 ? (stat.SellAvg - avg) / PipSize : (avg - stat.BuyAvg) / PipSize;
			}

			// Calculate Sum ClosedPip
			sumStat.ClosedPip = statistics.Sum(s => s.ClosedPip * Math.Min(s.BuyTotal, s.SellTotal));
			if (sumStat.ClosedPip != 0) sumStat.ClosedPip /= statistics.Sum(s => Math.Min(s.BuyTotal, s.SellTotal));
			statistics.Insert(0, sumStat);

			return statistics.Select(s => new
			{
				Account = s.Account?.ToString() ?? "*",
				SellTotal = s.SellTotal.ToString("0"),
				BuyTotal = s.BuyTotal.ToString("0"),
				SellAvg = s.SellAvg.ToString("F5"),
				BuyAvg = s.BuyAvg.ToString("F5"),
				Total = s.Total.ToString("0"),
				ClosedPip = s.ClosedPip.ToString("F2"),
				Pip = s.Pip == 0 ? "" : s.Pip.ToString("F2")
			}).ToList();
		}

		private void Aggregator_AggregatedQuote(object sender, AggregatorQuoteEventArgs e)
		{
			var arbQuote =
				new StratHubArbQuoteEventArgs() {Quotes = new List<StratHubArbQuoteEventArgs.Quote>(), TimeStamp = e.TimeStamp};
			foreach (var aggQuote in e.Quotes)
			{
				arbQuote.Quotes.Add(new StratHubArbQuoteEventArgs.Quote()
				{
					GroupQuoteEntry = aggQuote.GroupQuoteEntry,
					AggAccount = aggQuote.AggAccount,
					Sum = PositionSum(aggQuote.AggAccount.Account)
				});
			}
			ArbQuote?.Invoke(this, arbQuote);
		}

		private decimal PositionSum(Account account)
		{
			if (account?.StratHubArbPositions == null) return 0;
			lock (account.StratHubArbPositions)
				return account.StratHubArbPositions.Where(e => e.StratHubArbId == Id).Sum(e => e.Position.SignedSize);
		}
	}
}

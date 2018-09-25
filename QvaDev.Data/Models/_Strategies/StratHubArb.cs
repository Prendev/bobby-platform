using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;
using QvaDev.Communication.FixApi;

namespace QvaDev.Data.Models
{
	public class StratHubArb : BaseDescriptionEntity
	{
		public enum StratHubArbOrderTypes
		{
			Market,
			Aggressive
		}

		public event EventHandler<StratHubArbQuoteEventArgs> ArbQuote;

		[InvisibleColumn] public int AggregatorId { get; set; }
		private Aggregator _aggregator;
		[InvisibleColumn]
		public Aggregator Aggregator
		{
			get => _aggregator;
			set
			{
				if (_aggregator != null)
					_aggregator.AggregatedQuote -= Aggregator_AggregatedQuote;

				if (value != null)
					value.AggregatedQuote += Aggregator_AggregatedQuote; ;

				_aggregator = value;
			}
		}

		public bool Run { get => Get<bool>(); set => Set(value); }

		public decimal Size { get; set; }
		[DisplayName("MaxSize")]
		public decimal MaxSizePerAccount { get; set; }
		public decimal PipSize { get; set; }

		[DisplayName("SignalDiff")]
		public decimal SignalDiffInPip { get; set; }
		[DisplayName("MinOpenTime")]
		public int MinOpenTimeInMinutes { get; set; }

		[InvisibleColumn] public TimeSpan? EarliestOpenTime { get; set; }
		[InvisibleColumn] public TimeSpan? LatestOpenTime { get; set; }
		[InvisibleColumn] public TimeSpan? LatestCloseTime { get; set; }

		public StratHubArbOrderTypes OrderType { get; set; }
		[DisplayName("MaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("RetryPeriod")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("Slippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("TimeWindow")]
		public int TimeWindowInMs { get; set; }

		[NotMapped] [InvisibleColumn] public bool HasTiming => EarliestOpenTime.HasValue && LatestOpenTime.HasValue && LatestCloseTime.HasValue;
		[NotMapped] [InvisibleColumn] public decimal Deviation => SlippageInPip * PipSize;

		[NotMapped]
		[InvisibleColumn]
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}
		private volatile bool _isBusy;

		[InvisibleColumn] public List<StratHubArbPosition> StratHubArbPositions { get => Get(() => new List<StratHubArbPosition>()); set => Set(value, false); }
		[InvisibleColumn] public DateTime? LastOpenTime { get => Get<DateTime?>(); set => Set(value); }

		private void Aggregator_AggregatedQuote(object sender, AggregatorQuoteEventArgs e)
		{
			var arbQuote = new StratHubArbQuoteEventArgs() {Quotes = new List<StratHubArbQuoteEventArgs.Quote>()};
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

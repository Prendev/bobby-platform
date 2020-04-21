using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Collections;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Common.Services;
using TradeSystem.Data.Models;
using static TradeSystem.Data.Models.StratHubArbQuoteEventArgs;
using OrderTypes = TradeSystem.Data.Models.StratHubArb.StratHubArbOrderTypes;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface IHubArbService
	{
		void Start(List<StratHubArb> arbs);
		void Stop();
		Task GoFlat(List<StratHubArb> arbs);
		Task GoFlat(StratHubArb arb);
	}

	public partial class HubArbService : IHubArbService
	{
		private volatile CancellationTokenSource _cancellation;
		private CustomThreadPool<OrderResponse> _orderPool;

		private readonly INewsCalendarService _newsCalendarService;
		private List<StratHubArb> _arbs;
		private readonly object _syncRoot = new object();

		private readonly ConcurrentDictionary<int, FastBlockingCollection<Action>> _arbQueues =
			new ConcurrentDictionary<int, FastBlockingCollection<Action>>();

		public HubArbService(INewsCalendarService newsCalendarService)
		{
			_newsCalendarService = newsCalendarService;
		}

		public void Start(List<StratHubArb> arbs)
		{
			_cancellation?.Dispose();

			_arbs = arbs;
			_cancellation = new CancellationTokenSource();

			var threadCount = _arbs.SelectMany(a => a.Aggregator.Accounts).Distinct().Count();
			_orderPool = new CustomThreadPool<OrderResponse>(threadCount, "ArbOrderPool", _cancellation.Token);

			foreach (var arb in _arbs)
			{
				arb.ArbQuote -= Arb_ArbQuote;
				new Thread(() => ArbLoop(arb, _cancellation.Token)) {Name = $"Arb_{arb.Id}", IsBackground = true}
					.Start();
				arb.ArbQuote += Arb_ArbQuote;
			}

			Logger.Info("Hub arbs are started");
		}

		public async Task GoFlat(List<StratHubArb> arbs)
		{
			Logger.Info("Hub arbs are going flat!!!");
			foreach (var arb in arbs.Where(a => a.Run)) await GoFlat(arb);
		}

		private void Arb_ArbQuote(object sender, StratHubArbQuoteEventArgs e)
		{
			if (_cancellation.IsCancellationRequested) return;
			var arb = (StratHubArb) sender;

			if (!arb.Run) return;
			if (arb.IsBusy) return;
			if (IsCloseTime(arb)) return;

			CheckOpen(arb, e);
		}

		private bool IsCloseTime(StratHubArb arb)
		{
			if (!arb.HasTiming) return false;
			if (IsTime(HiResDatetime.UtcNow.TimeOfDay, arb.EarliestOpenTime, arb.LatestCloseTime)) return false;
			GoFlat(arb);
			return true;
		}

		public async Task GoFlat(StratHubArb arb)
		{
			lock (_syncRoot)
			{
				if (arb.IsBusy) return;
				arb.IsBusy = true;
			}

			var accounts = new List<Account>();
			try
			{
				var tasks = new List<Task>();
				foreach (var aggAcc in arb.Aggregator.Accounts)
				{
					var sum = arb.StratHubArbPositions.Where(p => !p.Archived)
						.Where(p => p.Position.AccountId == aggAcc.AccountId)
						.Where(p => p.Position.Symbol == aggAcc.Symbol)
						.Sum(p => p.Position.SignedSize);

					Sides side;
					if (sum > 0) side = Sides.Sell;
					else if (sum < 0) side = Sides.Buy;
					else continue;

					if (aggAcc.Account.Connector?.IsConnected != true)
					{
						Logger.Error($"HubArbService.GoFlat {arb} - {aggAcc.Account} has exposure but disconnected!!!");
						continue;
					}

					lock (_syncRoot)
					{
						if (aggAcc.Account.IsBusy) continue;
						aggAcc.Account.IsBusy = true;
						accounts.Add(aggAcc.Account);
					}

					tasks.Add(Task.Run(async () => await SendPosition(arb, aggAcc.Account, aggAcc.Symbol, side, Math.Abs(sum), OrderTypes.Market)));
				}
				await Task.WhenAll(tasks);
			}
			finally
			{
				foreach (var account in accounts) account.IsBusy = false;
				arb.IsBusy = false;
			}
		}

		private void CheckOpen(StratHubArb arb, StratHubArbQuoteEventArgs e)
		{
			var now = HiResDatetime.UtcNow;
			if (IsTime(now.TimeOfDay, arb.LatestOpenTime, arb.EarliestOpenTime)) return;

			// No HighRiskSignalDiffInPip then no trade
			var isHighRisk = _newsCalendarService.IsHighRiskTime(now, 5);
			if (isHighRisk && !arb.HighRiskSignalDiffInPip.HasValue) return;

			//if (arb.LastOpenTime.HasValue &&
			//    (HiResDatetime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.RestingPeriodInMinutes) return;

			// Quotes where there were no orders in the last x minutes
			var bestQuotes = BestQuotes(arb, e, now);
			var buyQuote = bestQuotes?.Item1;
			var sellQuote = bestQuotes?.Item2;

			if (buyQuote == null || sellQuote == null) return;

			// During high risk different signal diff
			var diffInPip = (sellQuote.Bid - buyQuote.Ask) / arb.PipSize;
			var signalDiff = isHighRisk ? arb.HighRiskSignalDiffInPip.Value : arb.SignalDiffInPip;
			if (diffInPip < signalDiff) return;

			// Volume check
			var size = Math.Min(arb.Size, arb.MaxSizePerAccount - buyQuote.Sum);
			size = Math.Min(size, sellQuote.Sum + arb.MaxSizePerAccount);
			if ((buyQuote.AskVolume ?? 0) > 0) size = Math.Min(size, buyQuote.AskVolume ?? 0);
			if ((sellQuote.BidVolume ?? 0) > 0) size = Math.Min(size, sellQuote.BidVolume ?? 0);
			if (size <= 0) return;

			// Is busy?
			lock (_syncRoot)
			{
				if (arb.IsBusy) return;
				if (buyQuote.AggAccount.Account.IsBusy) return;
				if (sellQuote.AggAccount.Account.IsBusy) return;
				arb.IsBusy = true;
				buyQuote.AggAccount.Account.IsBusy = true;
				sellQuote.AggAccount.Account.IsBusy = true;
			}

			Logger.Trace(cb => cb($"HubArbService.CheckOpen {arb} on QuoteEvent at {e.TimeStamp:yyyy-MM-dd HH:mm:ss.ffffff}"));

			_arbQueues.GetOrAdd(arb.Id, new FastBlockingCollection<Action>()).Add(() => Open(arb, buyQuote, sellQuote, size));
		}

		private Tuple<Quote, Quote> BestQuotes(StratHubArb arb, StratHubArbQuoteEventArgs e, DateTime now)
		{
			var quotes = e.Quotes.Where(q => CheckAccount(arb, q.AggAccount.Account, now)).ToList();

			if (arb.OffsettingByAvg)
			{
				var buyQuotes = quotes
					.Where(q => !q.AggAccount.BuyDisabled)
					.Where(q => q.AggAccount.Avg.HasValue && q.Sum < arb.MaxSizePerAccount)
					.ToList();
				var sellQuotes = quotes
					.Where(q => !q.AggAccount.SellDisabled)
					.Where(q => q.AggAccount.Avg.HasValue && q.Sum > -arb.MaxSizePerAccount)
					.ToList();

				var bestPairs = new List<Tuple<Quote, Quote, decimal>>();
				foreach (var buyQuote in buyQuotes)
				{
					var sellQuote = sellQuotes.Where(q => q.AggAccount != buyQuote?.AggAccount)
						.OrderBy(q => buyQuote.Ask - buyQuote.AggAccount.Avg +
						              q.AggAccount.Avg - q.Bid)
						.FirstOrDefault();
					if (sellQuote == null) continue;
					var signal = buyQuote.Ask - buyQuote.AggAccount.Avg +
					             sellQuote.AggAccount.Avg - sellQuote.Bid;
					if (!signal.HasValue) continue;
					bestPairs.Add(new Tuple<Quote, Quote, decimal>(buyQuote, sellQuote, signal.Value));
				}
				foreach (var sellQuote in sellQuotes)
				{
					var buyQuote = buyQuotes.Where(q => q.AggAccount != sellQuote?.AggAccount)
						.OrderBy(q => q.Ask - q.AggAccount.Avg +
						              sellQuote.AggAccount.Avg - sellQuote.Bid)
						.FirstOrDefault();
					if (buyQuote == null) continue;
					var signal = buyQuote.Ask - buyQuote.AggAccount.Avg +
					             sellQuote.AggAccount.Avg - sellQuote.Bid;
					if (!signal.HasValue) continue;
					bestPairs.Add(new Tuple<Quote, Quote, decimal>(buyQuote, sellQuote, signal.Value));
				}

				if (!bestPairs.Any()) return null;
				var bestPair = bestPairs.OrderBy(p => p.Item3).First();
				return new Tuple<Quote, Quote>(bestPair.Item1, bestPair.Item2);
			}
			else
			{
				var buyQuotes = quotes.Where(q => !q.AggAccount.BuyDisabled && q.Sum < arb.MaxSizePerAccount);
				var sellQuotes = quotes.Where(q => !q.AggAccount.SellDisabled && q.Sum > -arb.MaxSizePerAccount);
				var buyQuote = buyQuotes.OrderBy(q => q.Ask).FirstOrDefault();
				var sellQuote = sellQuotes.OrderByDescending(q => q.Bid)
					.FirstOrDefault(q => q.AggAccount != buyQuote?.AggAccount);
				return new Tuple<Quote, Quote>(buyQuote, sellQuote);
			}

		}

		private bool CheckAccount(StratHubArb arb, Account account, DateTime now)
		{
			if (account.IsBusy) return false;
			if (!account.LastOrderTime.HasValue) return true;
			return (now - account.LastOrderTime.Value).Minutes >= arb.RestingPeriodInMinutes;
		}

		private async void Open(StratHubArb arb, Quote buyQuote, Quote sellQuote, decimal size)
		{
			try
			{
				Logger.Info($"{arb} arb is opening!!!");

				var result = await Opening(arb, buyQuote, sellQuote, size);

				if (result.Buy.FilledQuantity > result.Sell.FilledQuantity)
				{
					Logger.Warn($"{arb} arb opening size mismatch!!!");
					await SendPosition(arb, buyQuote, Sides.Sell, result.Buy.FilledQuantity - result.Sell.FilledQuantity, OrderTypes.Market);
				}
				else if (result.Buy.FilledQuantity < result.Sell.FilledQuantity)
				{
					Logger.Warn($"{arb} arb opening size mismatch!!!");
					await SendPosition(arb, sellQuote, Sides.Buy, result.Sell.FilledQuantity - result.Buy.FilledQuantity, OrderTypes.Market);
				}

				if (result.Buy.FilledQuantity == 0 || result.Sell.FilledQuantity == 0)
				{
					Logger.Warn($"{arb} arb failed to open!!!");
					return;
				}

				var arbDiff = (result.Sell.AveragePrice - result.Buy.AveragePrice) / arb.PipSize;
				if (arb.OffsettingByAvg && buyQuote.AggAccount.Avg.HasValue && sellQuote.AggAccount.Avg.HasValue)
				{
					var offset = buyQuote.AggAccount.Avg - sellQuote.AggAccount.Avg;
					var arbDiffOffset = (result.Sell.AveragePrice - result.Buy.AveragePrice + offset) / arb.PipSize;
					Logger.Info($"{arb} arb opened with {arbDiff:F2} pip, around {arbDiffOffset:F2} with offset!!!");
				}
				else Logger.Info($"{arb} arb opened with {arbDiff:F2} pip!!!");
			}
			finally
			{
				arb.IsBusy = false;
				buyQuote.AggAccount.Account.IsBusy = false;
				sellQuote.AggAccount.Account.IsBusy = false;
			}
		}

		private void ArbLoop(StratHubArb arb, CancellationToken token)
		{
			var queue = _arbQueues.GetOrAdd(arb.Id, new FastBlockingCollection<Action>());

			while (!token.IsCancellationRequested)
			{
				try
				{
					var action = queue.Take(token);
					action();
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("HubArbService.ArbLoop exception", e);
				}
			}

			_arbQueues.TryRemove(arb.Id, out queue);
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("Hub arbs are stopped");
		}

		private bool IsTime(TimeSpan current, TimeSpan? start, TimeSpan? end)
		{
			if (!start.HasValue || !end.HasValue) return false;
			var startOk = current >= start;
			var endOk = current < end;

			if (end < start) return startOk || endOk;
			return startOk && endOk;
		}
	}
}

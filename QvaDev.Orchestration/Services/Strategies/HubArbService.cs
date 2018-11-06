using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using QvaDev.Common;
using QvaDev.Common.Integration;
using QvaDev.Common.Services;
using QvaDev.Data.Models;
using static QvaDev.Data.Models.StratHubArbQuoteEventArgs;

namespace QvaDev.Orchestration.Services.Strategies
{
	public interface IHubArbService
	{
		void Start(List<StratHubArb> arbs);
		void Stop();
		Task GoFlat(List<StratHubArb> arbs);
		Task GoFlat(StratHubArb arb);
	}

	public class HubArbService : IHubArbService
	{
		private volatile CancellationTokenSource _cancellation;
		private CustomThreadPool<OrderResponse> _orderPool;

		private readonly INewsCalendarService _newsCalendarService;
		private List<StratHubArb> _arbs;
		private readonly object _syncRoot = new object();

		private readonly ConcurrentDictionary<int, BufferBlock<Action>> _arbQueues =
			new ConcurrentDictionary<int, BufferBlock<Action>>();

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
					var sum = arb.StratHubArbPositions
						.Where(p => p.Position.AccountId == aggAcc.AccountId)
						.Where(p => p.Position.Symbol == aggAcc.Symbol)
						.Sum(p => p.Position.SignedSize);

					Sides side;
					if (sum > 0) side = Sides.Sell;
					else if (sum < 0) side = Sides.Buy;
					else continue;

					lock (_syncRoot)
					{
						if (aggAcc.Account.IsBusy) continue;
						aggAcc.Account.IsBusy = true;
						accounts.Add(aggAcc.Account);
					}

					tasks.Add(Task.Factory.StartNew(
						async () => await SendPosition(arb, aggAcc.Account, aggAcc.Symbol, side, Math.Abs(sum)),
						TaskCreationOptions.LongRunning));
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
			var quotes = e.Quotes
				.Where(q => CheckAccount(arb, q.Account, now)).ToList();
			var buyQuote = quotes
				.OrderBy(q => q.GroupQuoteEntry.Ask)
				.FirstOrDefault(q => q.Sum < arb.MaxSizePerAccount);
			var sellQuote = quotes
				.OrderByDescending(q => q.GroupQuoteEntry.Bid)
				.FirstOrDefault(q => q.Sum > -arb.MaxSizePerAccount);
			if (buyQuote == null || sellQuote == null) return;

			// During high risk different signal diff
			var diffInPip = (sellQuote.GroupQuoteEntry.Bid - buyQuote.GroupQuoteEntry.Ask) / arb.PipSize;
			var signalDiff = isHighRisk ? arb.HighRiskSignalDiffInPip.Value : arb.SignalDiffInPip;
			if (diffInPip < signalDiff) return;

			// Volume check
			var size = Math.Min(buyQuote.GroupQuoteEntry.AskVolume ?? 0, sellQuote.GroupQuoteEntry.AskVolume ?? 0);
			size = Math.Min(size, arb.Size);
			size = Math.Min(size, arb.MaxSizePerAccount - buyQuote.Sum);
			size = Math.Min(size, sellQuote.Sum + arb.MaxSizePerAccount);
			if (size <= 0) return;

			// Is busy?
			lock (_syncRoot)
			{
				if (arb.IsBusy) return;
				if (buyQuote.Account.IsBusy) return;
				if (sellQuote.Account.IsBusy) return;
				arb.IsBusy = true;
				buyQuote.Account.IsBusy = true;
				sellQuote.Account.IsBusy = true;
			}

			Logger.Trace(cb => cb($"HubArbService.CheckOpen {arb} on QuoteEvent at {e.TimeStamp:yyyy-MM-dd HH:mm:ss.ffffff}"));

			_arbQueues.GetOrAdd(arb.Id, new BufferBlock<Action>()).Post(() => Open(arb, buyQuote, sellQuote, size));
		}

		private bool CheckAccount(StratHubArb arb, Account account, DateTime now)
		{
			if (account.IsBusy) return false;

			var openTime = account.StratPositions
				.OrderByDescending(x => x.OpenTime)
				.FirstOrDefault()?.OpenTime;

			if (!openTime.HasValue) return true;
			return (now - openTime.Value).Minutes >= arb.RestingPeriodInMinutes;
		}

		private async void Open(StratHubArb arb, Quote buyQuote, Quote sellQuote, decimal size)
		{
			try
			{
				Logger.Info($"{arb.Description} arb is opening!!!");

				var buy = _orderPool.Run(() => SendPosition(arb, buyQuote, Sides.Buy, size));
				var sell = _orderPool.Run(() => SendPosition(arb, sellQuote, Sides.Sell, size));
				await Task.WhenAll(buy, sell);
				var buyPos = buy.Result;
				var sellPos = sell.Result;

				if (buyPos.FilledQuantity > sellPos.FilledQuantity)
				{
					Logger.Error($"{arb.Description} arb opening size mismatch!!!");
					await SendPosition(arb, buyQuote, Sides.Sell, buyPos.FilledQuantity - sellPos.FilledQuantity, true);
				}
				else if (buyPos.FilledQuantity < sellPos.FilledQuantity)
				{
					Logger.Error($"{arb.Description} arb opening size mismatch!!!");
					await SendPosition(arb, sellQuote, Sides.Buy, sellPos.FilledQuantity - buyPos.FilledQuantity, true);
				}

				if (buyPos.FilledQuantity == 0 || sellPos.FilledQuantity == 0) return;

				Logger.Info($"{arb.Description} arb opened!!!");
			}
			finally
			{
				arb.IsBusy = false;
				buyQuote.Account.IsBusy = false;
				sellQuote.Account.IsBusy = false;
			}
		}

		private Task<OrderResponse> SendPosition(StratHubArb arb, Quote quote, Sides side, decimal size, bool forceMarket = false)
		{
			var symbol = quote.GroupQuoteEntry.Symbol.ToString();
			var price = side == Sides.Buy ? quote.GroupQuoteEntry.Ask : quote.GroupQuoteEntry.Bid;

			return SendPosition(arb, quote.Account, symbol, side, size, forceMarket ? null : price);
		}

		private async Task<OrderResponse> SendPosition(StratHubArb arb, Account account, string symbol, Sides side, decimal size, decimal? price = null)
		{
			if (!(account.Connector is IFixConnector fix)) throw new NotImplementedException();
			OrderResponse response;

			if (price == null || arb.OrderType == StratHubArb.StratHubArbOrderTypes.Market)
				response = await fix.SendMarketOrderRequest(symbol, side, size);
			else
				response = await fix.SendAggressiveOrderRequest(symbol, side, size, price.Value, arb.Deviation, arb.TimeWindowInMs,
					arb.MaxRetryCount, arb.RetryPeriodInMs);

			PersistPosition(arb, account, symbol, response);
			return response;
		}

		private void PersistPosition(StratHubArb arb, Account account, string symbol, OrderResponse closePos)
		{
			var side = closePos.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell;

			var pos = new StratPosition()
			{
				AccountId = account.Id,
				Account = account,
				AvgPrice = closePos.AveragePrice ?? 0,
				OpenTime = HiResDatetime.UtcNow,
				Side = side,
				Size = closePos.FilledQuantity,
				Symbol = symbol
			};
			arb.StratHubArbPositions.Add(new StratHubArbPosition { Position = pos });
			account.StratPositions.Add(pos);
		}

		private void ArbLoop(StratHubArb arb, CancellationToken token)
		{
			var queue = _arbQueues.GetOrAdd(arb.Id, new BufferBlock<Action>());

			while (!token.IsCancellationRequested)
			{
				try
				{
					var action = queue.ReceiveAsync(token).Result;
					action();
				}
				catch (AggregateException e)
				{
					if (e.InnerException is TaskCanceledException) break;
					Logger.Error("HubArbService.ArbLoop exception", e);
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

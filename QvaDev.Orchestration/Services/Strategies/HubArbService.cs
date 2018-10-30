using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
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
		private bool _isStarted;
		private readonly ILog _log;
		private readonly INewsCalendarService _newsCalendarService;
		private List<StratHubArb> _arbs;
		private readonly object _syncRoot = new object();

		public HubArbService(
			ILog log,
			INewsCalendarService newsCalendarService)
		{
			_newsCalendarService = newsCalendarService;
			_log = log;
		}

		public void Start(List<StratHubArb> arbs)
		{
			_arbs = arbs;

			foreach (var arb in _arbs)
			{
				arb.ArbQuote -= Arb_ArbQuote;
				arb.ArbQuote += Arb_ArbQuote;
			}

			_isStarted = true;
			_log.Info("Hub arbs are started");
		}

		public async Task GoFlat(List<StratHubArb> arbs)
		{
			_log.Info("Hub arbs are going flat!!!");
			foreach (var arb in arbs.Where(a => a.Run)) await GoFlat(arb);
		}

		private void Arb_ArbQuote(object sender, StratHubArbQuoteEventArgs e)
		{
			if (!_isStarted) return;
			var arb = (StratHubArb) sender;

			if (!arb.Run) return;
			if (arb.IsBusy) return;
			if (IsCloseTime(arb)) return;

			CheckOpen(arb, e);
		}

		private bool IsCloseTime(StratHubArb arb)
		{
			if (!arb.HasTiming) return false;
			if (IsTime(DateTime.UtcNow.TimeOfDay, arb.EarliestOpenTime, arb.LatestCloseTime)) return false;
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
			var now = DateTime.UtcNow;
			if (IsTime(now.TimeOfDay, arb.LatestOpenTime, arb.EarliestOpenTime)) return;

			// No HighRiskSignalDiffInPip then no trade
			var isHighRisk = _newsCalendarService.IsHighRiskTime(now, 5);
			if (isHighRisk && !arb.HighRiskSignalDiffInPip.HasValue) return;

			//if (arb.LastOpenTime.HasValue &&
			//    (DateTime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.RestingPeriodInMinutes) return;

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

			Open(arb, buyQuote, sellQuote, size);
		}

		private bool CheckAccount(StratHubArb arb, Account account, DateTime now)
		{
			if (account.IsBusy) return false;

			var openTime = account.StratPositions
				.OrderByDescending(x => x.OpenTime)
				.FirstOrDefault()?.OpenTime;

			if (openTime.HasValue && (now - openTime.Value).Minutes < arb.RestingPeriodInMinutes)
				return false;
			return true;
		}

		private void Open(StratHubArb arb, Quote buyQuote, Quote sellQuote, decimal size)
		{
			lock (_syncRoot)
			{
				if (arb.IsBusy) return;
				if (buyQuote.Account.IsBusy) return;
				if (sellQuote.Account.IsBusy) return;
				arb.IsBusy = true;
				buyQuote.Account.IsBusy = true;
				sellQuote.Account.IsBusy = true;
			}

			Task.Factory.StartNew(async () =>
			{
				try
				{
					var buy = Task.Run(() => SendPosition(arb, buyQuote, Sides.Buy, size));
					var sell = Task.Run(() => SendPosition(arb, sellQuote, Sides.Sell, size));
					await Task.WhenAll(buy, sell);
					var buyPos = buy.Result;
					var sellPos = sell.Result;

					if (buyPos.FilledQuantity > sellPos.FilledQuantity)
					{
						_log.Error($"{arb.Description} arb opening size mismatch!!!");
						await SendPosition(arb, buyQuote, Sides.Sell, buyPos.FilledQuantity - sellPos.FilledQuantity, true);
					}
					else if (buyPos.FilledQuantity < sellPos.FilledQuantity)
					{
						_log.Error($"{arb.Description} arb opening size mismatch!!!");
						await SendPosition(arb, sellQuote, Sides.Buy, sellPos.FilledQuantity - buyPos.FilledQuantity, true);
					}

					if (buyPos.FilledQuantity == 0 || sellPos.FilledQuantity == 0) return;

					_log.Info($"{arb.Description} arb opened!!!");
				}
				finally
				{
					arb.IsBusy = false;
					buyQuote.Account.IsBusy = false;
					sellQuote.Account.IsBusy = false;
				}
			}, TaskCreationOptions.LongRunning);
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
				OpenTime = DateTime.UtcNow,
				Side = side,
				Size = closePos.FilledQuantity,
				Symbol = symbol
			};
			arb.StratHubArbPositions.Add(new StratHubArbPosition { Position = pos });
			account.StratPositions.Add(pos);
		}

		public void Stop()
		{
			_isStarted = false;
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

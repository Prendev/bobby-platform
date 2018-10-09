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
		void GoFlat(List<StratHubArb> arbs);
		void GoFlat(StratHubArb arb);
	}

	public class HubArbService : IHubArbService
	{
		private bool _isStarted;
		private readonly ILog _log;
		private readonly INewsCalendarService _newsCalendarService;
		private List<StratHubArb> _arbs;

		public HubArbService(
			ILog log,
			INewsCalendarService newsCalendarService)
		{
			_newsCalendarService = newsCalendarService;
			_log = log;
			_isStarted = true;
			_log.Info("Hub arbs are started");
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

		public void GoFlat(List<StratHubArb> arbs)
		{
			foreach (var arb in arbs) GoFlat(arb);
			_log.Info("Hub arbs are going flat!!!");
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

		public void GoFlat(StratHubArb arb)
		{
			lock (arb)
			{
				if (arb.IsBusy) return;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(async () =>
			{
				try
				{
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

						var closePos =
							await SendPosition(arb, aggAcc.Account.Connector as IFixConnector, aggAcc.Symbol, side, Math.Abs(sum));

						PersistPosition(arb, aggAcc.Account, aggAcc.Symbol, closePos);
					}
				}
				finally
				{
					arb.IsBusy = false;
				}
			}, TaskCreationOptions.LongRunning);
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
			lock (arb)
			lock (buyQuote.Account)
			lock (sellQuote.Account)
			{
				if (arb.IsBusy) return;
				if (buyQuote.Account.IsBusy) return;
				if (sellQuote.Account.IsBusy) return;

				arb.LastOpenTime = DateTime.UtcNow;
				arb.IsBusy = true;
				buyQuote.Account.IsBusy = true;
				sellQuote.Account.IsBusy = true;
			}

			Task.Factory.StartNew(async () =>
			{
				try
				{
					var buy = SendPosition(arb, Sides.Buy, buyQuote, size);
					var sell = SendPosition(arb, Sides.Sell, sellQuote, size);
					await Task.WhenAll(buy, sell);
					var buyPos = buy.Result;
					var sellPos = sell.Result;

					if (buyPos.FilledQuantity > sellPos.FilledQuantity)
					{
						_log.Error($"{arb.Description} arb opening size mismatch!!!");
						var closePos =
							await SendPosition(arb, Sides.Sell, buyQuote, buyPos.FilledQuantity - sellPos.FilledQuantity, true);

						PersistPosition(arb, buyQuote.Account, buyQuote.GroupQuoteEntry.Symbol.ToString(), closePos);
					}
					else if (buyPos.FilledQuantity < sellPos.FilledQuantity)
					{
						_log.Error($"{arb.Description} arb opening size mismatch!!!");
						var closePos =
							await SendPosition(arb, Sides.Buy, sellQuote, sellPos.FilledQuantity - buyPos.FilledQuantity, true);

						PersistPosition(arb, sellQuote.Account, sellQuote.GroupQuoteEntry.Symbol.ToString(), closePos);
					}

					PersistPosition(arb, buyQuote.Account, buyQuote.GroupQuoteEntry.Symbol.ToString(), buyPos);
					PersistPosition(arb, sellQuote.Account, sellQuote.GroupQuoteEntry.Symbol.ToString(), sellPos);

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

		private void PersistPosition(StratHubArb arb, Account account, string symbol, OrderResponse closePos)
		{
			var side = closePos.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell;

			var pos = new StratPosition()
			{
				AccountId = account.Id,
				Account = account,
				AvgPrice = closePos.AveragePrice ?? 0,
				OpenTime = arb.LastOpenTime ?? DateTime.UtcNow,
				Side = side,
				Size = closePos.FilledQuantity,
				Symbol = symbol
			};
			arb.StratHubArbPositions.Add(new StratHubArbPosition {Position = pos});
			account.StratPositions.Add(pos);
		}

		private Task<OrderResponse> SendPosition(StratHubArb arb, Sides side, Quote quote, decimal size, bool forceMarket = false)
		{
			if (!(quote.Account.Connector is IFixConnector fix)) throw new NotImplementedException();

			var symbol = quote.GroupQuoteEntry.Symbol.ToString();
			var price = side == Sides.Buy ? quote.GroupQuoteEntry.Ask : quote.GroupQuoteEntry.Bid;

			return SendPosition(arb, fix, symbol, side, size, forceMarket ? null : price);
		}


		private Task<OrderResponse> SendPosition(StratHubArb arb, IFixConnector fix, string symbol, Sides side, decimal size, decimal? price = null)
		{
			if (arb.OrderType == StratHubArb.StratHubArbOrderTypes.Market || price == null)
				return fix.SendMarketOrderRequest(symbol, side, size);

			return fix.SendAggressiveOrderRequest(symbol, side, size,
				price.Value, arb.Deviation, arb.TimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);
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

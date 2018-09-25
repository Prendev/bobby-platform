using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using static QvaDev.Data.Models.StratHubArbQuoteEventArgs;

namespace QvaDev.Orchestration.Services.Strategies
{
	public interface IHubArbService
	{
		void Start(List<StratHubArb> arbs);
		void Stop();
	}

	public class HubArbService : IHubArbService
	{
		private bool _isStarted;
		private readonly ILog _log;
		private List<StratHubArb> _arbs;

		public HubArbService(ILog log)
		{
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

				foreach (var aggAcc in arb.Aggregator.Accounts)
					aggAcc.Account.Connector.Subscribe(aggAcc.Symbol);
			}

			_isStarted = true;
			_log.Info("Hub arbs are started");
		}

		private void Arb_ArbQuote(object sender, StratHubArbQuoteEventArgs e)
		{
			if (!_isStarted) return;
			var arb = (StratHubArb) sender;

			if (!arb.Run) return;
			if (arb.IsBusy) return;

			if (IsTimingClose(arb)) return;
			if (arb.LastOpenTime.HasValue &&
			    (DateTime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.MinOpenTimeInMinutes) return;

			CheckOpen(arb, e);
		}

		private bool IsTimingClose(StratHubArb arb)
		{
			var timingClose = arb.HasTiming && IsTime(DateTime.UtcNow.TimeOfDay, arb.LatestCloseTime, arb.EarliestOpenTime);
			if (!timingClose) return false;

			lock (arb)
			{
				if (arb.IsBusy) return true;
				arb.IsBusy = true;
			}

			// Go to flat
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

			return true;
		}

		private void CheckOpen(StratHubArb arb, StratHubArbQuoteEventArgs e)
		{
			if (IsTime(DateTime.UtcNow.TimeOfDay, arb.LatestOpenTime, arb.EarliestOpenTime)) return;

			var buyQuote = e.Quotes
				.OrderBy(q => q.GroupQuoteEntry.Ask)
				.FirstOrDefault(q => q.Sum < arb.MaxSizePerAccount - arb.Size);
			var sellQuote = e.Quotes
				.OrderByDescending(q => q.GroupQuoteEntry.Bid)
				.FirstOrDefault(q => q.Sum > -arb.MaxSizePerAccount + arb.Size);
			if (buyQuote == null || sellQuote == null) return;

			var diffInPip = (sellQuote.GroupQuoteEntry.Bid - buyQuote.GroupQuoteEntry.Ask) / arb.PipSize;
			if (diffInPip < arb.SignalDiffInPip) return;

			Open(arb, buyQuote, sellQuote);
		}

		private void Open(StratHubArb arb, Quote buyQuote, Quote sellQuote)
		{
			lock (arb)
			{
				if (arb.IsBusy) return;
				arb.LastOpenTime = DateTime.UtcNow;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(async () =>
			{
				try
				{
					var buy = SendPosition(arb, Sides.Buy, buyQuote, arb.Size);
					var sell = SendPosition(arb, Sides.Sell, sellQuote, arb.Size);
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
				}
			}, TaskCreationOptions.LongRunning);
		}

		private void PersistPosition(StratHubArb arb, Account account, string symbol, OrderResponse closePos)
		{
			var side = closePos.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell;
			arb.StratHubArbPositions.Add(new StratHubArbPosition()
			{
				Position = new StratPosition()
				{
					AccountId = account.Id,
					AvgPrice = closePos.AveragePrice ?? 0,
					OpenTime = arb.LastOpenTime ?? DateTime.UtcNow,
					Side = side,
					Size = closePos.FilledQuantity,
					Symbol = symbol
				}
			});
		}

		private Task<OrderResponse> SendPosition(StratHubArb arb, Sides side, Quote quote, decimal size, bool forceMarket = false)
		{
			if (!(quote.Account.Connector is IFixConnector fix)) throw new NotImplementedException();

			var symbol = quote.GroupQuoteEntry.Symbol.ToString();
			var price = side == Sides.Buy ? quote.GroupQuoteEntry.Ask : quote.GroupQuoteEntry.Bid;

			return SendPosition(arb, fix, symbol, side, size, price);
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

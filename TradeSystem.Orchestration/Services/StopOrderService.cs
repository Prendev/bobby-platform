using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services
{
	public interface IStopOrderService
	{
		StopResponse SendStopOrder(MarketMaker set, Sides side, decimal stopPrice, decimal marketPrice);
		void Stop();
		void ClearLimits(MarketMaker set);
		event EventHandler<StopResponse> Fill;
	}

	public class StopOrderService : IStopOrderService
	{
		private readonly ConcurrentDictionary<MarketMaker, ConcurrentBag<StopResponse>> _stopOrders =
			new ConcurrentDictionary<MarketMaker, ConcurrentBag<StopResponse>>();

		public event EventHandler<StopResponse> Fill;

		public void Stop()
		{
			lock (_stopOrders)
			{
				foreach (var stopOrder in _stopOrders)
				{
					stopOrder.Key.NewTick -= Set_NewTick;
					stopOrder.Key.LimitFill -= Set_LimitFill;
					Logger.Debug($"{stopOrder.Key.Account} StopOrderService.Stop({stopOrder.Key})");
				}
				_stopOrders.Clear();
			}
		}

		public void ClearLimits(MarketMaker set)
		{
			lock (_stopOrders)
			{
				if (!_stopOrders.TryGetValue(set, out var orders)) return;
				while (orders.TryTake(out var _)) ;
			}
		}

		public StopResponse SendStopOrder(MarketMaker set, Sides side, decimal stopPrice, decimal marketPrice)
		{
			lock (_stopOrders)
			{
				set.NewTick -= Set_NewTick;
				set.NewTick += Set_NewTick;
				set.LimitFill -= Set_LimitFill;
				set.LimitFill += Set_LimitFill;

				var stopOrders = _stopOrders.GetOrAdd(set, new ConcurrentBag<StopResponse>());
				var stopResponse = new StopResponse()
				{
					Symbol = set.Symbol,
					Side = side,
					StopPrice = stopPrice,
					MarketPrice = marketPrice,
					DomTrigger = set.DomTrigger
				};
				stopOrders.Add(stopResponse);

				Logger.Debug($"{set.Account} StopOrderService.SendStopOrder({set}, {side}, {stopPrice}, {marketPrice})");
				return stopResponse;
			}
		}

		private void Set_NewTick(object sender, NewTick e)
		{
			if (!(sender is MarketMaker set)) return;
			if (set.Token.IsCancellationRequested) return;
			if (!(set.Account.Connector is FixApiConnectorBase)) return;
			Task.Run(() => CheckStopOrders(set));
		}

		private void Set_LimitFill(object sender, LimitFill e)
		{
			if (!(sender is MarketMaker set)) return;
			if (set.Token.IsCancellationRequested) return;
			if (!(set.Account.Connector is FixApiConnectorBase)) return;
			Task.Run(() => CheckFills(set, e));
		}

		private void CheckFills(MarketMaker set, LimitFill e)
		{
			var responses = _stopOrders.GetOrAdd(set, new ConcurrentBag<StopResponse>());
			OnFill(set, responses.FirstOrDefault(r => r.LimitResponse == e.LimitResponse));
		}

		private void CheckStopOrders(MarketMaker set)
		{
			if (set.Token.IsCancellationRequested) return;
			var responses = _stopOrders.GetOrAdd(set, new ConcurrentBag<StopResponse>());
			foreach (var response in responses.Where(o => !o.IsFilled))
			{
				if (set.Token.IsCancellationRequested) return;

				lock (response)
				{
					if (response.IsBusy) return;
					response.IsBusy = true;
				}

				if (response.Side == Sides.Buy) CheckBuy(set, response);
				else if (response.Side == Sides.Sell) CheckSell(set, response);

				response.IsBusy = false;
			}
		}

		private void CheckBuy(MarketMaker set, StopResponse response)
		{
			if (response.IsFilled) return;
			if (response.LimitResponse?.FilledQuantity > 0) return;
			var lastTick = set.Account.GetLastTick(response.Symbol);
			if (lastTick.Ask < response.StopPrice) return;
			if (lastTick.Ask == response.StopPrice && response.DomTrigger > 0 && lastTick.AskVolume > response.DomTrigger) return;

			var connector = (FixApiConnectorBase)set.Account.Connector;

			if (response.LimitResponse == null)
				response.LimitResponse = connector.PutNewOrderRequest(response.Symbol, Sides.Buy, set.ContractSize, lastTick.Ask).Result;
			else if (lastTick.Ask >= response.MarketPrice && lastTick.Ask > response.LimitResponse.OrderPrice)
			{
				connector.CancelLimit(response.LimitResponse).Wait();
				var lastStatus = connector.GetOrderStatusReport(response.LimitResponse);
				if (lastStatus.Status == OrderStatus.Canceled)
				{
					response.LimitResponse = null;
					CheckBuy(set, response);
				}
			}
		}

		private void CheckSell(MarketMaker set, StopResponse response)
		{
			if (response.IsFilled) return;
			if (response.LimitResponse?.FilledQuantity > 0) return;
			var lastTick = set.Account.GetLastTick(response.Symbol);
			if (lastTick.Bid > response.StopPrice) return;
			if (lastTick.Bid == response.StopPrice && response.DomTrigger > 0 && lastTick.BidVolume > response.DomTrigger) return;

			var connector = (FixApiConnectorBase)set.Account.Connector;

			if (response.LimitResponse == null)
				response.LimitResponse = connector.PutNewOrderRequest(response.Symbol, Sides.Sell, set.ContractSize, lastTick.Bid).Result;
			else if (lastTick.Bid > response.MarketPrice && lastTick.Bid < response.LimitResponse.OrderPrice)
			{
				connector.CancelLimit(response.LimitResponse).Wait();
				var lastStatus = connector.GetOrderStatusReport(response.LimitResponse);
				if (lastStatus.Status == OrderStatus.Canceled)
				{
					response.LimitResponse = null;
					CheckSell(set, response);
				}
			}
		}

		private void OnFill(MarketMaker set, StopResponse response)
		{
			if (response == null) return;
			if (response.IsFilled) return;
			response.IsFilled = true;
			Fill?.Invoke(set, response);
		}
	}
}

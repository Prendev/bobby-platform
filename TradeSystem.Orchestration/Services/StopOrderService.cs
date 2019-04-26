using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services
{
	public interface IStopOrderService
	{
		void Start(MarketMaker set);
		StopResponse SendStopOrder(MarketMaker set, Sides side, decimal stopPrice, decimal marketPrice, string desc, int magic);
		void ClearLimits(MarketMaker set);
		void Stop();
		event EventHandler<StopResponse> Fill;
	}

	public class StopOrderService : IStopOrderService
	{
		private readonly ConcurrentDictionary<MarketMaker, ConcurrentDictionary<string, StopResponse>> _stopOrders =
			new ConcurrentDictionary<MarketMaker, ConcurrentDictionary<string,StopResponse>>();
		private readonly ConcurrentDictionary<LimitResponse, StopResponse> _limitMapping =
			new ConcurrentDictionary<LimitResponse, StopResponse>();

		public event EventHandler<StopResponse> Fill;

		public void Start(MarketMaker set)
		{
			set.NewTick -= Set_NewTick;
			set.NewTick += Set_NewTick;
			set.LimitFill -= Set_LimitFill;
			set.LimitFill += Set_LimitFill;
		}

		public StopResponse SendStopOrder(MarketMaker set, Sides side, decimal stopPrice, decimal marketPrice, string desc, int magic)
		{
			lock (_stopOrders)
			{
				var stopOrders = _stopOrders.GetOrAdd(set, new ConcurrentDictionary<string, StopResponse>());
				var stopResponse = new StopResponse()
				{
					Symbol = set.Symbol,
					Side = side,
					StopPrice = stopPrice,
					AggressivePrice = marketPrice,
					DomTrigger = set.DomTrigger,
					Description = desc,
					Magic = magic
				};
				stopOrders.AddOrUpdate($"{desc}_{magic}", stopResponse, (s, o) => stopResponse);

				Logger.Debug($"{set} StopOrderService.SendStopOrder({side}, {stopPrice}, {marketPrice}, {desc}, {magic})");
				return stopResponse;
			}
		}

		public void ClearLimits(MarketMaker set)
		{
			lock (_stopOrders)
			{
				if (!_stopOrders.TryGetValue(set, out var orders)) return;
				if (orders.IsEmpty) return;
				Logger.Debug($"{set} StopOrderService.ClearLimits");
				orders.Clear();
			}
		}

		public void Stop()
		{
			lock (_stopOrders)
			{
				foreach (var stopOrder in _stopOrders)
				{
					stopOrder.Key.NewTick -= Set_NewTick;
					stopOrder.Key.LimitFill -= Set_LimitFill;
					Logger.Debug($"{stopOrder.Key} StopOrderService.Stop");
				}
				_stopOrders.Clear();
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
			if (e.LimitResponse == null) return;
			if (!_limitMapping.TryGetValue(e.LimitResponse, out var stopResponse)) return;
			OnFill(set, stopResponse);
		}

		private void CheckStopOrders(MarketMaker set)
		{
			if (set.Token.IsCancellationRequested) return;
			var responses = _stopOrders.GetOrAdd(set, new ConcurrentDictionary<string, StopResponse>());
			foreach (var response in responses.Values)
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
			{
				response.LimitResponse = connector.PutNewOrderRequest(response.Symbol, Sides.Buy, set.ContractSize, lastTick.Ask).Result;
				if (response.LimitResponse == null) return;
				_limitMapping.AddOrUpdate(response.LimitResponse, response, (l, s) => response);
				if (response.LimitResponse.RemainingQuantity == 0) OnFill(set, response);
			}
			else if (lastTick.Ask >= response.AggressivePrice && lastTick.Ask > response.LimitResponse.OrderPrice)
			{
				Logger.Warn($"{set} StopOrderService.CheckBuy.CancelLimit of {response?.StopPrice} stop price - {response.Side} {response.Description}_{response.Magic}");
				connector.CancelLimit(response.LimitResponse).Wait();
				var lastStatus = connector.GetOrderStatusReport(response.LimitResponse);
				if (lastStatus.Status != OrderStatus.Canceled) return;
				response.LimitResponse = null;
				CheckBuy(set, response);
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
			{
				response.LimitResponse = connector.PutNewOrderRequest(response.Symbol, Sides.Sell, set.ContractSize, lastTick.Bid).Result;
				if (response.LimitResponse == null) return;
				_limitMapping.AddOrUpdate(response.LimitResponse, response, (l, s) => response);
				if (response.LimitResponse.RemainingQuantity == 0) OnFill(set, response);
			}
			else if (lastTick.Bid <= response.AggressivePrice && lastTick.Bid < response.LimitResponse.OrderPrice)
			{
				Logger.Warn($"{set} StopOrderService.CheckSell.CancelLimit of {response?.StopPrice} stop price - {response.Side} {response.Description}_{response.Magic}");
				connector.CancelLimit(response.LimitResponse).Wait();
				var lastStatus = connector.GetOrderStatusReport(response.LimitResponse);
				if (lastStatus.Status != OrderStatus.Canceled) return;
				response.LimitResponse = null;
				CheckSell(set, response);
			}
		}

		private void OnFill(MarketMaker set, StopResponse response)
		{
			if (response == null) return;
			lock (response)
			{
				if (response.IsFilled) return;
				response.IsFilled = true;
			}
			Logger.Info($"{set} StopOrderService.OnFill at {response.StopPrice} stop price - {response.Side} {response.Description}_{response.Magic}");
			Fill?.Invoke(set, response);
		}
	}
}

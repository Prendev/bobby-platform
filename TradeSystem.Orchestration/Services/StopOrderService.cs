﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services
{
	public interface IStopOrderService
	{
		void Start(MarketMaker set);
		StopResponse SendStopOrder(MarketMaker set, Sides side, decimal stopPrice, decimal marketPrice, string userId);
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

		public StopResponse SendStopOrder(MarketMaker set, Sides side, decimal stopPrice, decimal marketPrice, string userId)
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
					UserId = userId,
				};
				stopOrders.AddOrUpdate($"{userId}", stopResponse, (s, o) => stopResponse);

				Logger.Debug($"{set} StopOrderService.SendStopOrder({side}, {stopPrice}, {marketPrice}, {userId})");
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
			set.Queue?.Add(() => CheckStopOrders(set));
		}

		private void Set_LimitFill(object sender, LimitFill e)
		{
			if (e.LimitResponse?.RemainingQuantity != 0) return;
			if (!(sender is MarketMaker set)) return;
			if (set.Token.IsCancellationRequested) return;
			if (!(set.Account.Connector is FixApiConnectorBase)) return;
			if (!_limitMapping.TryGetValue(e.LimitResponse, out var stopResponse)) return;
			set.Queue?.Add(() => OnFill(set, stopResponse));

		}

		private void CheckStopOrders(MarketMaker set)
		{
			if (set.Token.IsCancellationRequested) return;
			var responses = _stopOrders.GetOrAdd(set, new ConcurrentDictionary<string, StopResponse>());
			foreach (var response in responses.Values.ToList().OrderBy(r => r.UserId))
			{
				if (set.Token.IsCancellationRequested) return;
				if (response.Side == Sides.Buy) CheckBuy(set, response);
				else if (response.Side == Sides.Sell) CheckSell(set, response);
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
				var price = lastTick.Ask < response.AggressivePrice ? response.StopPrice : lastTick.Ask;
				response.LimitResponse = connector.PutNewOrderRequest(response.Symbol, Sides.Buy, set.ContractSize, price).Result;
				if (response.LimitResponse == null) return;
				_limitMapping.AddOrUpdate(response.LimitResponse, response, (l, s) => response);
				if (response.LimitResponse.RemainingQuantity == 0) OnFill(set, response);
			}
			else if (lastTick.Ask >= response.AggressivePrice && lastTick.Ask > response.LimitResponse.OrderPrice)
			{
				Logger.Warn($"{set} StopOrderService.CheckBuy.CancelLimit of {response?.StopPrice} stop price - {response.Side} {response}");
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
				var price = lastTick.Bid > response.AggressivePrice ? response.StopPrice : lastTick.Bid;
				response.LimitResponse = connector.PutNewOrderRequest(response.Symbol, Sides.Sell, set.ContractSize, price).Result;
				if (response.LimitResponse == null) return;
				_limitMapping.AddOrUpdate(response.LimitResponse, response, (l, s) => response);
				if (response.LimitResponse.RemainingQuantity == 0) OnFill(set, response);
			}
			else if (lastTick.Bid <= response.AggressivePrice && lastTick.Bid < response.LimitResponse.OrderPrice)
			{
				Logger.Warn($"{set} StopOrderService.CheckSell.CancelLimit of {response?.StopPrice} stop price - {response.Side} {response}");
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
			Logger.Info($"{set} StopOrderService.OnFill at {response.StopPrice} stop price - {response.Side} {response}");
			Fill?.Invoke(set, response);
		}
	}
}

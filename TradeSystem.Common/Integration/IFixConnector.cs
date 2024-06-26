﻿using System.Threading.Tasks;
using TradeSystem.Communication;

namespace TradeSystem.Common.Integration
{
	public interface IFixConnector : IConnector
	{
		Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity);

		Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity,
			int timeout, int retryCount, int retryPeriod);

		Task<OrderResponse> CloseOrderRequest(string symbol, Sides side, decimal quantity,
			int timeout, int retryCount, int retryPeriod, string[] orderIds);

		Task<OrderResponse> SendAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff,
			int timeout, int retryCount, int retryPeriod);

		Task<OrderResponse> SendDelayedAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff, decimal correction,
			int timeout, int retryCount, int retryPeriod);

		Task<OrderResponse> SendGtcLimitOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff,
			int timeout, int retryCount, int retryPeriod);

		Task<LimitResponse> PutNewOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice);
		OrderStatusReport GetOrderStatusReport(LimitResponse response);

		Task<bool> ChangeLimitPrice(LimitResponse response, decimal limitPrice);
		Task<bool> CancelLimit(LimitResponse response);
	}
}

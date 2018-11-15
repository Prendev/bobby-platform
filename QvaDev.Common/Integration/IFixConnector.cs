﻿using System.Threading.Tasks;

namespace QvaDev.Common.Integration
{
	public interface IFixConnector : IConnector
	{
		Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity);

		Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity,
			int timeout, int retryCount, int retryPeriod);

		Task<OrderResponse> SendAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation,
			int timeout, int retryCount, int retryPeriod);

		void OrderMultipleCloseBy(string symbol);
		SymbolData GetSymbolInfo(string symbol);
	}
}

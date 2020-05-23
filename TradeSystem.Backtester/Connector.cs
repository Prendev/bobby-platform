using System;
using System.Threading.Tasks;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;
using TradeSystem.Data.Models;

namespace TradeSystem.Backtester
{
	public class Connector : ConnectorBase, IFixConnector
	{
		private readonly BacktesterAccount _account;

		public override int Id => _account?.Id ?? 0;
		public override string Description => _account?.Description;
		public override bool IsConnected => true;

		public Connector(BacktesterAccount account)
		{
			_account = account;
		}

		public void Connect() => OnConnectionChanged(ConnectionStates.Connected);
		public override void Disconnect() => OnConnectionChanged(ConnectionStates.Disconnected);
		public override void Subscribe(string symbol) => Logger.Debug($"{Description} Connector.Subscribe({symbol})");

		public override Tick GetLastTick(string symbol)
		{
			throw new NotImplementedException();
		}

		public Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity)
		{
			var price = GetLastTick(symbol).GetPrice(side);
			var response = new OrderResponse()
			{
				Side = side,
				OrderPrice = price,
				OrderedQuantity = quantity,
				AveragePrice = price,
				FilledQuantity = quantity
			};
			LogOrderResponse(symbol, response);
			return Task.FromResult(response);
		}
		public Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, int timeout,
			int retryCount, int retryPeriod) => SendMarketOrderRequest(symbol, side, quantity);
		public Task<OrderResponse> SendAggressiveOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice,
			decimal deviation, decimal priceDiff, int timeout, int retryCount, int retryPeriod) =>
			SendMarketOrderRequest(symbol, side, quantity);
		public Task<OrderResponse> SendDelayedAggressiveOrderRequest(string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff, decimal correction, int timeout, int retryCount,
			int retryPeriod) => SendMarketOrderRequest(symbol, side, quantity);
		public Task<OrderResponse> SendGtcLimitOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice,
			decimal deviation, decimal priceDiff, int timeout, int retryCount, int retryPeriod) =>
			SendMarketOrderRequest(symbol, side, quantity);

		public Task<LimitResponse> PutNewOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice) =>
			throw new NotImplementedException();
		public OrderStatusReport GetOrderStatusReport(LimitResponse response) => throw new NotImplementedException();
		public Task<bool> ChangeLimitPrice(LimitResponse response, decimal limitPrice) => throw new NotImplementedException();
		public Task<bool> CancelLimit(LimitResponse response) => throw new NotImplementedException();

		public void Start()
		{
		}
		public void Pause()
		{
		}
		public void Stop()
		{
		}

		private void LogOrderResponse(string symbol, OrderResponse response)
		{
			Logger.Debug($"\t{Description}" +
			             $"\t{_account.UtcNow:yyyy-MM-dd HH:mm:ss.ffff}" +
						 $"\t{symbol}" +
			             $"\t{response.Side}" +
			             $"\t{response.FilledQuantity}");
		}
	}
}

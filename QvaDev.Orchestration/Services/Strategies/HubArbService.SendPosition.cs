using System;
using System.Threading.Tasks;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using static QvaDev.Data.Models.StratHubArbQuoteEventArgs;
using OrderTypes = QvaDev.Data.Models.StratHubArb.StratHubArbOrderTypes;

namespace QvaDev.Orchestration.Services.Strategies
{
	public partial class HubArbService
	{
		private Task<OrderResponse> SendPosition(StratHubArb arb, Quote quote, Sides side,
			decimal size, OrderTypes orderType)
		{
			var symbol = quote.GroupQuoteEntry.Symbol.ToString();
			var price = side == Sides.Buy ? quote.GroupQuoteEntry.Ask : quote.GroupQuoteEntry.Bid;

			return SendPosition(arb, quote.AggAccount.Account, symbol, side, size, orderType, price);
		}

		private async Task<OrderResponse> SendPosition(StratHubArb arb, Account account, string symbol, Sides side,
			 decimal size, OrderTypes orderType, decimal? price = null)
		{
			try
			{
				if (!(account.Connector is IFixConnector fix)) throw new NotImplementedException();
				OrderResponse response;

				if (price == null || orderType == OrderTypes.Market)
					response = await fix.SendMarketOrderRequest(symbol, side, size,
						arb.TimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);

				else if (orderType == OrderTypes.Aggressive)
					response = await fix.SendAggressiveOrderRequest(
						symbol, side, size, price.Value, arb.Deviation, 0,
						arb.TimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);

				else if (orderType == OrderTypes.DelayedAggressive)
					response = await fix.SendDelayedAggressiveOrderRequest(
						symbol, side, size, price.Value, arb.Deviation, 0, arb.Correction,
						arb.DelayTimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);

				else throw new NotImplementedException();

				PersistPosition(arb, account, symbol, response);
				return response;
			}
			catch (Exception e)
			{
				Logger.Error("HubArbService.SendPosition exception", e);
				return new OrderResponse()
				{
					OrderedQuantity = size,
					AveragePrice = null,
					FilledQuantity = 0,
					Side = side
				};
			}
		}

		private void PersistPosition(StratHubArb arb, Account account, string symbol, OrderResponse response)
		{
			var side = response.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell;

			var newEntity = new StratHubArbPosition()
			{
				StratHubArb = arb,
				StratHubArbId = arb.Id,
				Position = new StratPosition()
				{
					AccountId = account.Id,
					Account = account,
					AvgPrice = response.AveragePrice ?? 0,
					OpenTime = HiResDatetime.UtcNow,
					Side = side,
					Size = response.FilledQuantity,
					Symbol = symbol
				}
			};
			account.LastOrderTime = newEntity.Position.OpenTime;
			arb.StratHubArbPositions.Add(newEntity);
			lock (account.StratHubArbPositions) account.StratHubArbPositions.Add(newEntity);
		}
	}
}

using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;
using static TradeSystem.Data.Models.StratHubArbQuoteEventArgs;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public partial class HubArbService
	{
		private class OpeningResult
		{
			public OrderResponse Buy { get; set; } = new OrderResponse();
			public OrderResponse Sell { get; set; } = new OrderResponse();
		}

		private async Task<OpeningResult> Opening(StratHubArb arb, Quote buyQuote, Quote sellQuote, decimal size)
		{
			if (arb.OpeningLogic == StratHubArb.StratHubArbOpeningLogics.Parallel ||
			    buyQuote.AggAccount.FeedSpeed == sellQuote.AggAccount.FeedSpeed)
				return await OpeningParallel(arb, buyQuote, sellQuote, size);

			if (arb.OpeningLogic == StratHubArb.StratHubArbOpeningLogics.SlowFirst)
				return await OpeningSlowFirst(arb, buyQuote, sellQuote, size);

			return new OpeningResult();
		}

		private async Task<OpeningResult> OpeningParallel(StratHubArb arb, Quote buyQuote, Quote sellQuote, decimal size)
		{
			var retValue = new OpeningResult();
			var buy = _orderPool.Run(() => SendPosition(arb, buyQuote, Sides.Buy, size, arb.OrderType));
			var sell = _orderPool.Run(() => SendPosition(arb, sellQuote, Sides.Sell, size, arb.OrderType));
			await Task.WhenAll(buy, sell);
			retValue.Buy = buy.Result;
			retValue.Sell = sell.Result;
			return retValue;
		}

		private async Task<OpeningResult> OpeningSlowFirst(StratHubArb arb, Quote buyQuote, Quote sellQuote, decimal size)
		{
			var retValue = new OpeningResult();
			if (buyQuote.AggAccount.FeedSpeed < sellQuote.AggAccount.FeedSpeed)
			{
				retValue.Buy = await SendPosition(arb, buyQuote, Sides.Buy, size, arb.SlowOrderType);
				if (retValue.Buy.FilledQuantity == 0) return retValue;
				retValue.Sell = await SendPosition(arb, sellQuote, Sides.Sell, retValue.Buy.FilledQuantity, arb.FastOrderType);
			}
			else if (sellQuote.AggAccount.FeedSpeed < buyQuote.AggAccount.FeedSpeed)
			{
				retValue.Sell = await SendPosition(arb, sellQuote, Sides.Sell, size, arb.SlowOrderType);
				if (retValue.Sell.FilledQuantity == 0) return retValue;
				retValue.Buy = await SendPosition(arb, buyQuote, Sides.Buy, retValue.Sell.FilledQuantity, arb.FastOrderType);
			}
			return retValue;
		}
	}
}

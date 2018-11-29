using System.Threading.Tasks;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using static QvaDev.Data.Models.StratHubArbQuoteEventArgs;

namespace QvaDev.Orchestration.Services.Strategies
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
				return await ParallelOpening(arb, buyQuote, sellQuote, size);

			if (arb.OpeningLogic == StratHubArb.StratHubArbOpeningLogics.SlowFirst)
				return await SlowFirstOpening(arb, buyQuote, sellQuote, size);

			if (arb.OpeningLogic == StratHubArb.StratHubArbOpeningLogics.DelayedHedge)
				return await DelayedHedgeOpening(arb, buyQuote, sellQuote, size);

			return new OpeningResult();
		}

		private async Task<OpeningResult> SlowFirstOpening(StratHubArb arb, Quote buyQuote, Quote sellQuote, decimal size)
		{
			var retValue = new OpeningResult();
			if (buyQuote.AggAccount.FeedSpeed < sellQuote.AggAccount.FeedSpeed)
			{
				retValue.Buy = await SendPosition(arb, buyQuote, Sides.Buy, size);
				if (retValue.Buy.FilledQuantity == 0) return retValue;
				retValue.Sell = await SendPosition(arb, sellQuote, Sides.Sell, retValue.Buy.FilledQuantity);
			}
			else if (sellQuote.AggAccount.FeedSpeed < buyQuote.AggAccount.FeedSpeed)
			{
				retValue.Sell = await SendPosition(arb, sellQuote, Sides.Sell, size);
				if (retValue.Sell.FilledQuantity == 0) return retValue;
				retValue.Buy = await SendPosition(arb, buyQuote, Sides.Buy, retValue.Sell.FilledQuantity);
			}
			return retValue;
		}

		private async Task<OpeningResult> ParallelOpening(StratHubArb arb, Quote buyQuote, Quote sellQuote, decimal size)
		{
			var retValue = new OpeningResult();
			var buy = _orderPool.Run(() => SendPosition(arb, buyQuote, Sides.Buy, size));
			var sell = _orderPool.Run(() => SendPosition(arb, sellQuote, Sides.Sell, size));
			await Task.WhenAll(buy, sell);
			retValue.Buy = buy.Result;
			retValue.Sell = sell.Result;
			return retValue;
		}

		private async Task<OpeningResult> DelayedHedgeOpening(StratHubArb arb, Quote buyQuote, Quote sellQuote, decimal size)
		{
			var retValue = new OpeningResult();
			if (buyQuote.AggAccount.FeedSpeed < sellQuote.AggAccount.FeedSpeed)
			{
				retValue.Buy = await SendPosition(arb, buyQuote, Sides.Buy, size);
				if (retValue.Buy.FilledQuantity == 0) return retValue;
				retValue.Sell = await SendPosition(arb, sellQuote, Sides.Sell, retValue.Buy.FilledQuantity);
			}
			else if (sellQuote.AggAccount.FeedSpeed < buyQuote.AggAccount.FeedSpeed)
			{
				retValue.Sell = await SendPosition(arb, sellQuote, Sides.Sell, size);
				if (retValue.Sell.FilledQuantity == 0) return retValue;
				retValue.Buy = await SendPosition(arb, buyQuote, Sides.Buy, retValue.Sell.FilledQuantity);
			}
			return retValue;
		}
	}
}

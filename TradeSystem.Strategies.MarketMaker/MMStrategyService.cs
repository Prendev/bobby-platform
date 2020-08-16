using System.Threading;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;

namespace TradeSystem.Strategies.MarketMaker
{
	/// <summary>
	/// Cross exchange market maker strategy service
	/// </summary>
	public class MMStrategyService : StrategyServiceBase<MM>, IMMStrategyService
	{
		/// <inheritdoc/>
		protected override void Check(MM set, CancellationToken token)
		{
			while (set.LimitFills.TryDequeue(out var e))
				OnLimitFill(set, e.Account, e.LimitFill);

			if (set.LastTakerTick.Bid - set.LastMakerTick.Bid >= set.MinProfitability)
			{
				if (set.MakerBuyLimit == null)
				{
					set.MakerBuyLimit = set.MakerConnector
						.PutNewOrderRequest(set.MakerSymbol, Sides.Buy, set.OrderSize, set.LastMakerTick.Bid).Result;
					Logger.Debug(
						$"MMStrategyService {set} new {nameof(set.MakerBuyLimit)} is created with price {set.MakerBuyLimit.OrderPrice}");
				}
				else if (set.MakerBuyLimit.OrderPrice != set.LastMakerTick.Bid)
				{
					set.MakerConnector.ChangeLimitPrice(set.MakerBuyLimit, set.LastMakerTick.Bid);
					Logger.Debug(
						$"MMStrategyService {set} {nameof(set.MakerBuyLimit)} is updated with price {set.MakerBuyLimit.OrderPrice}");
				}
			}
			else if (set.MakerBuyLimit != null)
			{
				set.MakerConnector.CancelLimit(set.MakerBuyLimit);
				set.MakerBuyLimit = null;
				Logger.Debug(
					$"MMStrategyService {set} profit is under {nameof(set.MinProfitabilityInTick)} so cancelled {nameof(set.MakerBuyLimit)}");
			}

			if (set.LastMakerTick.Ask - set.LastTakerTick.Ask >= set.MinProfitability)
			{
				if (set.MakerSellLimit == null)
				{
					set.MakerSellLimit = set.MakerConnector
						.PutNewOrderRequest(set.MakerSymbol, Sides.Sell, set.OrderSize, set.LastMakerTick.Ask).Result;
					Logger.Debug(
						$"MMStrategyService {set} new {nameof(set.MakerSellLimit)} is created with price {set.MakerSellLimit.OrderPrice}");
				}
				else if (set.MakerSellLimit.OrderPrice != set.LastMakerTick.Ask)
				{
					set.MakerConnector.ChangeLimitPrice(set.MakerSellLimit, set.LastMakerTick.Ask);
					Logger.Debug(
						$"MMStrategyService {set} {nameof(set.MakerSellLimit)} is updated with price {set.MakerSellLimit.OrderPrice}");
				}
			}
			else if (set.MakerSellLimit != null)
			{
				set.MakerConnector.CancelLimit(set.MakerSellLimit);
				set.MakerSellLimit = null;
				Logger.Debug(
					$"MMStrategyService {set} profit is under {nameof(set.MinProfitabilityInTick)} so cancelled {nameof(set.MakerSellLimit)}");
			}
		}

		/// <inheritdoc/>
		protected override bool IsBackTester(MM set) =>
			set.MakerAccount.BacktesterAccountId.HasValue;

		/// <inheritdoc/>
		protected override void OnTickProcessed(MM strategy) =>
			strategy.MakerAccount.Connector.OnTickProcessed();

		/// <summary>
		/// Limit fill event handler
		/// </summary>
		/// <param name="set">Set of a trading strategy</param>
		/// <param name="account">Account</param>
		/// <param name="limitFill">Limit fill</param>
		private void OnLimitFill(MM set, Account account, LimitFill limitFill)
		{
			if (account != set.MakerAccount) return;
			set.TakerConnector.SendMarketOrderRequest(set.TakerSymbol, limitFill.LimitResponse.Side.Inv(), set.OrderSize);
		}
	}
}

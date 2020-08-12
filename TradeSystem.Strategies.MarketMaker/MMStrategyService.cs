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
		protected override void Check(MM strategy, CancellationToken token)
		{
			while (strategy.LimitFills.TryDequeue(out var e))
				OnLimitFill(strategy, e.Account, e.LimitFill);
		}

		/// <inheritdoc/>
		protected override bool IsBackTester(MM strategy) =>
			strategy.MakerAccount.BacktesterAccountId.HasValue;

		/// <inheritdoc/>
		protected override void OnTickProcessed(MM strategy) =>
			strategy.MakerAccount.Connector.OnTickProcessed();

		/// <summary>
		/// Limit fill event handler
		/// </summary>
		/// <param name="strategy">Set of a trading strategy</param>
		/// <param name="account">Account</param>
		/// <param name="limitFill">Limit fill</param>
		private void OnLimitFill(MM strategy, Account account, LimitFill limitFill)
		{
			if (account != strategy.MakerAccount) return;

			var connector = (IFixConnector)strategy.TakerAccount.Connector;
			connector.SendMarketOrderRequest(strategy.TakerSymbol, limitFill.LimitResponse.Side.Inv(), strategy.OrderSize);
		}
	}
}

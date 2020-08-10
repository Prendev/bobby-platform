using System.Threading;
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
		}

		/// <inheritdoc/>
		protected override bool IsBackTester(MM strategy) =>
			strategy.MakerAccount.BacktesterAccountId.HasValue;

		/// <inheritdoc/>
		protected override void OnTickProcessed(MM strategy) =>
			strategy.MakerAccount.Connector.OnTickProcessed();
	}
}

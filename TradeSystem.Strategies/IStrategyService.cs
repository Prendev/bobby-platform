using TradeSystem.Data.Models;

namespace TradeSystem.Strategies
{
	/// <summary>
	/// Strategy service interface
	/// </summary>
	/// <typeparam name="T">Strategy entity type</typeparam>
	public interface IStrategyService<in T> where T : StrategyEntityBase
	{
		/// <summary>
		/// Start trading with set
		/// </summary>
		/// <param name="strategy">Set of a trading strategy</param>
		void Start(T strategy);

		/// <summary>
		/// Suspend trading with set
		/// </summary>
		/// <param name="strategy">Set of a trading strategy</param>
		void Suspend(T strategy);
	}
}

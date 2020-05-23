using TradeSystem.Data.Models;

namespace TradeSystem.Backtester
{
	public interface IBacktesterService
	{
		void Start(BacktesterAccount account);
		void Pause(BacktesterAccount account);
		void Stop(BacktesterAccount account);
	}

	public class BacktesterService : IBacktesterService
	{
		public void Start(BacktesterAccount account)
		{
		}
		public void Pause(BacktesterAccount account)
		{
		}
		public void Stop(BacktesterAccount account)
		{
		}
	}
}

using System.Linq;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services
{
	public interface IBacktesterService
	{
		void Start(BacktesterAccount account);
		void Pause(BacktesterAccount account);
		void Stop(BacktesterAccount account);
	}

	public class BacktesterService : IBacktesterService
	{
		public void Start(BacktesterAccount account) =>
			(account.Accounts.FirstOrDefault()?.Connector as Backtester.Connector)?.Start();

		public void Pause(BacktesterAccount account) =>
			(account.Accounts.FirstOrDefault()?.Connector as Backtester.Connector)?.Pause();

		public void Stop(BacktesterAccount account) =>
			(account.Accounts.FirstOrDefault()?.Connector as Backtester.Connector)?.Stop();
	}
}

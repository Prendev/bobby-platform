using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface IRiskManagementStrategyService
	{
		void Start(List<RiskManagement> riskManagements, int throttlingInSec);
		void Stop();
		void SetThrottling(int throttlingInSec);
	}
	public class RiskManagementStrategyService : BaseStrategyService, IRiskManagementStrategyService
	{
		private volatile CancellationTokenSource _cancellation;

		public void Start(List<RiskManagement> riskManagements, int throttlingInSec)
		{
			_throttlingInSec = throttlingInSec;
			_cancellation?.Dispose();

			_cancellation = new CancellationTokenSource();
			new Thread(() => SetLoop(riskManagements, _cancellation.Token)) { Name = $"RiskManagement_strategy", IsBackground = true }.Start();

			Logger.Info("Risk Management is started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("Risk Management is stopped");
		}

		private void SetLoop(List<RiskManagement> riskManagements, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					foreach (var riskManagement in riskManagements)
					{
						riskManagement.HighestTicketDuration = GetHighetTicketDuration(riskManagement.Account);
						riskManagement.NumTicketsHighDuration = GetNumTicketsHighDuration(riskManagement.Account);
					}

					Thread.Sleep(_throttlingInSec * 1000);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("TradesService.Loop exception", e);
				}
			}
		}

		private int? GetHighetTicketDuration(Account account)
		{
			var opennedPositions = account.Connector.Positions.Where(p => !p.Value.IsClosed);
			if (opennedPositions.Any())
			{
				return opennedPositions.Max(p => DateTime.Now - p.Value.OpenTime).Days;
			}
			return null;
		}

		private int? GetNumTicketsHighDuration(Account account)
		{
			var opennedPositions = account.Connector.Positions.Where(p => !p.Value.IsClosed);
			if (opennedPositions.Any())
			{
				return opennedPositions.Count(p => (DateTime.Now - p.Value.OpenTime).Days == account.RiskManagement.HighestTicketDuration);
			}
			return null;
		}
	}
}

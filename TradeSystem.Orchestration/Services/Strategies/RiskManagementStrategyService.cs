using System;
using System.Collections.Generic;
using System.Linq;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface IRiskManagementStrategyService
	{
		void UpdateHighestDurationForOpenPositions(List<RiskManagement> riskManagements);
	}
	public class RiskManagementStrategyService : IRiskManagementStrategyService
	{
		public void UpdateHighestDurationForOpenPositions(List<RiskManagement> riskManagements)
		{
			foreach (var riskManagement in riskManagements)
			{
				riskManagement.HighestTicketDuration = GetHighetTicketDuration(riskManagement.Account);
				riskManagement.NumTicketsHighDuration = GetNumTicketsHighDuration(riskManagement.Account);
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

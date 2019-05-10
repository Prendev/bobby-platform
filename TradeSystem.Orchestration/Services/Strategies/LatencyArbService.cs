using System.Collections.Generic;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface ILatencyArbService
	{
		void Start(List<LatencyArb> sets);
		void Stop();
	}

	public class LatencyArbService : ILatencyArbService
	{
		public void Start(List<LatencyArb> sets)
		{
		}

		public void Stop()
		{
		}
	}
}

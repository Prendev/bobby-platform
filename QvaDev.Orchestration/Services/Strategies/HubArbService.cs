using System.Collections.Generic;
using log4net;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration.Services.Strategies
{
	public interface IHubArbService
	{
		void Start(List<StratHubArb> arbs);
		void Stop();
	}

	public class HubArbService : IHubArbService
	{
		private bool _isStarted;
		private readonly ILog _log;
		private List<StratHubArb> _arbs;

		public HubArbService(ILog log)
		{
			_log = log;
			_isStarted = true;
			_log.Info("Hub arbs are started");
		}

		public void Start(List<StratHubArb> arbs)
		{
			_arbs = arbs;
		}

		public void Stop()
		{
			_isStarted = false;
		}
	}
}

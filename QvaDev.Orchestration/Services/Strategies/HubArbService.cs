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

			foreach (var arb in _arbs)
			{
				arb.GroupQuote -= Aggregator_GroupQuote;
				arb.GroupQuote += Aggregator_GroupQuote;
			}

			_isStarted = true;
			_log.Info("Hub arbs are started");
		}

		private void Aggregator_GroupQuote(object sender, Communication.FixApi.GroupQuoteEventArgs e)
		{
			if (!_isStarted) return;
			var arb = (StratHubArb)sender;

			if (!arb.Run) return;
			if (arb.IsBusy) return;
		}

		public void Stop()
		{
			_isStarted = false;
		}
	}
}

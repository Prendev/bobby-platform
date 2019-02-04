using System.Linq;
using System.Threading.Tasks;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration
{
    public partial interface IOrchestrator
    {
		void SendPushingFuturesOrder(Pushing pushing, Sides side, decimal contractSize);

		Task OpeningBeta(Pushing pushing);
		Task OpeningAlpha(Pushing pushing);
	    Task OpeningPull(Pushing pushing);
		Task OpeningFinish(Pushing pushing);

		Task ClosingFirst(Pushing pushing);
	    Task ClosingPull(Pushing pushing);
		Task OpeningHedge(Pushing pushing);
		Task ClosingSecond(Pushing pushing);
		Task ClosingFinish(Pushing pushing);

        void PushingPanic(Pushing pushing);

	    Task StartStrategies(DuplicatContext duplicatContext);
	    void StopStrategies();

	    Task HubArbsGoFlat(DuplicatContext duplicatContext);
	    Task HubArbsExport(DuplicatContext duplicatContext);
    }

    public partial class Orchestrator : IOrchestrator
    {
	    public async void SendPushingFuturesOrder(Pushing pushing, Sides side, decimal contractSize)
	    {
		    var connector = (IFixConnector)pushing.FutureAccount.Connector;
		    var response = await connector.SendMarketOrderRequest(pushing.FutureSymbol, side, contractSize);
		}

		public Task OpeningBeta(Pushing pushing)
		{
			pushing.PushingDetail.OpenedFutures = 0;
			pushing.InPanic = false;
			return Task.Run(() => _pushingService.OpeningBeta(pushing));
		}

	    public Task OpeningPull(Pushing pushing)
	    {
		    pushing.InPanic = false;
		    return Task.Run(() => _pushingService.OpeningPull(pushing));
	    }

		public Task OpeningAlpha(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushingService.OpeningAlpha(pushing));
		}

		public Task OpeningFinish(Pushing pushing)
		{
			pushing.PushingDetail.PriceLimit = null;
			pushing.InPanic = false;
			return Task.Run(() => _pushingService.OpeningFinish(pushing));
		}

		public Task ClosingFirst(Pushing pushing)
		{
			pushing.PushingDetail.OpenedFutures = 0;
			pushing.InPanic = false;
			return Task.Run(() => _pushingService.ClosingFirst(pushing));
		}

	    public Task ClosingPull(Pushing pushing)
	    {
		    pushing.InPanic = false;
		    return Task.Run(() => _pushingService.ClosingPull(pushing));
	    }

		public Task OpeningHedge(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushingService.OpeningHedge(pushing));
		}

		public Task ClosingSecond(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushingService.ClosingSecond(pushing));
		}

		public Task ClosingFinish(Pushing pushing)
		{
			pushing.PushingDetail.PriceLimit = null;
			pushing.InPanic = false;
			return Task.Run(() => _pushingService.ClosingFinish(pushing));
		}

        public void PushingPanic(Pushing pushing)
        {
            pushing.InPanic = true;
        }

	    public async Task StartStrategies(DuplicatContext duplicatContext)
		{
			await Connect(duplicatContext);
			var hubArbs = duplicatContext.StratHubArbs.Local.ToList();
			_hubArbService.Start(hubArbs);
		}

		public void StopStrategies() => _hubArbService.Stop();

		public async Task HubArbsGoFlat(DuplicatContext duplicatContext)
	    {
		    var hubArbs = duplicatContext.StratHubArbs.Local.ToList();
		    await _hubArbService.GoFlat(hubArbs);
		}
	    public async Task HubArbsExport(DuplicatContext duplicatContext)
	    {
		    var arbPositions = duplicatContext.StratHubArbPositions.ToList();
		    await _reportService.HubArbsExport(arbPositions);
	    }
	}
}

using System.Linq;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Data;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration
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

        void Panic(Pushing pushing);

	    Task StartStrategies(DuplicatContext duplicatContext);
	    void StopStrategies();

	    Task HubArbsGoFlat(DuplicatContext duplicatContext);
	    Task HubArbsExport(DuplicatContext duplicatContext);

	    Task OpeningBeta(Spoofing spoofing);
	    Task OpeningBetaEnd(Spoofing spoofing);
		Task OpeningAlpha(Spoofing spoofing);
	    Task OpeningAlphaEnd(Spoofing spoofing);
		Task ClosingFirst(Spoofing spoofing);
	    Task ClosingFirstEnd(Spoofing spoofing);
		Task ClosingSecond(Spoofing spoofing);
	    Task ClosingSecondEnd(Spoofing spoofing);
		void Panic(Spoofing spoofing);
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

        public void Panic(Pushing pushing)
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

	    public Task OpeningBeta(Spoofing spoofing)
		{
			spoofing.InPanic = false;
			return Task.Run(() => _spoofStrategyService.OpeningBeta(spoofing));
		}

	    public Task OpeningBetaEnd(Spoofing spoofing)
		{
			spoofing.InPanic = false;
			return Task.Run(() => _spoofStrategyService.OpeningBetaEnd(spoofing));
		}

	    public Task OpeningAlpha(Spoofing spoofing)
		{
			spoofing.InPanic = false;
			return Task.Run(() => _spoofStrategyService.OpeningAlpha(spoofing));
		}

	    public Task OpeningAlphaEnd(Spoofing spoofing)
		{
			spoofing.InPanic = false;
			return Task.Run(() => _spoofStrategyService.OpeningAlphaEnd(spoofing));
		}

	    public Task ClosingFirst(Spoofing spoofing)
		{
			spoofing.InPanic = false;
			return Task.Run(() => _spoofStrategyService.ClosingFirst(spoofing));
		}

	    public Task ClosingFirstEnd(Spoofing spoofing)
		{
			spoofing.InPanic = false;
			return Task.Run(() => _spoofStrategyService.ClosingFirstEnd(spoofing));
		}

	    public Task ClosingSecond(Spoofing spoofing)
		{
			spoofing.InPanic = false;
			return Task.Run(() => _spoofStrategyService.ClosingSecond(spoofing));
		}

	    public Task ClosingSecondEnd(Spoofing spoofing)
		{
			spoofing.InPanic = false;
			return Task.Run(() => _spoofStrategyService.ClosingSecondEnd(spoofing));
		}

	    public void Panic(Spoofing spoofing)
		{
			spoofing.InPanic = true;
	    }
    }
}

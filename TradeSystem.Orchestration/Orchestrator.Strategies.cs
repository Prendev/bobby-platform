using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Data;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration
{
    public partial interface IOrchestrator
    {
		void SendOrder(Account account, Sides side, string symbol, decimal contractSize);

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
		public async void SendOrder(Account account, Sides side, string symbol, decimal contractSize)
		{
			if (!(account.Connector is IFixConnector connector)) return;
			var response = await connector.SendMarketOrderRequest(symbol, side, contractSize);
		}

		public Task OpeningBeta(Pushing pushing)
		{
			pushing.ReopenPosition = null;
			pushing.PushingDetail.OpenedFutures = 0;
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.OpeningBeta(pushing));
		}

		public Task OpeningPull(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.OpeningPull(pushing));
		}

		public Task OpeningAlpha(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.OpeningAlpha(pushing));
		}

		public Task OpeningFinish(Pushing pushing)
		{
			pushing.PushingDetail.PriceLimit = null;
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.OpeningFinish(pushing));
		}

		public Task ClosingFirst(Pushing pushing)
		{
			pushing.PushingDetail.OpenedFutures = 0;
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.ClosingFirst(pushing));
		}

		public Task ClosingPull(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.ClosingPull(pushing));
		}

		public Task OpeningHedge(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.OpeningHedge(pushing));
		}

		public Task ClosingSecond(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.ClosingSecond(pushing));
		}

		public Task ClosingFinish(Pushing pushing)
		{
			pushing.PushingDetail.PriceLimit = null;
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.ClosingFinish(pushing));
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

			var marketMakers = duplicatContext.MarketMakers.Local.Where(mm => mm.Type == MarketMaker.MarketMakerTypes.Normal).ToList();
			_marketMakerService.Start(marketMakers);
			var antiMarketMakers = duplicatContext.MarketMakers.Local.Where(mm => mm.Type == MarketMaker.MarketMakerTypes.Anti).ToList();
			_antiMarketMakerService.Start(antiMarketMakers);

			var latArbs = duplicatContext.LatencyArbs.Local.ToList();
			_latencyArbService.Start(latArbs);
		}

		public void StopStrategies()
		{
			_hubArbService.Stop();
			_marketMakerService.Stop();
			_antiMarketMakerService.Stop();
			_latencyArbService.Stop();
		}

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

		public async Task OpeningBeta(Spoofing spoofing)
		{
			spoofing.PanicSource = new CancellationTokenSource();
			await Task.Run(() => _spoofStrategyService.OpeningBeta(spoofing, spoofing.PanicSource.Token));
		}

		public async Task OpeningBetaEnd(Spoofing spoofing)
		{
			spoofing.PanicSource = new CancellationTokenSource();
			await Task.Run(() => _spoofStrategyService.OpeningBetaEnd(spoofing, spoofing.PanicSource.Token));
		}

		public async Task OpeningAlpha(Spoofing spoofing)
		{
			spoofing.PanicSource = new CancellationTokenSource();
			await Task.Run(() => _spoofStrategyService.OpeningAlpha(spoofing, spoofing.PanicSource.Token));
		}

		public async Task OpeningAlphaEnd(Spoofing spoofing)
		{
			spoofing.PanicSource = new CancellationTokenSource();
			await Task.Run(() => _spoofStrategyService.OpeningAlphaEnd(spoofing, spoofing.PanicSource.Token));
		}

		public async Task ClosingFirst(Spoofing spoofing)
		{
			spoofing.PanicSource = new CancellationTokenSource();
			await Task.Run(() => _spoofStrategyService.ClosingFirst(spoofing, spoofing.PanicSource.Token));
		}

		public async Task ClosingFirstEnd(Spoofing spoofing)
		{
			spoofing.PanicSource = new CancellationTokenSource();
			await Task.Run(() => _spoofStrategyService.ClosingFirstEnd(spoofing, spoofing.PanicSource.Token));
		}

		public async Task ClosingSecond(Spoofing spoofing)
		{
			spoofing.PanicSource = new CancellationTokenSource();
			await Task.Run(() => _spoofStrategyService.ClosingSecond(spoofing, spoofing.PanicSource.Token));
		}

		public async Task ClosingSecondEnd(Spoofing spoofing)
		{
			spoofing.PanicSource = new CancellationTokenSource();
			await Task.Run(() => _spoofStrategyService.ClosingSecondEnd(spoofing, spoofing.PanicSource.Token));
		}

		public void Panic(Spoofing spoofing)
		{
			spoofing.PanicSource.CancelEx();
		}
	}
}

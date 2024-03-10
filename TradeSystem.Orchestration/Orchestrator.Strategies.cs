using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common;
using TradeSystem.Common.BindingLists;
using TradeSystem.Common.Integration;
using TradeSystem.Communication.Mt5;
using TradeSystem.Data;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration
{
	public partial interface IOrchestrator
	{
		void SendOrder(Account account, Sides side, string symbol, decimal contractSize);

		Task<Sides> LatencyStart(Pushing pushing);
		void LatencyStop(Pushing pushing);

		Task OpeningBeta(Pushing pushing);
		Task OpeningPull(Pushing pushing);
		Task OpeningHedge(Pushing pushing);
		Task OpeningAlpha(Pushing pushing);
		Task OpeningFinish(Pushing pushing);

		Task ClosingFirst(Pushing pushing);
		Task ClosingPull(Pushing pushing);
		Task ClosingHedge(Pushing pushing);
		Task ClosingSecond(Pushing pushing);
		Task ClosingFinish(Pushing pushing);
		void FlipFinish(Pushing pushing);

		void Panic(Pushing pushing);

		Task StartStrategies(DuplicatContext duplicatContext, int throttlingInSec);
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
		void FlipFinish(Spoofing spoofing);
		void Panic(Spoofing spoofing);
		void StartExposureStrategy(SortableBindingList<SymbolStatus> symbolStatuses, int throttlingInSec);
		void StopExposureStrategy();
		void StartTradeStrategy(int throttlingInSec);
		void StopTradeStrategy();
		Task TradePositionClose(TradePosition position);
		void StartRiskManagementStrategy(int throttlingInSec);
		void SetThrottling(int throttlingInSec);
	}

	public partial class Orchestrator : IOrchestrator
	{
		public async void SendOrder(Account account, Sides side, string symbol, decimal contractSize)
		{
			if (!(account.Connector is IFixConnector connector)) return;
			var response = await connector.SendMarketOrderRequest(symbol, side, contractSize);
		}

		public Task<Sides> LatencyStart(Pushing pushing)
		{
			return Task.Run(() => _pushStrategyService.LatencyStart(pushing));
		}
		public void LatencyStop(Pushing pushing) => _pushStrategyService.LatencyStop(pushing);

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

		public Task OpeningHedge(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.OpeningHedge(pushing));
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

		public Task ClosingHedge(Pushing pushing)
		{
			pushing.InPanic = false;
			return Task.Run(() => _pushStrategyService.ClosingHedge(pushing));
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

		public void FlipFinish(Pushing pushing) => _pushStrategyService.FlipFinish(pushing);

		public void Panic(Pushing pushing)
		{
			pushing.InPanic = true;
		}

		public async Task StartStrategies(DuplicatContext duplicatContext, int throttlingInSec)
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

			var newsArbs = duplicatContext.NewsArbs.Local.ToList();
			_newsArbService.Start(newsArbs);

			var mms = duplicatContext.MMs.Local.ToList();
			mms.ForEach(mm => _mmStrategyService.Start(mm));

			_tradeService.Start(duplicatContext, throttlingInSec);

			var riskManagements = _duplicatContext.Accounts.Local.Where(a => a.Connector?.IsConnected == true).Select(a => a.RiskManagement).ToList();
			_riskManagementService.Start(riskManagements, throttlingInSec);
		}

		public void StopStrategies()
		{
			_hubArbService.Stop();
			_marketMakerService.Stop();
			_antiMarketMakerService.Stop();
			_latencyArbService.Stop();
			_newsArbService.Stop();
			_mmStrategyService.SuspendAll();
			_exposureStrategyService.Stop();
			_tradeService.Stop();
			_riskManagementService.Stop();
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

		public void FlipFinish(Spoofing spoofing) => _spoofStrategyService.FlipFinish(spoofing);

		public void Panic(Spoofing spoofing)
		{
			spoofing.PanicSource.CancelEx();
		}

		public void SetThrottling(int throttlingInSec)
		{
			_exposureStrategyService.SetThrottling(throttlingInSec);
			_tradeService.SetThrottling(throttlingInSec);
			_riskManagementService.SetThrottling(throttlingInSec);
		}

		public void StartExposureStrategy(SortableBindingList<SymbolStatus> symbolStatuses, int throttlingInSec)
		{
			var connectedMt4Mt5Accounts = _duplicatContext.Accounts.Local
				.Where(a => a.Connector?.IsConnected == true &&
					(a.MetaTraderAccount != null ||
					(a.FixApiAccount != null &&
					(a.Connector as FixApiIntegration.Connector).GeneralConnector is Mt5Connector)))
				.ToList();

			var mappingTables = _duplicatContext.MappingTables.Local.ToBindingList();
			_exposureStrategyService.Start(connectedMt4Mt5Accounts, mappingTables, symbolStatuses, throttlingInSec);
		}
		public void StopExposureStrategy()
		{
			_exposureStrategyService.Stop();
		}

		public void StartTradeStrategy(int throttlingInSec)
		{
			_tradeService.Start(_duplicatContext, throttlingInSec);
		}

		public void StopTradeStrategy()
		{
			_tradeService.Stop();
		}

		public async Task TradePositionClose(TradePosition position)
		{
			await _tradeService.TradePositionClose(_duplicatContext.TraderPositions.Local.ToBindingList(), position);
		}

		public void StartRiskManagementStrategy(int throttlingInSec)
		{
			var riskManagements = _duplicatContext.Accounts.Local.Where(a => a.Connector?.IsConnected == true).Select(a => a.RiskManagement).ToList();
			_riskManagementService.Start(riskManagements, throttlingInSec);
		}
	}
}

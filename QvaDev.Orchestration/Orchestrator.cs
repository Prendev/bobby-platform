using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication.FixApi;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Orchestration.Services;
using QvaDev.Orchestration.Services.Strategies;

namespace QvaDev.Orchestration
{
    public interface IOrchestrator
    {
        int SelectedAlphaMonitorId { get; set; }
        int SelectedBetaMonitorId { get; set; }

        Task StartCopiers(DuplicatContext duplicatContext);
        void StopCopiers();

		Task StartTickers(DuplicatContext duplicatContext);
		void StopTickers();

		Task Connect(DuplicatContext duplicatContext);
        Task Disconnect();

        Task OrderHistoryExport(DuplicatContext duplicatContext);
	    void MtAccountImport(DuplicatContext duplicatContext);
	    Task SaveTheWeekend(DuplicatContext duplicatContext);


		void TestMarketOrder(Pushing pushing);
		void TestLimitOrder(Pushing pushing);

		Task OpeningBeta(Pushing pushing);
		Task OpeningAlpha(Pushing pushing);
		Task OpeningFinish(Pushing pushing);

		Task ClosingFirst(Pushing pushing);
		Task OpeningHedge(Pushing pushing);
		Task ClosingSecond(Pushing pushing);
		Task ClosingFinish(Pushing pushing);

        void PushingPanic(Pushing pushing);

	    Task StartStrategies(DuplicatContext duplicatContext);
	    void StopStrategies();
	}

    public class Orchestrator : IOrchestrator
    {
        private SynchronizationContext _synchronizationContext;
        private DuplicatContext _duplicatContext;
        private readonly Func<SynchronizationContext> _synchronizationContextFactory;
        private readonly ICopierService _copierService;
        private readonly IPushingService _pushingService;
		private readonly ITickerService _tickerService;
	    private readonly IStrategiesService _strategiesService;
	    private readonly IConnectorFactory _connectorFactory;
	    private readonly IReportService _reportService;
	    private readonly IMtAccountImportService _mtAccountImportService;
	    private readonly ILog _log;

	    public int SelectedAlphaMonitorId { get; set; }
        public int SelectedBetaMonitorId { get; set; }

        public Orchestrator(
            Func<SynchronizationContext> synchronizationContextFactory,
			IConnectorFactory connectorFactory,
            ICopierService copierService,
            IPushingService pushingService,
			ITickerService tickerService,
            IStrategiesService strategiesService,
			IReportService reportService,
            IMtAccountImportService mtAccountImportService,
            ILog log)
        {
	        _log = log;
	        _mtAccountImportService = mtAccountImportService;
	        _reportService = reportService;
	        _connectorFactory = connectorFactory;
	        _strategiesService = strategiesService;
	        _tickerService = tickerService;
            _pushingService = pushingService;
            _copierService = copierService;
            _synchronizationContextFactory = synchronizationContextFactory;
        }

        public async Task Connect(DuplicatContext duplicatContext)
        {
            _duplicatContext = duplicatContext;
            _synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();

	        var accounts = _duplicatContext.Accounts.Local
		        .Where(pa => pa.Run).ToList()
		        .Where(pa => pa.ConnectionState != ConnectionStates.Connected);

	        var tasks = accounts.Select(account => Task.Run(() => _connectorFactory.Create(account))).ToList();

	        await Task.WhenAll(tasks);

	        foreach (var agg in _duplicatContext.Aggregators)
	        {
		        var groups = agg.Accounts
			        .Where(a => a.Account.FixApiAccountId.HasValue)
			        .Select(a =>
				        new
				        {
					        ((FixApiIntegration.Connector) a.Account.Connector).FixConnector,
					        Symbol = Symbol.Parse(a.Symbol)
				        }).ToDictionary(x => x.FixConnector, x => x.Symbol);

				agg.QuoteAggregator = MarketDataManager.CreateQuoteAggregator(groups);
			}
        }

		public Task Disconnect()
        {
            _duplicatContext.SaveChanges();

	        foreach (var agg in _duplicatContext.Aggregators)
	        {
		        agg.QuoteAggregator?.Dispose();
		        agg.QuoteAggregator = null;
	        }

			var accounts = _duplicatContext.Accounts.Local.ToList();
			var tasks = accounts.Select(pa => Task.Run(() => pa.Connector?.Disconnect()));

			return Task.WhenAll(tasks);
		}

        public async Task StartCopiers(DuplicatContext duplicatContext)
		{
			await Connect(duplicatContext);

			var copiers = duplicatContext.Copiers.Local
				.Where(c => c.Slave.Master.Account.ConnectionState == ConnectionStates.Connected)
				.Where(c => c.Slave.Account.ConnectionState == ConnectionStates.Connected)
				.Select(c => c.Slave.Master);

			var fixApiCopiers = duplicatContext.FixApiCopiers.Local
				.Where(c => c.Slave.Master.Account.ConnectionState == ConnectionStates.Connected)
				.Where(c => c.Slave.Account.ConnectionState == ConnectionStates.Connected)
				.Select(c => c.Slave.Master);

			var masters = copiers.Union(fixApiCopiers).Distinct().ToList();

			_copierService.Start(masters);
        }

        public void StopCopiers()
        {
            _copierService.Stop();
        }

        public void TestMarketOrder(Pushing pushing)
        {
            var connector = (IFixConnector)pushing.FutureAccount.Connector;
            connector.SendMarketOrderRequest(pushing.FutureSymbol, Sides.Buy, pushing.PushingDetail.SmallContractSize);
		}
		public void TestLimitOrder(Pushing pushing)
		{
			var connector = (FixTraderIntegration.Connector)pushing.FutureAccount.Connector;
			connector.SendLimitOrderRequest(pushing.FutureSymbol, Sides.Buy, pushing.PushingDetail.SmallContractSize, 0);
		}

		public Task OpeningBeta(Pushing pushing)
		{
			pushing.PushingDetail.OpenedFutures = 0;
			pushing.InPanic = false;
			return Task.Run(() => _pushingService.OpeningBeta(pushing));
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
			pushing.InPanic = false;
			return Task.Run(() => _pushingService.ClosingFirst(pushing));
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

			var arbs = duplicatContext.StratDealingArbs.Local
				.Where(c => c.AlphaAccount?.ConnectionState == ConnectionStates.Connected &&
				            c.BetaAccount?.ConnectionState == ConnectionStates.Connected)
				.ToList();

			_strategiesService.Start(arbs);
		}

	    public void StopStrategies()
	    {
		    _strategiesService.Stop();
		}

	    public async Task OrderHistoryExport(DuplicatContext duplicatContext)
	    {
		    await Connect(duplicatContext);

	        var accounts = duplicatContext.Accounts.Local
		        .Where(a => a.Run && a.Connector?.IsConnected == true && a.MetaTraderAccountId.HasValue)
		        .ToList();

		    await _reportService.OrderHistoryExport(accounts);
	    }

	    public void MtAccountImport(DuplicatContext duplicatContext)
	    {
		    _mtAccountImportService.Import(duplicatContext);
		}

	    public async Task SaveTheWeekend(DuplicatContext duplicatContext)
	    {
		    await Connect(duplicatContext);
		    _mtAccountImportService.SaveTheWeekend(duplicatContext);
		}

		public async Task StartTickers(DuplicatContext duplicatContext)
		{
			await Connect(duplicatContext);

			var tickers = duplicatContext.Tickers.Local
				.Where(c => c.MainAccount.ConnectionState == ConnectionStates.Connected)
				.ToList();

			_tickerService.Start(tickers);
		}

		public void StopTickers()
		{
			_tickerService.Stop();
		}
	}
}

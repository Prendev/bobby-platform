using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication;
using QvaDev.Communication.FixApi;
using QvaDev.Data;
using QvaDev.Orchestration.Services;
using QvaDev.Orchestration.Services.Strategies;

namespace QvaDev.Orchestration
{
    public partial interface IOrchestrator
	{
		Task Connect(DuplicatContext duplicatContext);
		Task Disconnect();

		Task StartCopiers(DuplicatContext duplicatContext);
        void StopCopiers();

		Task StartTickers(DuplicatContext duplicatContext);
		void StopTickers();

        Task OrderHistoryExport(DuplicatContext duplicatContext);
	    void MtAccountImport(DuplicatContext duplicatContext);
		Task SaveTheWeekend(DuplicatContext duplicatContext, DateTime from, DateTime to);
    }

    public partial class Orchestrator : IOrchestrator
    {
        private SynchronizationContext _synchronizationContext;
        private DuplicatContext _duplicatContext;
        private readonly Func<SynchronizationContext> _synchronizationContextFactory;
        private readonly ICopierService _copierService;
        private readonly IPushingService _pushingService;
		private readonly ITickerService _tickerService;
	    private readonly IHubArbService _hubArbService;
		private readonly IConnectorFactory _connectorFactory;
	    private readonly IReportService _reportService;
	    private readonly IMtAccountImportService _mtAccountImportService;
	    private readonly ILog _log;

        public Orchestrator(
            Func<SynchronizationContext> synchronizationContextFactory,
			IConnectorFactory connectorFactory,
            ICopierService copierService,
            IPushingService pushingService,
			ITickerService tickerService,
            IHubArbService hubArbService,
			IReportService reportService,
            IMtAccountImportService mtAccountImportService,
            ILog log)
        {
	        _log = log;
	        _mtAccountImportService = mtAccountImportService;
	        _reportService = reportService;
	        _connectorFactory = connectorFactory;
	        _hubArbService = hubArbService;
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

	        foreach (var agg in _duplicatContext.Aggregators.Where(a => a.Run))
	        {
		        var aggAccounts = agg.Accounts
			        .Where(a => a.Account.Run)
			        .Where(a => a.Account.FixApiAccountId.HasValue)
			        .Where(a => a.Account.Connector is FixApiIntegration.Connector)
			        .ToList();

		        foreach (var aggAccount in aggAccounts)
			        aggAccount.Account.Connector.Subscribe(aggAccount.Symbol);

				var groups = aggAccounts.Select(a =>
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
		public void StopCopiers() => _copierService.Stop();

		public async Task StartTickers(DuplicatContext duplicatContext)
		{
			await Connect(duplicatContext);

			var tickers = duplicatContext.Tickers.Local
				.Where(c => c.MainAccount.ConnectionState == ConnectionStates.Connected)
				.ToList();

			_tickerService.Start(tickers);
		}
		public void StopTickers() => _tickerService.Stop();

		public async Task OrderHistoryExport(DuplicatContext duplicatContext)
		{
			await Connect(duplicatContext);

			var accounts = duplicatContext.Accounts.Local
				.Where(a => a.Run && a.Connector?.IsConnected == true && a.MetaTraderAccountId.HasValue)
				.ToList();

			await _reportService.OrderHistoryExport(accounts);
		}
		public void MtAccountImport(DuplicatContext duplicatContext) => _mtAccountImportService.Import(duplicatContext);
		public async Task SaveTheWeekend(DuplicatContext duplicatContext, DateTime from, DateTime to)
		{
			await Connect(duplicatContext);
			_mtAccountImportService.SaveTheWeekend(duplicatContext, from, to);
		}
	}
}

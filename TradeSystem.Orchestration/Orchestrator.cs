﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;
using TradeSystem.Communication.FixApi;
using TradeSystem.Communication.Interfaces;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using TradeSystem.Orchestration.Services;
using TradeSystem.Orchestration.Services.Strategies;
using IConnector = TradeSystem.Communication.Interfaces.IConnector;

namespace TradeSystem.Orchestration
{
    public partial interface IOrchestrator
	{
		Task Connect(DuplicatContext duplicatContext);
		Task Disconnect();
		Task HeatUp();

		Task StartCopiers(DuplicatContext duplicatContext);
        void StopCopiers();
		Task CopierSync(Slave slave);
		Task CopierClose(Slave slave);

		Task StartTickers(DuplicatContext duplicatContext);
		void StopTickers();

        Task OrderHistoryExport(DuplicatContext duplicatContext);
		void MtAccountImport(DuplicatContext duplicatContext);
		void SaveTheWeekend(DuplicatContext duplicatContext, DateTime from, DateTime to);
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
	    private readonly IProxyService _proxyService;

	    public Orchestrator(
            Func<SynchronizationContext> synchronizationContextFactory,
			IConnectorFactory connectorFactory,
            ICopierService copierService,
            IPushingService pushingService,
			ITickerService tickerService,
            IHubArbService hubArbService,
			IReportService reportService,
            IMtAccountImportService mtAccountImportService,
			IProxyService proxyService)
        {
	        _proxyService = proxyService;
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
		        .Where(pa => pa.ConnectionState != ConnectionStates.Connected)
		        .ToList();

	        _proxyService.Start(duplicatContext.ProfileProxies.Local.Where(a => a.Run).ToList(), accounts);

			var tasks = accounts.Select(account => Task.Run(() => _connectorFactory.Create(account))).ToList();

	        await Task.WhenAll(tasks);

	        foreach (var agg in _duplicatContext.Aggregators.Local.Where(a => a.Run))
	        {
		        var aggAccounts = agg.Accounts
			        .Where(a => a.Account?.Run == true)
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
				        }).ToDictionary(x => x.FixConnector as IConnector, x => x.Symbol);

				agg.QuoteAggregator = MarketDataManager.CreateQuoteAggregator(groups);
			}
        }
		public async Task Disconnect()
        {
            _duplicatContext.SaveChanges();

	        _proxyService.Stop();
			foreach (var agg in _duplicatContext.Aggregators.Local)
	        {
		        agg.QuoteAggregator?.Dispose();
		        agg.QuoteAggregator = null;
	        }

			var accounts = _duplicatContext.Accounts.Local.ToList();
			var tasks = accounts.Select(pa => Task.Run(() => pa.Connector?.Disconnect()));

			await Task.WhenAll(tasks);
		}

	    public async Task HeatUp()
	    {
		    var accounts = _duplicatContext.Accounts.Local
			    .Where(a => a.FixApiAccountId.HasValue)
			    .Where(a => a.Connector?.IsConnected == true)
			    .ToList();

		    var tasks = accounts.Select(pa => Task.Run(() => (pa.Connector as FixApiIntegration.Connector)?.HeatUp()));

		    await Task.WhenAll(tasks);
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
		public Task CopierSync(Slave slave) => _copierService.Sync(slave);
		public Task CopierClose(Slave slave) => _copierService.Close(slave);

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
			var accounts = duplicatContext.Accounts.Local
				.Where(a => a.Run && a.Connector?.IsConnected == true && a.MetaTraderAccountId.HasValue)
				.ToList();

			await _reportService.OrderHistoryExport(accounts);
		}
		public void MtAccountImport(DuplicatContext duplicatContext) => _mtAccountImportService.Import(duplicatContext);
	    public void SaveTheWeekend(DuplicatContext duplicatContext, DateTime from, DateTime to) =>
		    _mtAccountImportService.SaveTheWeekend(duplicatContext, from, to);
    }
}
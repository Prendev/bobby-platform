using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
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
        private readonly ILog _log;
        private readonly CTraderIntegration.IConnectorFactory _connectorFactory;
        private readonly ICopierService _copierService;
        private readonly IPushingService _pushingService;
        private readonly IReportService _reportService;
		private readonly ITickerService _tickerService;
	    private readonly IStrategiesService _strategiesService;

	    public int SelectedAlphaMonitorId { get; set; }
        public int SelectedBetaMonitorId { get; set; }

        public Orchestrator(
            Func<SynchronizationContext> synchronizationContextFactory,
            CTraderIntegration.IConnectorFactory connectorFactory,
            ICopierService copierService,
            IPushingService pushingService,
            IReportService reportService,
			ITickerService tickerService,
            IStrategiesService strategiesService,
			ILog log)
        {
	        _strategiesService = strategiesService;
	        _tickerService = tickerService;
            _pushingService = pushingService;
            _copierService = copierService;
            _synchronizationContextFactory = synchronizationContextFactory;
            _connectorFactory = connectorFactory;
            _log = log;
        }

        public Task Connect(DuplicatContext duplicatContext)
        {
            _duplicatContext = duplicatContext;
            _synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
            return Task.WhenAll(ConnectMtAccounts(), ConenctCtAccounts(), ConnectFtAccounts());
        }

		private Task ConnectMtAccounts()
		{
			var accounts = _duplicatContext.Accounts
				.Where(pa => pa.Run && pa.State != Account.States.Connected && pa.MetaTraderAccountId.HasValue)
				.AsEnumerable();
			var tasks = accounts.Select(pa => Task.Factory.StartNew(() =>
			{
				var connector = pa.Connector as Mt4Integration.Connector;
				if (connector == null)
				{
					connector = new Mt4Integration.Connector(_log);
					pa.Connector = connector;
				}
				var connected = connector.Connect(new Mt4Integration.AccountInfo()
				{
					DbId = pa.MetaTraderAccount.Id,
					Description = pa.MetaTraderAccount.Description,
					User = (uint)pa.MetaTraderAccount.User,
					Password = pa.MetaTraderAccount.Password,
					Srv = pa.MetaTraderAccount.MetaTraderPlatform.SrvFilePath
				});
				pa.State = connected ? Account.States.Connected : Account.States.Error;
				pa.RaisePropertyChanged(_synchronizationContext, nameof(pa.State));
			}));

			return Task.WhenAll(tasks);
		}

        private Task ConenctCtAccounts()
		{
			var accounts = _duplicatContext.Accounts
				.Where(pa => pa.Run && pa.State != Account.States.Connected && pa.CTraderAccountId.HasValue)
				.AsEnumerable();
			var tasks = accounts.Select(pa => Task.Factory.StartNew(() =>
			{
                var connector = pa.Connector as CTraderIntegration.Connector;
                if (connector == null)
                {
                    connector = (CTraderIntegration.Connector) _connectorFactory.Create(
                        new CTraderIntegration.PlatformInfo
                        {
                            Description = pa.CTraderAccount.CTraderPlatform.Description,
                            AccountsApi = pa.CTraderAccount.CTraderPlatform.AccountsApi,
                            ClientId = pa.CTraderAccount.CTraderPlatform.ClientId,
                            TradingHost = pa.CTraderAccount.CTraderPlatform.TradingHost,
                            Secret = pa.CTraderAccount.CTraderPlatform.Secret,
                            Playground = pa.CTraderAccount.CTraderPlatform.Playground
                        },
                        new CTraderIntegration.AccountInfo
                        {
                            DbId = pa.CTraderAccount.Id,
                            Description = pa.CTraderAccount.Description,
                            AccountNumber = pa.CTraderAccount.AccountNumber,
                            AccessToken = pa.CTraderAccount.AccessToken
                        });
					pa.Connector = connector;
                }
                var connected = connector.Connect();
				pa.State = connected ? Account.States.Connected : Account.States.Error;
				pa.RaisePropertyChanged(_synchronizationContext, nameof(pa.State));
            }));

            return Task.WhenAll(tasks);
        }

        private Task ConnectFtAccounts()
        {
			var accounts = _duplicatContext.Accounts
				.Where(pa => pa.Run && pa.State != Account.States.Connected && pa.FixTraderAccountId.HasValue)
				.AsEnumerable();
			var tasks = accounts.Select(pa => Task.Factory.StartNew(() =>
			{
                var connector = pa.Connector as FixTraderIntegration.Connector;
                if (connector == null)
                {
                    connector = new FixTraderIntegration.Connector(_log);
					pa.Connector = connector;
                }
                var connected = connector.Connect(new FixTraderIntegration.AccountInfo
                {
                    Description = pa.FixTraderAccount.Description,
                    IpAddress = pa.FixTraderAccount.IpAddress,
                    CommandSocketPort = pa.FixTraderAccount.CommandSocketPort,
                    EventsSocketPort = pa.FixTraderAccount.EventsSocketPort
                });
				pa.State = connected ? Account.States.Connected : Account.States.Error;
				pa.RaisePropertyChanged(_synchronizationContext, nameof(pa.State));
            }));

            return Task.WhenAll(tasks);
        }

        public Task Disconnect()
        {
            _duplicatContext.SaveChanges();
			var accounts = _duplicatContext.Accounts
				 .Where(pa => pa.State != Account.States.Disconnected)
				 .AsEnumerable();
			var tasks = accounts.Select(pa => Task.Factory.StartNew(() =>
			{
				pa.Connector.Disconnect();
				pa.State = Account.States.Disconnected;
				pa.RaisePropertyChanged(_synchronizationContext, nameof(pa.State));
			}));
			return Task.WhenAll(tasks);
		}

        public async Task StartCopiers(DuplicatContext duplicatContext)
        {
            await Connect(duplicatContext);
            _copierService.Start(duplicatContext);
        }

        public void StopCopiers()
        {
            _copierService.Stop();
        }

        public void TestMarketOrder(Pushing pushing)
        {
            var connector = (FixTraderIntegration.Connector)pushing.FutureAccount.Connector;
            connector.SendMarketOrderRequest(pushing.FutureSymbol, Common.Integration.Sides.Buy, pushing.PushingDetail.SmallContractSize);
		}
		public void TestLimitOrder(Pushing pushing)
		{
			var connector = (FixTraderIntegration.Connector)pushing.FutureAccount.Connector;
			connector.SendLimitOrderRequest(pushing.FutureSymbol, Common.Integration.Sides.Buy, pushing.PushingDetail.SmallContractSize, 0);
		}

		public Task OpeningBeta(Pushing pushing)
		{
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
			_strategiesService.Start(duplicatContext);
		}

	    public void StopStrategies()
	    {
		    _strategiesService.Stop();
		}

	    public Task OrderHistoryExport(DuplicatContext duplicatContext)
        {
            return Connect(duplicatContext).ContinueWith(prevTask =>
            {
                _reportService.OrderHistoryExport(duplicatContext);
            });
		}

		public async Task StartTickers(DuplicatContext duplicatContext)
		{
			await Connect(duplicatContext);
			_tickerService.Start(duplicatContext);
		}

		public void StopTickers()
		{
			_tickerService.Stop();
		}
	}
}

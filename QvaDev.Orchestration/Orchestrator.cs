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
        Task StartMonitors(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId);
        void StopMonitors();
        Task StartExperts(DuplicatContext duplicatContext);
        void StopExperts();
		Task StartTickers(DuplicatContext duplicatContext);
		void StopTickers();
		Task Connect(DuplicatContext duplicatContext);
        Task Disconnect();
        Task BalanceReport(DateTime from, DateTime to, string reportPath);
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
        private readonly IBalanceReportService _balanceReportService;
        private readonly ICopierService _copierService;
        private readonly IMonitorServices _monitorServices;
        private readonly IFrpService _frpService;
        private readonly IPushingService _pushingService;
        private readonly IReportService _reportService;
		private readonly ITickerService _tickerService;
	    private readonly IStrategiesService _strategiesService;

	    public int SelectedAlphaMonitorId { get; set; }
        public int SelectedBetaMonitorId { get; set; }

        public Orchestrator(
            Func<SynchronizationContext> synchronizationContextFactory,
            CTraderIntegration.IConnectorFactory connectorFactory,
            IBalanceReportService balanceReportService,
            ICopierService copierService,
            IMonitorServices monitorServices,
            IFrpService frpService,
            IPushingService pushingService,
            IReportService reportService,
			ITickerService tickerService,
            IStrategiesService strategiesService,
			ILog log)
        {
	        _strategiesService = strategiesService;
	        _tickerService = tickerService;
            _pushingService = pushingService;
            _frpService = frpService;
            _monitorServices = monitorServices;
            _copierService = copierService;
            _balanceReportService = balanceReportService;
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
            var tasks = _duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (!account.ShouldConnect) return;
                    if (account.State == BaseAccountEntity.States.Connected) return;
                    var connector = account.Connector as Mt4Integration.Connector;
                    if (connector == null)
                    {
                        connector = new Mt4Integration.Connector(_log);
                        account.Connector = connector;
                    }
                    var connected = connector.Connect(new Mt4Integration.AccountInfo()
                    {
                        DbId = account.Id,
                        Description = account.Description,
                        User = (uint) account.User,
                        Password = account.Password,
                        Srv = account.MetaTraderPlatform.SrvFilePath
                    });
                    account.State = connected
                        ? BaseAccountEntity.States.Connected
                        : BaseAccountEntity.States.Error;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
                }));

            return Task.WhenAll(tasks);
        }

        private Task ConenctCtAccounts()
        {
            var tasks = _duplicatContext.CTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (!account.ShouldConnect) return;
                    if (account.State == BaseAccountEntity.States.Connected) return;
                    var connector = account.Connector as CTraderIntegration.Connector;
                    if (connector == null)
                    {
                        connector = (CTraderIntegration.Connector) _connectorFactory.Create(
                            new CTraderIntegration.PlatformInfo
                            {
                                Description = account.CTraderPlatform.Description,
                                AccountsApi = account.CTraderPlatform.AccountsApi,
                                ClientId = account.CTraderPlatform.ClientId,
                                TradingHost = account.CTraderPlatform.TradingHost,
                                Secret = account.CTraderPlatform.Secret,
                                Playground = account.CTraderPlatform.Playground
                            },
                            new CTraderIntegration.AccountInfo
                            {
                                DbId = account.Id,
                                Description = account.Description,
                                AccountNumber = account.AccountNumber,
                                AccessToken = account.AccessToken
                            });
                        account.Connector = connector;
                    }
                    var connected = connector.Connect();
                    account.State = connected
                        ? BaseAccountEntity.States.Connected
                        : BaseAccountEntity.States.Error;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
                }));

            return Task.WhenAll(tasks);
        }

        private Task ConnectFtAccounts()
        {
            var tasks = _duplicatContext.FixTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (!account.ShouldConnect) return;
                    if (account.State == BaseAccountEntity.States.Connected) return;
                    var connector = account.Connector as FixTraderIntegration.Connector;
                    if (connector == null)
                    {
                        connector = new FixTraderIntegration.Connector(_log);
                        account.Connector = connector;
                    }
                    var connected = connector.Connect(new FixTraderIntegration.AccountInfo
                    {
                        Description = account.Description,
                        IpAddress = account.IpAddress,
                        CommandSocketPort = account.CommandSocketPort,
                        EventsSocketPort = account.EventsSocketPort
                    });
                    account.State = connected
                        ? BaseAccountEntity.States.Connected
                        : BaseAccountEntity.States.Error;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
                }));

            return Task.WhenAll(tasks);
        }

        public Task Disconnect()
        {
            _duplicatContext.SaveChanges();
			return Task.WhenAll(DisconnectMtAccounts(), DisconnectCtAccounts(), DisconnectFtAccounts());
        }

        private Task DisconnectMtAccounts()
        {
            var tasks = _duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.State == BaseAccountEntity.States.Disconnected) return;
                    account.Connector.Disconnect();
                    account.State = BaseAccountEntity.States.Disconnected;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
                }));

            return Task.WhenAll(tasks);
        }

        private Task DisconnectCtAccounts()
        {
            var tasks = _duplicatContext.CTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.State == BaseAccountEntity.States.Disconnected) return;
                    account.Connector.Disconnect();
                    account.State = BaseAccountEntity.States.Disconnected;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
                }));

            return Task.WhenAll(tasks);
        }

        private Task DisconnectFtAccounts()
        {
            var tasks = _duplicatContext.FixTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.State == BaseAccountEntity.States.Disconnected) return;
                    account.Connector.Disconnect();
                    account.State = BaseAccountEntity.States.Disconnected;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
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

        public Task BalanceReport(DateTime from, DateTime to, string reportPath)
        {
            return Task.Factory.StartNew(() =>
            {
                _balanceReportService.Report(
                    _duplicatContext.Monitors.Local.FirstOrDefault(m => m.Id == SelectedAlphaMonitorId),
                    _duplicatContext.Monitors.Local.FirstOrDefault(m => m.Id == SelectedBetaMonitorId),
                    from, to, reportPath);
            });
        }

        public Task StartMonitors(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId)
        {
            return _monitorServices.Start(duplicatContext, alphaMonitorId, betaMonitorId);
        }

        public void StopMonitors()
        {
            _monitorServices.Stop();
        }

        public Task StartExperts(DuplicatContext duplicatContext)
        {
            return Connect(duplicatContext).ContinueWith(prevTask =>
            {
                _frpService.Start(duplicatContext);
            });
        }

        public void StopExperts()
        {
            _frpService.Stop();
        }

        public void TestMarketOrder(Pushing pushing)
        {
            var connector = (FixTraderIntegration.Connector)pushing.FutureAccount.Connector;
            connector.SendMarketOrderRequest(pushing.FutureSymbol, Common.Integration.Sides.Buy, pushing.PushingDetail.SmallContractSize);
		}
		public void TestLimitOrder(Pushing pushing)
		{
			var connector = (FixTraderIntegration.Connector)pushing.FutureAccount.Connector;
			connector.SendLimitOrderRequest(pushing.FutureSymbol, Common.Integration.Sides.Buy, pushing.PushingDetail.SmallContractSize);
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

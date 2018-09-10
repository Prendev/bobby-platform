using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		Task SaveTheWeekend(DuplicatContext duplicatContext, DateTime from, DateTime to);


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
            IMtAccountImportService mtAccountImportService)
        {
	        _mtAccountImportService = mtAccountImportService;
	        _reportService = reportService;
	        _connectorFactory = connectorFactory;
	        _strategiesService = strategiesService;
	        _tickerService = tickerService;
            _pushingService = pushingService;
            _copierService = copierService;
            _synchronizationContextFactory = synchronizationContextFactory;
        }

        public Task Connect(DuplicatContext duplicatContext)
        {
            _duplicatContext = duplicatContext;
            _synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();

	        var accounts = _duplicatContext.Accounts
		        .Where(pa => pa.Run).ToList()
		        .Where(pa => pa.State != Account.States.Connected);

	        var tasks = accounts.Select(account => Task.Factory.StartNew(() =>
	        {
		        _connectorFactory.Create(account);
		        account.State = account.Connector?.IsConnected == true ? Account.States.Connected : Account.States.Error;
		        account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
	        }));

			return Task.WhenAll(tasks);
        }

        public Task Disconnect()
        {
            _duplicatContext.SaveChanges();
			var accounts = _duplicatContext.Accounts.ToList()
				 .Where(pa => pa.State != Account.States.Disconnected);
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

	    public void MtAccountImport(DuplicatContext duplicatContext)
	    {
		    _mtAccountImportService.Import(duplicatContext);
		}

	    public Task SaveTheWeekend(DuplicatContext duplicatContext, DateTime from, DateTime to)
		{
		    return Connect(duplicatContext).ContinueWith(prevTask =>
		    {
			    _mtAccountImportService.SaveTheWeekend(duplicatContext, from, to);
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

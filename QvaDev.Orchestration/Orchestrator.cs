using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Orchestration.Services;

namespace QvaDev.Orchestration
{
    public interface IOrchestrator
    {
        int SelectedAlphaMonitorId { get; set; }
        int SelectedBetaMonitorId { get; set; }

        Task StartCopiers(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId);
        void StopCopiers();
        Task StartMonitors(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId);
        void StopMonitors();
        Task Connect(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId);
        Task Disconnect();
        Task BalanceReport(DateTime from);
    }

    public class Orchestrator : IOrchestrator
    {
        private SynchronizationContext _synchronizationContext;
        private readonly Func<SynchronizationContext> _synchronizationContextFactory;
        private readonly ILog _log;
        private readonly CTraderIntegration.IConnectorFactory _connectorFactory;
        private readonly IBalanceReportService _balanceReportService;

        private DuplicatContext _duplicatContext;
        private bool _areCopiersStarted;
        private bool _areMonitorsStarted;

        public int SelectedAlphaMonitorId { get; set; }
        public int SelectedBetaMonitorId { get; set; }

        public Orchestrator(
            Func<SynchronizationContext> synchronizationContextFactory,
            CTraderIntegration.IConnectorFactory connectorFactory,
            IBalanceReportService balanceReportService,
            ILog log)
        {
            _balanceReportService = balanceReportService;
            _synchronizationContextFactory = synchronizationContextFactory;
            _connectorFactory = connectorFactory;
            _log = log;
        }

        public Task Connect(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId)
        {
            SelectedBetaMonitorId = betaMonitorId;
            SelectedAlphaMonitorId = alphaMonitorId;
            _duplicatContext = duplicatContext;
            _synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
            return Task.WhenAll(ConnectMtAccounts(), ConenctCtAccounts());
        }

        private Task ConnectMtAccounts()
        {
            var tasks = _duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
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

        public Task Disconnect()
        {
            _areCopiersStarted = false;
            _areMonitorsStarted = false;
            return Task.WhenAll(DisconnectMtAccounts(), DisconnectCtAccounts());
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

        public async Task StartCopiers(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId)
        {
            await Connect(duplicatContext, alphaMonitorId, betaMonitorId);
            foreach (var master in _duplicatContext.Copiers.Local
                .Where(c => c.Slave.CTraderAccount.State == BaseAccountEntity.States.Connected &&
                            c.Slave.Master.MetaTraderAccount.State == BaseAccountEntity.States.Connected)
                .Select(c => c.Slave.Master).Distinct())
            {
                master.MetaTraderAccount.Connector.OnPosition -= MasterOnOrderUpdate;
                master.MetaTraderAccount.Connector.OnPosition += MasterOnOrderUpdate;
            }

            _areCopiersStarted = true;
            _log.Info($"Copiers are started");
        }
        public void StopCopiers()
        {
            _areCopiersStarted = false;
        }

        public Task StartMonitors(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId)
        {
            _areMonitorsStarted = true;
            return Connect(duplicatContext, alphaMonitorId, betaMonitorId).ContinueWith(prevTask =>
            {
                var mtTasks = _duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                    Task.Factory.StartNew(() => MonitorAccount(account)));
                var ctTasks = _duplicatContext.CTraderAccounts.AsEnumerable().Select(account =>
                    Task.Factory.StartNew(() => MonitorAccount(account)));
                return Task.WhenAll(Task.WhenAll(mtTasks), Task.WhenAll(ctTasks));
            });
        }
        public void StopMonitors()
        {
            _areMonitorsStarted = false;
        }


        public Task BalanceReport(DateTime from)
        {
            return Task.Factory.StartNew(() =>
            {
                _balanceReportService.Report(
                    _duplicatContext.Monitors.Local.FirstOrDefault(m => m.Id == SelectedAlphaMonitorId),
                    _duplicatContext.Monitors.Local.FirstOrDefault(m => m.Id == SelectedBetaMonitorId),
                    from);
            });
        }

        private void MasterOnOrderUpdate(object sender, PositionEventArgs e)
        {
            if (!_areCopiersStarted) return;
            Task.Factory.StartNew(() =>
            {
                lock (sender)
                {
                    _log.Info($"Master {e.Action:F} {e.Position.Side:F} signal on " +
                              $"{e.Position.Symbol} with open time: {e.Position.OpenTime:o}");

                    var masters = _duplicatContext.Masters.Local
                        .Where(m => m.MetaTraderAccountId == e.DbId);
                    var type = e.Position.Side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;

                    foreach (var master in masters)
                    foreach (var slave in master.Slaves)
                    {
                        var slaveConnector = (CTraderIntegration.Connector) slave.CTraderAccount.Connector;
                        var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
                            ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
                            : e.Position.Symbol + (slave.SymbolSuffix ?? "");
                        foreach (var copier in slave.Copiers)
                        {
                            var volume = (long) (100 * Math.Abs(e.Position.RealVolume) * copier.CopyRatio);
                            if (e.Action == PositionEventArgs.Actions.Open && copier.UseMarketRangeOrder)
                                slaveConnector.SendMarketRangeOrderRequest(symbol, type, volume, e.Position.OperPrice,
                                    copier.SlippageInPips, $"{slave.Id}-{e.Position.Id}", copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                            else if (e.Action == PositionEventArgs.Actions.Open && !copier.UseMarketRangeOrder)
                                slaveConnector.SendMarketOrderRequest(symbol, type, volume, $"{slave.Id}-{e.Position.Id}",
                                    copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                            else if (e.Action == PositionEventArgs.Actions.Close)
                                slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Position.Id}",
                                    copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                        }
                    }
                }
            });
        }

        private void MonitorAccount(BaseAccountEntity account)
        {
            UpdateActualContracts(account);

            account.Connector.OnPosition -= Monitor_OnPosition;
            account.Connector.OnPosition += Monitor_OnPosition;
        }

        private void Monitor_OnPosition(object sender, PositionEventArgs e)
        {
            if (_areMonitorsStarted) return;
            BaseAccountEntity account = null;
            if (e.AccountType == AccountTypes.Ct)
                account = _duplicatContext.CTraderAccounts.Local.FirstOrDefault(a => a.Id == e.DbId);
            else if(e.AccountType == AccountTypes.Mt4)
                account = _duplicatContext.MetaTraderAccounts.Local.FirstOrDefault(a => a.Id == e.DbId);

            UpdateActualContracts(account);
        }

        private void UpdateActualContracts(BaseAccountEntity account)
        {
            if (account?.Connector == null) return;
            if (account.State != BaseAccountEntity.States.Connected) return;

            var monitored = account.MonitoredAccounts
                .FirstOrDefault(a => a.MonitorId == SelectedAlphaMonitorId || a.MonitorId == SelectedBetaMonitorId);
            if (monitored == null) return;

            var symbol = monitored.Symbol ?? monitored.Monitor.Symbol;
            monitored.ActualContracts = account.Connector.GetOpenContracts(symbol);
            monitored.RaisePropertyChanged(_synchronizationContext, nameof(monitored.ActualContracts));

            monitored.Monitor.ActualContracts = monitored.Monitor.MonitoredAccounts
                .Where(a => !a.IsMaster)
                .Sum(a => a.ActualContracts);
            monitored.Monitor.RaisePropertyChanged(_synchronizationContext, nameof(monitored.Monitor.ActualContracts));

            monitored.Monitor.ExpectedContracts = monitored.Monitor.MonitoredAccounts
                .Where(a => !a.IsMaster)
                .Sum(a => a.ExpectedContracts);
            monitored.Monitor.RaisePropertyChanged(_synchronizationContext, nameof(monitored.Monitor.ExpectedContracts));
        }
    }
}

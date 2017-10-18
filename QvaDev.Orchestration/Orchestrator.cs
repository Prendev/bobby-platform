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
        Task StartCopiers(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId);
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
        private bool _areCopiersActive;
        private int _alphaMonitorId;
        private int _betaMonitorId;

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
            _betaMonitorId = betaMonitorId;
            _alphaMonitorId = alphaMonitorId;
            _duplicatContext = duplicatContext;
            _synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
            _areCopiersActive = false;
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
                        Id = account.Id,
                        Description = account.Description,
                        User = (uint) account.User,
                        Password = account.Password,
                        Srv = account.MetaTraderPlatform.SrvFilePath
                    });
                    account.State = connected
                        ? BaseAccountEntity.States.Connected
                        : BaseAccountEntity.States.Error;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));

                    MonitorAccount(account);
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
                                AccountId = account.Id,
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

                    MonitorAccount(account);
                }));

            return Task.WhenAll(tasks);
        }

        public Task Disconnect()
        {
            _areCopiersActive = false;
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
            await Connect(_duplicatContext, alphaMonitorId, betaMonitorId);
            foreach (var master in _duplicatContext.Copiers.Local
                .Where(c => c.Slave.CTraderAccount.State == BaseAccountEntity.States.Connected &&
                            c.Slave.Master.MetaTraderAccount.State == BaseAccountEntity.States.Connected)
                .Select(c => c.Slave.Master).Distinct())
            {
                master.MetaTraderAccount.Connector.OnPosition -= MasterOnOrderUpdate;
                master.MetaTraderAccount.Connector.OnPosition += MasterOnOrderUpdate;
            }

            _areCopiersActive = true;
            _log.Info($"Copiers are started");
        }


        public Task BalanceReport(DateTime from)
        {
            return Task.Factory.StartNew(() =>
            {
                _balanceReportService.Report(
                    _duplicatContext.Monitors.Local.FirstOrDefault(m => m.Id == _alphaMonitorId),
                    _duplicatContext.Monitors.Local.FirstOrDefault(m => m.Id == _betaMonitorId),
                    from);
            });
        }

        private void MasterOnOrderUpdate(object sender, PositionEventArgs e)
        {
            if (!_areCopiersActive) return;
            Task.Factory.StartNew(() =>
            {
                lock (sender)
                {
                    _log.Info($"Master {e.Action:F} {e.Position.Side:F} signal on " +
                              $"{e.Position.Symbol} with open time: {e.Position.OpenTime:o}");

                    var masters = _duplicatContext.Masters.Local
                        .Where(m => m.MetaTraderAccountId == e.Position.AccountId);
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
            BaseAccountEntity account = null;
            if (e.AccountType == AccountTypes.Ct)
                account = _duplicatContext.CTraderAccounts.Local.FirstOrDefault(a => a.Id == e.Position.AccountId);
            else if(e.AccountType == AccountTypes.Mt4)
                account = _duplicatContext.MetaTraderAccounts.Local.FirstOrDefault(a => a.Id == e.Position.AccountId);

            UpdateActualContracts(account);
        }

        private void UpdateActualContracts(BaseAccountEntity account)
        {
            if (account?.Connector == null) return;
            if (account.State != BaseAccountEntity.States.Connected) return;

            var monitored = account.MonitoredAccounts
                .FirstOrDefault(a => a.MonitorId == _alphaMonitorId || a.MonitorId == _betaMonitorId);
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

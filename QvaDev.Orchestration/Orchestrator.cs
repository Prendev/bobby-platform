using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration;
using QvaDev.Data;
using QvaDev.Data.Models;
using CtConnector = QvaDev.CTraderIntegration.ConnectorRetryDecorator;
using MtConnector = QvaDev.Mt4Integration.Connector;

namespace QvaDev.Orchestration
{
    public interface IOrchestrator
    {
        Task StartCopiers(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId);
        Task Connect(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId);
        Task Disconnect();
    }

    public class Orchestrator : IOrchestrator
    {
        private SynchronizationContext _synchronizationContext;
        private readonly Func<SynchronizationContext> _synchronizationContextFactory;
        private readonly ILog _log;
        private readonly IConnectorFactory _connectorFactory;

        private DuplicatContext _duplicatContext;
        private bool _areCopiersActive;

        public Orchestrator(
            Func<SynchronizationContext> synchronizationContextFactory,
            IConnectorFactory connectorFactory,
            ILog log)
        {
            _synchronizationContextFactory = synchronizationContextFactory;
            _connectorFactory = connectorFactory;
            _log = log;
        }

        public Task Connect(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId)
        {
            _duplicatContext = duplicatContext;
            _synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
            _areCopiersActive = false;
            return Task.WhenAll(ConnectMtAccounts(alphaMonitorId, betaMonitorId), ConenctCtAccounts(alphaMonitorId, betaMonitorId));
        }

        private Task ConnectMtAccounts(int alphaMonitorId, int betaMonitorId)
        {
            var tasks = _duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.State == BaseAccountEntity.States.Connected) return;
                    var connector = account.Connector as MtConnector;
                    if (connector == null)
                    {
                        connector = new MtConnector(_log);
                        account.Connector = connector;
                    }
                    var connected = connector.Connect(new Mt4Integration.AccountInfo()
                    {
                        Description = account.Description,
                        User = (uint) account.User,
                        Password = account.Password,
                        Srv = account.MetaTraderPlatform.SrvFilePath
                    });
                    account.State = connected
                        ? BaseAccountEntity.States.Connected
                        : BaseAccountEntity.States.Error;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));

                    MonitorAccount(account, alphaMonitorId, betaMonitorId);
                }));

            return Task.WhenAll(tasks);
        }

        private Task ConenctCtAccounts(int alphaMonitorId, int betaMonitorId)
        {
            var tasks = _duplicatContext.CTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.State == BaseAccountEntity.States.Connected) return;
                    var connector = account.Connector as CtConnector;
                    if (connector == null)
                    {
                        connector = (CtConnector) _connectorFactory.Create(
                            new PlatformInfo
                            {
                                Description = account.CTraderPlatform.Description,
                                AccountsApi = account.CTraderPlatform.AccountsApi,
                                ClientId = account.CTraderPlatform.ClientId,
                                TradingHost = account.CTraderPlatform.TradingHost,
                                Secret = account.CTraderPlatform.Secret,
                                Playground = account.CTraderPlatform.Playground
                            },
                            new AccountInfo
                            {
                                Description = account.Description,
                                AccountNumber = account.AccountNumber,
                                AccessToken = account.AccessToken
                            });
                        account.Connector = connector;
                    }
                    var connected = connector.Connect(new AccountInfo()
                    {
                        Description = account.Description,
                        AccountNumber = account.AccountNumber
                    });
                    account.State = connected
                        ? BaseAccountEntity.States.Connected
                        : BaseAccountEntity.States.Error;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));

                    MonitorAccount(account, alphaMonitorId, betaMonitorId);
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
                master.MetaTraderAccount.Connector.OnOrder -= MasterOnOrderUpdate;
                master.MetaTraderAccount.Connector.OnOrder += MasterOnOrderUpdate;
            }

            _areCopiersActive = true;
            _log.Info($"Copiers are started");
        }

        private void MasterOnOrderUpdate(object sender, OrderEventArgs e)
        {
            if (!_areCopiersActive) return;
            Task.Factory.StartNew(() =>
            {
                lock (sender)
                {
                    _log.Info($"Master {e.Action:F} {e.Side:F} signal on {e.Symbol} with open time: {e.OpenTime:o}");

                    var masters = _duplicatContext.Masters.Local
                        .Where(m => m.MetaTraderAccount.Description == e.AccountDescription);
                    var type = e.Side == OrderEventArgs.Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;

                    foreach (var master in masters)
                    foreach (var slave in master.Slaves)
                    {
                        var slaveConnector = (CtConnector) slave.CTraderAccount.Connector;
                        var symbol = slave.SymbolMappings?.Any(m => m.From == e.Symbol) == true
                            ? slave.SymbolMappings.First(m => m.From == e.Symbol).To
                            : e.Symbol + (slave.SymbolSuffix ?? "");
                        foreach (var copier in slave.Copiers)
                        {
                            var volume = (long) (100 * e.Volume * copier.CopyRatio);
                            if (e.Action == OrderEventArgs.Actions.Open && copier.UseMarketRangeOrder)
                                slaveConnector.SendMarketRangeOrderRequest(symbol, type, volume, e.OperPrice,
                                    copier.SlippageInPips, $"{slave.Id}-{e.Ticket}", copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                            else if (e.Action == OrderEventArgs.Actions.Open && !copier.UseMarketRangeOrder)
                                slaveConnector.SendMarketOrderRequest(symbol, type, volume, $"{slave.Id}-{e.Ticket}",
                                    copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                            else if (e.Action == OrderEventArgs.Actions.Close)
                                slaveConnector.SendClosePositionRequests($"{slave.Id}-{e.Ticket}",
                                    copier.MaxRetryCount, copier.RetryPeriodInMilliseconds);
                        }
                    }
                }
            });
        }

        private void MonitorAccount(MetaTraderAccount account, int alphaMonitorId, int betaMonitorId)
        {
            var monitored = account.MonitoredAccounts
                .FirstOrDefault(a => a.MonitorId == alphaMonitorId || a.MonitorId == betaMonitorId);
            if (monitored == null) return;

            var connector = account.Connector as MtConnector;
            if (connector == null) return;

            var symbol = monitored.Symbol ?? monitored.Monitor.Symbol;

            var symbolInfo = connector.QuoteClient.GetSymbolInfo(symbol);

            monitored.ActualContracts = (long)connector.QuoteClient.GetOpenedOrders()
                .Where(o => o.Symbol == symbol)
                .Sum(o => o.Lots * symbolInfo.ContractSize);
            monitored.RaisePropertyChanged(_synchronizationContext, nameof(monitored.ActualContracts));
        }

        private void MonitorAccount(CTraderAccount account, int alphaMonitorId, int betaMonitorId)
        {
            if (account.State != BaseAccountEntity.States.Connected) return;

            var monitored = account.MonitoredAccounts
                .FirstOrDefault(a => a.MonitorId == alphaMonitorId || a.MonitorId == betaMonitorId);
            if (monitored == null) return;

            var connector = account.Connector as CtConnector;
            if (connector == null) return;

            var symbol = monitored.Symbol ?? monitored.Monitor.Symbol;

            monitored.ActualContracts = connector.Positions
                .Where(p => p.Value.Symbol == symbol)
                .Sum(p => p.Value.Volume / 100);
            monitored.RaisePropertyChanged(_synchronizationContext, nameof(monitored.ActualContracts));
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration;
using QvaDev.Data;
using CtConnector = QvaDev.CTraderIntegration.ConnectorRetryDecorator;
using MtConnector = QvaDev.Mt4Integration.Connector;

namespace QvaDev.Orchestration
{
    public interface IOrchestrator
    {
        Task Connect(DuplicatContext duplicatContext);
        Task Disconnect(DuplicatContext duplicatContext);
        Task StartCopiers(DuplicatContext duplicatContext);
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

        public Task Connect(DuplicatContext duplicatContext)
        {
            _synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
            _areCopiersActive = false;
            return Task.WhenAll(ConnectMtAccounts(duplicatContext), ConenctCtAccounts(duplicatContext));
        }

        private Task ConnectMtAccounts(DuplicatContext duplicatContext)
        {
            var tasks = duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.IsConnected) return;
                    var connector = account.Connector as MtConnector;
                    if (connector == null)
                    {
                        connector = new MtConnector(_log);
                        account.Connector = connector;
                    }
                    account.IsConnected = connector.Connect(new Mt4Integration.AccountInfo()
                    {
                        Description = account.Description,
                        User = (uint) account.User,
                        Password = account.Password,
                        Srv = account.MetaTraderPlatform.SrvFilePath
                    });
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.IsConnected));
                }));

            return Task.WhenAll(tasks);
        }

        private Task ConenctCtAccounts(DuplicatContext duplicatContext)
        {
            var tasks = duplicatContext.CTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.IsConnected) return;
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
                    account.IsConnected = connector.Connect(new AccountInfo()
                    {
                        Description = account.Description,
                        AccountNumber = account.AccountNumber
                    });
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.IsConnected));
                }));

            return Task.WhenAll(tasks);
        }

        public Task Disconnect(DuplicatContext duplicatContext)
        {
            _areCopiersActive = false;
            return Task.WhenAll(DisconnectMtAccounts(duplicatContext), DisconnectCtAccounts(duplicatContext));
        }

        private Task DisconnectMtAccounts(DuplicatContext duplicatContext)
        {
            var tasks = duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (!account.IsConnected) return;
                    account.Connector.Disconnect();
                    account.IsConnected = false;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.IsConnected));
                }));

            return Task.WhenAll(tasks);
        }

        private Task DisconnectCtAccounts(DuplicatContext duplicatContext)
        {
            var tasks = duplicatContext.CTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (!account.IsConnected) return;
                    account.Connector.Disconnect();
                    account.IsConnected = false;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.IsConnected));
                }));

            return Task.WhenAll(tasks);
        }

        public async Task StartCopiers(DuplicatContext duplicatContext)
        {
            _duplicatContext = duplicatContext;
            await Connect(_duplicatContext);
            foreach (var master in duplicatContext.Copiers.Local
                .Where(c => c.Slave.CTraderAccount.IsConnected && c.Slave.Master.MetaTraderAccount.IsConnected)
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
    }
}

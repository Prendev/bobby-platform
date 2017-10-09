using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration;
using QvaDev.CTraderIntegration.Services;
using QvaDev.Data;
using CtConnector = QvaDev.CTraderIntegration.Connector;
using MtConnector = QvaDev.Mt4Integration.Connector;

namespace QvaDev.Orchestration
{
    public interface IOrchestrator
    {
        void Connect(DuplicatContext duplicatContext);
        void Disconnect(DuplicatContext duplicatContext);
        void StartCopiers(DuplicatContext duplicatContext);
    }

    public class Orchestrator : IOrchestrator
    {
        private class CtPosition
        {
            public long Volume { get; set; }
            public string ClientOrderId { get; set; }
        }

        private readonly ILog _log;
        private readonly IConnectorFactory _connectorFactory;

        private DuplicatContext _duplicatContext;
        private bool _areCopiersActive;
        private readonly ConcurrentDictionary<long, ConcurrentDictionary<long, CtPosition>> _ctPositions =
            new ConcurrentDictionary<long, ConcurrentDictionary<long, CtPosition>>();


        public Orchestrator(
            IConnectorFactory connectorFactory,
            ILog log)
        {
            _connectorFactory = connectorFactory;
            _log = log;
        }

        public void Connect(DuplicatContext duplicatContext)
        {
            _areCopiersActive = false;
            ConnectMtAccounts(duplicatContext);
            ConenctCtAccounts(duplicatContext);
        }

        private void ConnectMtAccounts(DuplicatContext duplicatContext)
        {
            foreach (var account in duplicatContext.MetaTraderAccounts)
            {
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
                        User = (uint)account.User,
                        Password = account.Password,
                        Srv = account.MetaTraderPlatform.SrvFilePath
                    });
                }, TaskCreationOptions.LongRunning);
            }
        }

        private void ConenctCtAccounts(DuplicatContext duplicatContext)
        {
            foreach (var account in duplicatContext.CTraderAccounts)
            {
                Task.Factory.StartNew(() =>
                {
                    if (account.IsConnected) return;
                    var connector = account.Connector as CtConnector;
                    if (connector == null)
                    {
                        connector = _connectorFactory.Create(new Common.Configuration.CTraderPlatform()
                        {
                            Description = account.CTraderPlatform.Description,
                            AccessToken = account.CTraderPlatform.AccessToken,
                            AccountsApi = account.CTraderPlatform.AccountsApi,
                            ClientId = account.CTraderPlatform.ClientId,
                            TradingHost = account.CTraderPlatform.TradingHost,
                            Secret = account.CTraderPlatform.Secret,
                            Playground = account.CTraderPlatform.Playground
                        });
                        account.Connector = connector;
                    }
                    account.IsConnected = connector.Connect(new AccountInfo()
                    {
                        Description = account.Description,
                        AccountNumber = account.AccountNumber
                    });

                    if (!account.IsConnected) return;

                    _ctPositions.GetOrAdd(account.AccountNumber, new ConcurrentDictionary<long, CtPosition>());
                    foreach (var p in connector.GetPositions())
                    {
                        _ctPositions[account.AccountNumber].GetOrAdd(p.positionId,
                            new CtPosition {Volume = p.volume, ClientOrderId = p.GetCliendOrderId()});
                    }
                }, TaskCreationOptions.LongRunning);
            }
        }

        public void Disconnect(DuplicatContext duplicatContext)
        {
            _areCopiersActive = false;
            DisconnectMtAccounts(duplicatContext);
            DisconnectCtAccounts(duplicatContext);
        }

        private void DisconnectMtAccounts(DuplicatContext duplicatContext)
        {
            foreach (var account in duplicatContext.MetaTraderAccounts)
            {
                if (!account.IsConnected) continue;
                Task.Factory.StartNew(() =>
                {
                    account.Connector.Disconnect();
                    account.IsConnected = false;
                }, TaskCreationOptions.LongRunning);
            }
        }

        private void DisconnectCtAccounts(DuplicatContext duplicatContext)
        {
            foreach (var account in duplicatContext.CTraderAccounts)
            {
                if (!account.IsConnected) continue;
                Task.Factory.StartNew(() =>
                {
                    account.Connector.Disconnect();
                    account.IsConnected = false;
                }, TaskCreationOptions.LongRunning);
            }
        }

        public void StartCopiers(DuplicatContext duplicatContext)
        {
            _duplicatContext = duplicatContext;
            Connect(_duplicatContext);
            foreach (var master in duplicatContext.Copiers.Local
                .Where(c => c.Slave.CTraderAccount.IsConnected && c.Slave.Master.MetaTraderAccount.IsConnected)
                .Select(c => c.Slave.Master).Distinct())
            {
                master.MetaTraderAccount.Connector.OnOrder -= MasterOnOrderUpdate;
                master.MetaTraderAccount.Connector.OnOrder += MasterOnOrderUpdate;
            }

            _areCopiersActive = true; ;
        }

        private void MasterOnOrderUpdate(object sender, OrderEventArgs e)
        {
            if (!_areCopiersActive) return;
            Task.Factory.StartNew(() =>
            {
                lock (sender)
                {
                    _log.Info($"Master {e.Side:F} signal on {e.Symbol} with open time: {e.OpenTime:o}");

                    //var account = _config.MasterAccounts.First(a => a.Description == masterConnector.AccountInfo.Description);
                    var masters = _duplicatContext.Masters.Where(m => m.MetaTraderAccount.Description == e.AccountDescription);
                    var type = e.Side == OrderEventArgs.Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;

                    foreach (var master in masters)
                    foreach (var slave in master.Slaves)
                    {
                        var slaveConnector = (CtConnector) slave.CTraderAccount.Connector;
                        var symbol = slave.SymbolMappings?.Any(m => m.From == e.Symbol) == true
                            ? slave.SymbolMappings.First(m => m.From == e.Symbol).To
                            : e.Symbol + slave.SymbolSuffix;
                        foreach (var copier in slave.Copiers)
                        {
                            if (e.Action == OrderEventArgs.Actions.Open)
                            {
                                var volume = 100 * e.Volume * copier.CopyRatio;
                                slaveConnector.SendMarketRangeOrderRequest(symbol, type, (long) volume, e.OperPrice, e.Ticket);
                            }
                            else if (e.Action == OrderEventArgs.Actions.Close)
                            {
                                //foreach (var pos in _cTraderPositions[slave]
                                //    .Where(p => p.Value.ClientOrderId == e.Order.Ticket.ToString()))
                                //{
                                //    slaveConnector.SendClosePositionRequest(pos.Key, Math.Abs(pos.Value.Volume));
                                //}
                            }
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}

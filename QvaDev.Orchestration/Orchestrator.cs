using System.Threading.Tasks;
using log4net;
using QvaDev.CTraderIntegration;
using QvaDev.Data;
using CtConnector = QvaDev.CTraderIntegration.Connector;
using MtConnector = QvaDev.Mt4Integration.Connector;

namespace QvaDev.Orchestration
{
    public interface IOrchestrator
    {
        void Connect(DuplicatContext duplicatContext);
        void Disconnect(DuplicatContext duplicatContext);
    }

    public class Orchestrator : IOrchestrator
    {
        private readonly ILog _log;
        private readonly IConnectorFactory _connectorFactory;

        public Orchestrator(
            IConnectorFactory connectorFactory,
            ILog log)
        {
            _connectorFactory = connectorFactory;
            _log = log;
        }

        public void Connect(DuplicatContext duplicatContext)
        {
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
                }, TaskCreationOptions.LongRunning);
            }
        }

        public void Disconnect(DuplicatContext duplicatContext)
        {
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
    }
}

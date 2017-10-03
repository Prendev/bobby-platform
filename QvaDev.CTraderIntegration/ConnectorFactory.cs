using System.Collections.Concurrent;
using log4net;
using QvaDev.Common.Configuration;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public class ConnectorFactory
    {
        /// <summary>
        /// The key is Platform's description
        /// </summary>
        private readonly ConcurrentDictionary<string, CTraderClientWrapper> _cTraderClientWrappers = new ConcurrentDictionary<string, CTraderClientWrapper>();

        private readonly ILog _log;
        private readonly ITradingAccountsService _tradingAccountsService;
        private readonly ConnectorConfig _connectorConfig;

        public ConnectorFactory(
            ConnectorConfig connectorConfig,
            ILog log,
            ITradingAccountsService tradingAccountsService)
        {
            _connectorConfig = connectorConfig;
            _tradingAccountsService = tradingAccountsService;
            _log = log;
        }

        public Connector Create(CTraderPlatform platform, CTraderAccount account)
        {
            var connector = new Connector(_connectorConfig, _log);

            var cTraderClientWrapper = _cTraderClientWrappers.GetOrAdd(platform.Description,
                new CTraderClientWrapper(platform, _tradingAccountsService));

            connector.Connect(cTraderClientWrapper,
                new AccountInfo() {Description = account.Description, AccountNumber = account.AccountNumber});

            return connector;
        }
    }
}

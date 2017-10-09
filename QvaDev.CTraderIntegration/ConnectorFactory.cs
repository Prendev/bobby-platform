using System.Collections.Concurrent;
using log4net;
using QvaDev.Common.Configuration;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public interface IConnectorFactory
    {
        Connector Create(CTraderPlatform platform);
    }

    public class ConnectorFactory : IConnectorFactory
    {
        /// <summary>
        /// The key is Platform's description
        /// </summary>
        private readonly ConcurrentDictionary<string, CTraderClientWrapper> _cTraderClientWrappers = new ConcurrentDictionary<string, CTraderClientWrapper>();

        private readonly ILog _log;
        private readonly ITradingAccountsService _tradingAccountsService;

        public ConnectorFactory(
            ILog log,
            ITradingAccountsService tradingAccountsService)
        {
            _tradingAccountsService = tradingAccountsService;
            _log = log;
        }

        public Connector Create(CTraderPlatform platform)
        {
            lock (platform.Description)
            {
                var cTraderClientWrapper = _cTraderClientWrappers.GetOrAdd(platform.Description,
                    s => new CTraderClientWrapper(platform, _tradingAccountsService));

                var connector = new Connector(cTraderClientWrapper, _tradingAccountsService, _log);

                return connector;
            }
        }
    }
}

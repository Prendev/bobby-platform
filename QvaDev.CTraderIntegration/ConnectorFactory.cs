using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using log4net;
using QvaDev.Common.Configuration;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Dto;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public interface IConnectorFactory
    {
        IConnector Create(CTraderPlatform platform, CTraderAccount account);
    }

    public class ConnectorFactory : IConnectorFactory
    {
        /// <summary>
        /// The key is Platform's description
        /// </summary>
        private readonly ConcurrentDictionary<string, CTraderClientWrapper> _cTraderClientWrappers =
            new ConcurrentDictionary<string, CTraderClientWrapper>();

        /// <summary>
        /// The ley is access token
        /// </summary>
        public ConcurrentDictionary<string, List<AccountData>> _accounts =
            new ConcurrentDictionary<string, List<AccountData>>();

        private readonly ILog _log;
        private readonly ITradingAccountsService _tradingAccountsService;

        public ConnectorFactory(
            ILog log,
            ITradingAccountsService tradingAccountsService)
        {
            _tradingAccountsService = tradingAccountsService;
            _log = log;
        }

        public IConnector Create(CTraderPlatform platform, CTraderAccount account)
        {
            var accountInfo = new AccountInfo()
            {
                Description = account.Description,
                AccountNumber = account.AccountNumber,
                AccessToken = account.AccessToken
            };

            lock (account.AccessToken)
            {
                var accountData = _accounts.GetOrAdd(account.AccessToken,
                    s => _tradingAccountsService
                        .GetAccounts(new BaseRequest()
                        {
                            AccessToken = account.AccessToken,
                            BaseUrl = platform.AccountsApi
                        })).FirstOrDefault(a => a.accountNumber == account.AccountNumber);

                if (accountData != null)
                    accountInfo.AccountId = accountData.accountId;
            }

            CTraderClientWrapper cTraderClientWrapper;
            lock (platform.Description)
            {
                cTraderClientWrapper = _cTraderClientWrappers.GetOrAdd(platform.Description,
                    s => new CTraderClientWrapper(platform, _log));
            }

            var connector = new ConnectorRetryDecorator(accountInfo, cTraderClientWrapper, _tradingAccountsService, _log);

            return connector;
        }
    }
}

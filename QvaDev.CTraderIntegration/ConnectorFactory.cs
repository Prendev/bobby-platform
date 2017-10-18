using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Dto;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public interface IConnectorFactory
    {
        IConnector Create(PlatformInfo platformInfo, AccountInfo accountInfo);
    }

    public class ConnectorFactory : IConnectorFactory
    {
        /// <summary>
        /// The key is Platform's description
        /// </summary>
        private readonly ConcurrentDictionary<string, Lazy<CTraderClientWrapper>> _cTraderClientWrappers =
            new ConcurrentDictionary<string, Lazy<CTraderClientWrapper>>();

        /// <summary>
        /// The key is access token
        /// </summary>
        private readonly ConcurrentDictionary<string, Lazy<List<AccountData>>> _accounts =
            new ConcurrentDictionary<string, Lazy<List<AccountData>>>();

        private readonly ILog _log;
        private readonly ITradingAccountsService _tradingAccountsService;

        public ConnectorFactory(
            ILog log,
            ITradingAccountsService tradingAccountsService)
        {
            _tradingAccountsService = tradingAccountsService;
            _log = log;
        }

        public IConnector Create(PlatformInfo platformInfo, AccountInfo accountInfo)
        {
            var accounts = _accounts.GetOrAdd(accountInfo.AccessToken,
                accessToken => new Lazy<List<AccountData>>(() =>
                {
                    var accs = _tradingAccountsService
                        .GetAccounts(new BaseRequest
                        {
                            AccessToken = accountInfo.AccessToken,
                            BaseUrl = platformInfo.AccountsApi
                        });

                    _log.Debug($"Accounts acquired for access token: {accessToken}");
                    return accs;
                }, true));

            accountInfo.AccountId = accounts.Value
                .FirstOrDefault(a => a.accountNumber == accountInfo.AccountNumber)?.accountId ?? 0;

            var cTraderClientWrapper = _cTraderClientWrappers.GetOrAdd(platformInfo.Description,
                key => new Lazy<CTraderClientWrapper>(() => new CTraderClientWrapper(platformInfo, _log), true));

            var connector = new Connector(accountInfo, cTraderClientWrapper.Value,
                _tradingAccountsService, _log);

            return connector;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using QvaDev.Common.Integration;
using QvaDev.Common.Logging;
using QvaDev.CTraderIntegration.Dto;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public interface ICtConnectorFactory
    {
        IConnector Create(PlatformInfo platformInfo, AccountInfo accountInfo);
    }

    public class CtConnectorFactory : ICtConnectorFactory
    {
        /// <summary>
        /// The key is Platform's description
        /// </summary>
        private static readonly ConcurrentDictionary<string, Lazy<CTraderClientWrapper>> CTraderClientWrappers =
            new ConcurrentDictionary<string, Lazy<CTraderClientWrapper>>();

        /// <summary>
        /// The key is access token
        /// </summary>
        private static readonly ConcurrentDictionary<string, Lazy<List<AccountData>>> Accounts =
            new ConcurrentDictionary<string, Lazy<List<AccountData>>>();

        private readonly ICustomLog _log;
        private readonly ITradingAccountsService _tradingAccountsService;

        public CtConnectorFactory(
	        ICustomLog log,
            ITradingAccountsService tradingAccountsService)
        {
            _tradingAccountsService = tradingAccountsService;
            _log = log;
        }

        public IConnector Create(PlatformInfo platformInfo, AccountInfo accountInfo)
        {
            var accounts = Accounts.GetOrAdd(accountInfo.AccessToken,
                accessToken => new Lazy<List<AccountData>>(() =>
                {
                    try
                    {
                        var accs = _tradingAccountsService
                            .GetAccounts(new BaseRequest
                            {
                                AccessToken = accountInfo.AccessToken,
                                BaseUrl = platformInfo.AccountsApi
                            });
                        return accs;
                    }
                    catch (Exception e)
                    {
                        _log.Error("Get accounts exception", e);
                    }
                    _log.Debug($"Accounts acquired for access token: {accessToken}");
                    return null;
                }, true));

            accountInfo.AccountId = accounts.Value?
                .FirstOrDefault(a => a.accountNumber == accountInfo.AccountNumber)?.accountId ?? 0;

            var cTraderClientWrapper = CTraderClientWrappers.GetOrAdd(platformInfo.Description,
                key => new Lazy<CTraderClientWrapper>(() => new CTraderClientWrapper(platformInfo, _log), true));

            var connector = new Connector(accountInfo, cTraderClientWrapper.Value,
                _tradingAccountsService, _log);

            return connector;
        }
    }
}

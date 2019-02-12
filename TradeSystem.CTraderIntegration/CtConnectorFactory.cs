using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TradeSystem.Common.Integration;
using TradeSystem.CTraderIntegration.Dto;
using TradeSystem.CTraderIntegration.Services;

namespace TradeSystem.CTraderIntegration
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

        private readonly ITradingAccountsService _tradingAccountsService;

        public CtConnectorFactory(ITradingAccountsService tradingAccountsService)
        {
            _tradingAccountsService = tradingAccountsService;
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
	                    Logger.Error("Get accounts exception", e);
                    }
	                Logger.Debug($"Accounts acquired for access token: {accessToken}");
                    return null;
                }, true));

            accountInfo.AccountId = accounts.Value?
                .FirstOrDefault(a => a.accountNumber == accountInfo.AccountNumber)?.accountId ?? 0;

            var cTraderClientWrapper = CTraderClientWrappers.GetOrAdd(platformInfo.Description,
                key => new Lazy<CTraderClientWrapper>(() => new CTraderClientWrapper(platformInfo), true));

            var connector = new Connector(accountInfo, cTraderClientWrapper.Value, _tradingAccountsService);

            return connector;
        }
    }
}

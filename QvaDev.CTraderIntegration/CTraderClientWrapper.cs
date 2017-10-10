using System.Collections.Generic;
using log4net;
using QvaDev.Common.Configuration;
using QvaDev.CTraderApi;
using QvaDev.CTraderIntegration.Dto;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public class CTraderClientWrapper
    {
        public bool IsConnected { get; }
        public CTraderPlatform Platform { get; }
        public CTraderClient CTraderClient { get; }
        public List<AccountData> Accounts { get; }

        public CTraderClientWrapper(
            CTraderPlatform platform,
            ITradingAccountsService tradingAccountService,
            ILog log)
        {
            CTraderClient = new CTraderClient();
            Platform = platform;
            IsConnected = CTraderClient.Connect(platform.TradingHost, 5032, platform.ClientId, platform.Secret);
            if (!IsConnected)
            {
                log.Error($"{platform.Description} cTrader platform FAILED to connect");
                return;
            }
            log.Debug($"{platform.Description} cTrader platform connected");
            Accounts = tradingAccountService.GetAccounts(
                new BaseRequest() {AccessToken = platform.AccessToken, BaseUrl = platform.AccountsApi});

            log.Debug($"{platform.Description} cTrader platform accounts acquired");
        }
    }
}

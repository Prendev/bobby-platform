using System.Collections.Generic;
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

        public CTraderClientWrapper(CTraderPlatform platform, ITradingAccountsService tradingAccountService)
        {
            CTraderClient = new CTraderClient();
            Platform = platform;
            IsConnected = CTraderClient.Connect(platform.TradingHost, 5032, platform.ClientId, platform.Secret);
            if (!IsConnected) return;
            Accounts = tradingAccountService.GetAccounts(
                new BaseRequest() {AccessToken = platform.AccessToken, BaseUrl = platform.AccountsApi});
        }
    }
}

using log4net;
using QvaDev.CTraderApi;

namespace QvaDev.CTraderIntegration
{
    public class CTraderClientWrapper
    {
        public bool IsConnected { get; }
        public PlatformInfo PlatformInfo { get; }
        public CTraderClient CTraderClient { get; }

        public CTraderClientWrapper(
            PlatformInfo platformInfo,
            ILog log)
        {
            PlatformInfo = platformInfo;
            CTraderClient = new CTraderClient(log);
            IsConnected = CTraderClient.Connect(platformInfo.TradingHost);
            if (!IsConnected)
            {
                log.Error($"{platformInfo.Description} cTrader platform FAILED to connect");
                return;
            }
            CTraderClient.SendAuthorizationRequest(platformInfo.ClientId, platformInfo.Secret);

            log.Debug($"{platformInfo.Description} cTrader platform connected");

            log.Debug($"{platformInfo.Description} cTrader platform accounts acquired");
        }
    }
}

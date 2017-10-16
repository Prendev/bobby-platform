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
            lock (log) CTraderClient.Log = CTraderClient.Log;

            CTraderClient = new CTraderClient();
            PlatformInfo = platformInfo;
            IsConnected = CTraderClient.Connect(platformInfo.TradingHost, 5032, platformInfo.ClientId, platformInfo.Secret);
            if (!IsConnected)
            {
                log.Error($"{platformInfo.Description} cTrader platform FAILED to connect");
                return;
            }
            log.Debug($"{platformInfo.Description} cTrader platform connected");

            log.Debug($"{platformInfo.Description} cTrader platform accounts acquired");
        }
    }
}

using QvaDev.Common.Logging;
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
	        IsConnected = CTraderClient.Connect(new ConnectionDetails()
	        {
		        Description = platformInfo.Description,
		        ClientId = platformInfo.ClientId,
		        TradingHost = platformInfo.TradingHost,
		        Secret = platformInfo.Secret
	        });
        }
    }
}

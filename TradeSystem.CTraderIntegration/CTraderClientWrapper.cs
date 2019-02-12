using TradeSystem.CTraderApi;

namespace TradeSystem.CTraderIntegration
{
    public class CTraderClientWrapper
    {
        public bool IsConnected { get; }
        public PlatformInfo PlatformInfo { get; }
        public CTraderClient CTraderClient { get; }

        public CTraderClientWrapper(
            PlatformInfo platformInfo)
        {
            PlatformInfo = platformInfo;
            CTraderClient = new CTraderClient();
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

using System;
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
			var host = new Uri($"http://{platformInfo.TradingHost}");
	        IsConnected = CTraderClient.Connect(new ConnectionDetails()
	        {
		        Description = platformInfo.Description,
		        ClientId = platformInfo.ClientId,
		        TradingHost = host.Host,
		        Port = host.Port,
		        Secret = platformInfo.Secret
	        });
        }
    }
}

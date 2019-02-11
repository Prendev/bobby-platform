using TradeSystem.Common.Integration;

namespace TradeSystem.IlyaFastFeedIntegration
{
	public class AccountInfo : BaseAccountInfo
	{
		public string IpAddress { get; set; }
		public int Port { get; set; }
		public string UserName { get; set; }
	}
}

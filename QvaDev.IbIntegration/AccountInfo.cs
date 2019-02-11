using TradeSystem.Common.Integration;

namespace TradeSystem.IbIntegration
{
	public class AccountInfo : BaseAccountInfo
	{
		public int Port { get; set; }
		public int ClientId { get; set; }
	}
}

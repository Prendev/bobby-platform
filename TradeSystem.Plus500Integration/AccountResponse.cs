using Newtonsoft.Json;

namespace TradeSystem.Plus500Integration
{
	public class AccountResponse
	{
		[JsonProperty("availableBalance")]
		public double AvailableBalance { get; set; }

		[JsonProperty("equity")]
		public double Equity { get; set; }

		[JsonProperty("maintenanceMargin")]
		public double MaintenanceMargin { get; set; }
		
		[JsonProperty("totalNetProfitLoss")]
		public double PnL { get; set; }
	}
}

using System.Text.Json.Serialization;
using TradeSystem.Common.Integration;

namespace TradeSystem.Plus500Integration
{
	public class AccountInfo : BaseAccountInfo
	{
		[JsonPropertyName("clientId")]
		public string ClientId { get; set; }
		public string SrvPath { get; set; }
	}
}

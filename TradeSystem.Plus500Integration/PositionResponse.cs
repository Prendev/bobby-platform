using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Plus500Integration
{
	//[JsonConverter(typeof(StringEnumConverter))]
	//public enum Op1
	//{
	//	[JsonProperty("Buy")]
	//	Buy,

	//	[JsonProperty("Sell")]
	//	Sell
	//}

	public class PositionResponse
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("openTime")]
		public DateTime? OpenTime { get; set; }

		[JsonProperty("type")]
		public Op? Type { get; set; }

		[InvisibleColumn]
		[JsonProperty("amount")]
		public string Amount { get; set; }

		public decimal Unit
		{
			get => ConvertAmountToUnit();
		}

		[JsonProperty("symbol")]
		public string Symbol { get; set; }

		[JsonProperty("currentValue")]
		public double? CurrentValue { get; set; }

		[JsonProperty("openValue")]
		public double? OpenValue { get; set; }

		[JsonProperty("openingRate")]
		public double? OpeningRate { get; set; }

		[JsonProperty("maintenanceMargin")]
		public double? MaintenanceMargin { get; set; }

		[JsonProperty("initialMargin")]
		public double? InitialMargin { get; set; }

		[JsonProperty("takeProfit")]
		public double? TakeProfit { get; set; }

		[JsonProperty("stopLoss")]
		public double? StopLoss { get; set; }

		[JsonProperty("trailingStop")]
		public double? TrailingStop { get; set; }

		[JsonProperty("profit")]
		public double? Profit { get; set; }

		private decimal ConvertAmountToUnit()
		{
			// Regular expression to match numeric part
			// Step 1: Remove non-numeric characters except for the decimal point
			string cleanedStr = Regex.Replace(Amount, "[^0-9.,]", "");

			// Step 2: Remove commas
			cleanedStr = cleanedStr.Replace(",", "");

			var success = decimal.TryParse(cleanedStr, out var value);

			return success ? value : 0;
		}
	}
}

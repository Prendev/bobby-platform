using System.ComponentModel.DataAnnotations;

namespace TradeSystem.Data.Models
{
	public partial class MM : StrategyEntityBase
	{
		public int MakerAccountId { get; set; }
		/// <summary>
		/// The exchange where the bot will place maker orders (limits).
		/// </summary>
		public Account MakerAccount { get => Get<Account>(); set => Set(value); }
		[Required] public string MakerSymbol { get; set; }

		public int TakerAccountId { get; set; }
		/// <summary>
		/// The exchange where the bot will execute taker orders (hedge markets).
		/// </summary>
		public Account TakerAccount { get => Get<Account>(); set => Set(value); }
		[Required] public string TakerSymbol { get; set; }

		/// <summary>
		/// Minimum required profitability in order to place an order on the maker exchange.
		/// </summary>
		public decimal MinProfitabilityInTick { get; set; } = 1;

		/// <summary>
		/// If enabled, the strategy will place the order on top of the top bid and ask if it is more profitable to place it there.
		/// If disabled, the strategy will ignore the top of the maker order book for price calculations and only place the order based on taker price and MinProfitabilityInTick.
		/// Refer to Adjusting orders and maker price calculations section above. Default value: True
		/// </summary>
		public bool AdjustOrderEnabled { get; set; } = true;

		/// <summary>
		/// Use mimimum size
		/// </summary>
		public decimal OrderSize { get; set; } = 1;

		public decimal TickSize { get; set; }
	}
}

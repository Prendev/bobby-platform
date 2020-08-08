using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class MM : BaseDescriptionEntity
	{
		[DisplayPriority(-1)] public bool Run { get; set; }

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

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
		/// Use mimimum size
		/// </summary>
		public decimal OrderSize { get; set; } = 1;

		public decimal TickSize { get; set; }
	}
}

using System;

namespace TradeSystem.Plus500Integration
{
	/// <summary>
	/// Contains order data.
	/// </summary>
	public class Position
	{
		/// <summary>
		/// Position id number.
		/// </summary>
		public int Id;

		/// <summary>
		/// Open time.
		/// </summary>
		public DateTime OpenTime;

		/// <summary>
		/// Close time. Just for history orders.
		/// </summary>
		public DateTime CloseTime;

		/// <summary>
		/// Expiration time of pending order.
		/// </summary>
		public DateTime Expiration;

		/// <summary>
		/// Order type.
		/// </summary>
		public Op Type;

		/// <summary>
		/// Amount of position.
		/// </summary>
		public double Amount;

		/// <summary>
		/// Trading instrument.
		/// </summary>
		public string Symbol;

		/// <summary>
		/// Open price.
		/// </summary>
		public double OpenValue;

		/// <summary>
		/// Close value.
		/// </summary>
		public double CloseValue;

		/// <summary>
		/// Convertation rate from profit currency to group deposit currency for open time.
		/// </summary>
		public double OpeningRate;

		/// <summary>
		/// Convertation rate from profit currency to group deposit currency for close time.
		/// </summary>
		public double CloseRate;

		/// <summary>
		/// To keep a position open, available account equity must exceed maintenance margin level. The maintenance margin level requirement is specific for each financial instrument.
		/// </summary>
		public double MaintenanceMargin;

		/// <summary>
		/// To open a new position, the available account equity must exceed the initial margin level requirement.
		/// </summary>
		public double InitialMargin;

		/// <summary>
		/// Close at profit
		/// </summary>
		public double TakeProfit;

		/// <summary>
		/// Close at loss
		/// </summary>
		public double StopLoss;

		/// <summary>
		/// Place a stop loss order which automatically updates to lock...
		/// </summary>
		public double TrailingStop;

		/// <summary>
		/// PnL - Net profit value in base currency.
		/// </summary>
		public double Profit;

		/// <summary>
		/// Convert to string.
		/// </summary>
		/// <returns>"Ticket Symbol Type"</returns>
		public override string ToString()
		{
			return Id + " " + Symbol + " " + Type;
		}
	}

	/// <summary>
	/// Operation type for the OrderSend() function.
	/// </summary>
	public enum Op
	{
		/// <summary>
		/// Buying position.
		/// </summary>
		Buy,

		/// <summary>
		/// Selling position.
		/// </summary>
		Sell,

		/// <summary>
		/// Buy limit pending position.
		/// </summary>
		BuyLimit,

		/// <summary>
		/// Sell limit pending position.
		/// </summary>
		SellLimit,
	}
}

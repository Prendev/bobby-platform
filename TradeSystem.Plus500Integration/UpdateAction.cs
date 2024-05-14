using System;
namespace TradeSystem.Plus500Integration
{
	/// <summary>
	/// Type of update action for OrderUpdate event.
	/// </summary>
	public enum UpdateAction
	{
		/// <summary>
		/// New postion.
		/// </summary>
		PositionOpen,

		/// <summary>
		/// Position closed full or partially.
		/// </summary>
		PositionClose,

		/// <summary>
		/// New pening order.
		/// </summary>
		PendingOpen,

		/// <summary>
		/// Pending order was deleted.
		/// </summary>
		PendingClose,
	}
}

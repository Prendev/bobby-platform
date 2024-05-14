using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSystem.Plus500Integration
{
	/// <summary>
	///  Arguments for OrderUpdate event.
	/// </summary>
	public class OrderUpdateEventArgs
	{
		/// <summary>
		/// Updated order.
		/// </summary>
		public Position Order;

		/// <summary>
		/// Update action
		/// </summary>
		public UpdateAction Action;
	}
}

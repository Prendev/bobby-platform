﻿using System.Threading.Tasks;

namespace QvaDev.Common.Integration
{
	public interface IFixConnector : IConnector
	{
		Task<decimal> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null);
		void OrderMultipleCloseBy(string symbol);
		SymbolData GetSymbolInfo(string symbol);
	}
}

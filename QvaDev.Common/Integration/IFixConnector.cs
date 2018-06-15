namespace QvaDev.Common.Integration
{
	public interface IFixConnector : IConnector
	{
		double SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null);
		void OrderMultipleCloseBy(string symbol);
		SymbolData GetSymbolInfo(string symbol);
	}
}

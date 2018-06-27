namespace QvaDev.Common.Integration
{
	public interface IFixConnector : IConnector
	{
		decimal SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null);
		void OrderMultipleCloseBy(string symbol);
		SymbolData GetSymbolInfo(string symbol);
		void Subscribe(string symbol);
	}
}

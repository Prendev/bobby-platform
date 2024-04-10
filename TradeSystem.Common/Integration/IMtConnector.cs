namespace TradeSystem.Common.Integration
{
	public interface IMtConnector : IConnector
	{
		PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots);

		PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber,
			string comment, int maxRetryCount, int retryPeriodInMs);

		PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots, decimal price, decimal deviation, int magicNumber,
			string comment, int maxRetryCount, int retryPeriodInMs);

		PositionResponse SendClosePositionRequests(Position position);
		PositionResponse SendClosePositionRequests(long ticket, int maxRetryCount, int retryPeriodInMs);
		PositionResponse SendClosePositionRequests(Position position, int maxRetryCount, int retryPeriodInMs);
	}
}

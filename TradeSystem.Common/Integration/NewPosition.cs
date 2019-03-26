namespace TradeSystem.Common.Integration
{
	public enum NewPositionActions
	{
		Open,
		Close
	}

	public enum NewPositionOrderTypes
	{
		Market,
		Pending
	}

	public class NewPosition
    {
        public Position Position { get; set; }
        public NewPositionActions Action { get; set; }
        public AccountTypes AccountType { get; set; }
        public NewPositionOrderTypes OrderType { get; set; }
	}
}

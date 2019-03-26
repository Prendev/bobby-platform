namespace TradeSystem.Common.Integration
{
	public enum NewPositionActions
	{
		Open,
		Close
	}

	public class NewPosition
    {
        public Position Position { get; set; }
        public NewPositionActions Action { get; set; }
        public AccountTypes AccountType { get; set; }
	}
}

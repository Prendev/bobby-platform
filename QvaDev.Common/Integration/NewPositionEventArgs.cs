namespace QvaDev.Common.Integration
{
    public delegate void NewPositionEventHandler(object sender, NewPositionEventArgs e);

    public class NewPositionEventArgs
    {
        public enum Actions
        {
            Open,
            Close
        }

        public Position Position { get; set; }
        public Actions Action { get; set; }
        public AccountTypes AccountType { get; set; }
	}
}

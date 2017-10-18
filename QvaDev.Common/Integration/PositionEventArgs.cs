namespace QvaDev.Common.Integration
{
    public delegate void PositionEventHandler(object sender, PositionEventArgs e);

    public class PositionEventArgs
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

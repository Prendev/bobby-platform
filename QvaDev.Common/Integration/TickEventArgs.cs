namespace QvaDev.Common.Integration
{
    public delegate void TickEventHandler(object sender, TickEventArgs e);

    public class TickEventArgs
    {
        public Tick Tick { get; set; }
    }
}

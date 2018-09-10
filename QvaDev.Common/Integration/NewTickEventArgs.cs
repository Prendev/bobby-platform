namespace QvaDev.Common.Integration
{
    public delegate void NewTickEventHandler(object sender, NewTickEventArgs e);

    public class NewTickEventArgs
    {
        public Tick Tick { get; set; }
    }
}

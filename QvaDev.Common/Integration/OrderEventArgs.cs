using System;

namespace QvaDev.Common.Integration
{
    public delegate void OrderEventHandler(object sender, OrderEventArgs e);

    public class OrderEventArgs
    {
        public enum Sides
        {
            Sell,
            Buy
        }

        public enum Actions
        {
            Open,
            Close
        }

        public string AccountDescription { get; set; }
        public string Symbol { get; set; }
        public Sides Side { get; set; }
        public Actions Action { get; set; }
        public long Volume { get; set; }
        public int Ticket { get; set; }
        public DateTime OpenTime { get; set; }
        public double OperPrice { get; set; }
    }
}

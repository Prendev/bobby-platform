using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Common.Integration
{
    public class AccountMetric : BaseNotifyPropertyChange
    {
        [ReadOnly(true)]
		[DisplayName(nameof(Metric))]
		public string Metric { get => Get<string>(); set => Set(value); }

		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double Sum { get => Get<double>(); set => Set(value); }
    }

	public sealed class Metric
	{
		public static readonly string Balance = new Metric("Balance").Value;
		public static readonly string Equity = new Metric("Equity").Value;
		public static readonly string PnL = new Metric("PnL").Value;
		public static readonly string Margin = new Metric("Margin").Value;
		public static readonly string FreeMargin = new Metric("Free M").Value;

		public readonly string Value;
		private Metric(string metricValue)
		{
			Value = metricValue;
		}
	}
}

using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Common.Integration
{
    public class AccountMetric : BaseNotifyPropertyChange
    {
        [ReadOnly(true)]
		public string Metric { get => Get<string>(); set => Set(value); }

		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double Sum { get => Get<double>(); set => Set(value); }
    }
}

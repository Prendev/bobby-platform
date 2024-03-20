using System.ComponentModel;
using TradeSystem.Common;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class RiskManagerAccountVisibility : BaseNotifyPropertyChange
	{
		[DisplayName("Accounts")]
		[ReadOnly(true)]
		public string AccountName { get; set; }

		[InvisibleColumn]
        public RiskManagement RiskManagement { get; set; }

        [CheckBox]
		public bool IsVisible { get => Get<bool>(); set => Set(value); }
	}
}

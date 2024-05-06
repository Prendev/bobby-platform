using System.ComponentModel;
using TradeSystem.Common;

namespace TradeSystem.Data.Models
{
	public class KeyValue : BaseNotifyPropertyChange
	{
		[ReadOnly(true)]
		[DisplayName("Settings")]
		public string Key { get; set; }
		public object Value { get => Get<object>(); set => Set(value); }
	}
}

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class RiskManagement
	{
		[ReadOnly(true)]
		[DisplayPriority(0)]
		public string AccountName
		{
			get => Account.MetaTraderAccount?.Description ?? Account.FixApiAccount?.Description ?? "";
		}

		[NotMapped]
		[ReadOnly(true)]
		[DisplayPriority(0)]
		public string Broker
		{
			get => Account.Connector?.Broker ?? "";
		}

		[NotMapped]
		[ReadOnly(true)]
		[DisplayPriority(2)]
		public int AccountAge { get => Get<int>(GetAccountAge); private set => Set(value); }

		private int GetAccountAge()
		{
			if (firstActivity.HasValue)
			{
				var accountAge = (DateTime.Now - firstActivity.Value).Days;
				if (accountAge > 0) return accountAge;
			}
			return 0;
		}
	}
}

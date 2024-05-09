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
		[ShowHideColumn]
		public string Broker
		{
			get => Account.Connector?.Broker ?? "";
		}

		[NotMapped]
		[ReadOnly(true)]
		[DisplayPriority(2)]
		[ShowHideColumn]
		public int AccountAge { get => Get<int>(GetAccountAge); private set => Set(value); }

		[NotMapped]
		[ReadOnly(true)]
		[DisplayPriority(3)]
		[ShowHideColumn]
		[Tooltip("Count of open orders that have been open for the longest duration")]
		public int? HighestTicketDuration { get => Get<int?>(); set => Set(value); }

		[NotMapped]
		[ReadOnly(true)]
		[DisplayPriority(3)]
		[Tooltip("Amount of orders with a value that’s greater than MaxTicketDuration")]
		[ShowHideColumn]
		public int? NumTicketsHighDuration { get => Get<int?>(); set => Set(value); }

		[NotMapped]
		[ReadOnly(true)]
		[DisplayPriority(3)]
		[DecimalPrecision(2)]
		[DisplayName("Low Equity Level")]
		[Tooltip("OptimumEquity * AddEq => Red when Equity level of the account is below LowEquity amount, Yellow when Equity level is within 5% of LowEquity, Otherwise green")]
		[ShowHideColumn]
		public double? LowEquity { get => Get<double?>(); set => Set(value); }


		[NotMapped]
		[ReadOnly(true)]
		[DisplayPriority(3)]
		[DecimalPrecision(2)]
		[DisplayName("Equity")]
		[Tooltip("Account's Equity")]
		[ShowHideColumn]
		public double? Equity { get => Get<double?>(); set => Set(value); }

		[NotMapped]
		[ReadOnly(true)]
		[DisplayPriority(3)]
		[DecimalPrecision(2)]
		[DisplayName("High Equity Level")]
		[Tooltip("OptimumEquity * WdrawEq => Red when Equity level of the account is below HighEquity  amount, Yellow when Equity level is within 5% of HighEquity , Otherwise green")]
		[ShowHideColumn]
		public double? HighEquity { get => Get<double?>(); set => Set(value); }

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

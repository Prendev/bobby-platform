using System;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class RiskManagement : BaseEntity
	{
		private DateTime? firstActivity;

		[InvisibleColumn] public int AccountId { get; set; }
		[InvisibleColumn] public Account Account { get; set; }
		[InvisibleColumn] public RiskManagementSetting RiskManagementSetting { get; set; }

		[DateTimePicker("yyyy.MM.dd")]
		[DisplayPriority(1)]
		[ShowHideColumn]
		public DateTime? FirstActivity
		{
			get => firstActivity;
			set
			{
				firstActivity = value;
				AccountAge = GetAccountAge();
			}
		}

		[DisplayPriority(4)]
		[InvisibleColumn]
		[ShowHideColumn]
		public bool Regulated { get; set; }

		[DisplayPriority(4)]
		[InvisibleColumn]
		[ShowHideColumn]
		public double PnL { get; set; }

		[DisplayPriority(4)]
		[InvisibleColumn]
		[Tooltip(" Hypothetical accrued swaps in money")]
		[ShowHideColumn]
		public double HypoAccSwaps { get; set; }
	}
}

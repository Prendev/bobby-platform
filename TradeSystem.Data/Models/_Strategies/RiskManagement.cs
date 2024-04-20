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
		public DateTime? FirstActivity
		{
			get => firstActivity;
			set
			{
				firstActivity = value;
				AccountAge = GetAccountAge();
			}
		}

		[DisplayPriority(3)]
		public bool Regulated { get; set; }

		[DisplayPriority(3)]
		public double PnL { get; set; }

		[DisplayPriority(3)]
		[Tooltip(" Hypothetical accrued swaps in money")]
		public double HypoAccSwaps { get; set; }
	}
}

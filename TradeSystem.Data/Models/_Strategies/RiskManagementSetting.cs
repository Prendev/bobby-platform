using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class RiskManagementSetting : BaseEntity
	{
		[InvisibleColumn] public int RiskManagementId { get; set; }
		[InvisibleColumn]
		public RiskManagement RiskManagement
		{
			get => Get<RiskManagement>();
			set
			{
				Set(value);
				if (value != null)
				{
					RiskManagement.LowEquity = OptimumEquity * AddEq;
					RiskManagement.HighEquity = OptimumEquity * WdrawEq;
				}
			}
		}

		public int MaxAccAge { get; set; }

		[DisplayName("Max P/L")]
		public double MaxPnL { get; set; }

		public double MaxSwaps { get; set; }

		public int MaxTicketDuration { get; set; }
		public double OptimumEquity
		{
			get
			{
				return Get<double>();
			}
			set
			{
				Set(value);
				if (RiskManagement != null)
				{
					RiskManagement.LowEquity = OptimumEquity * AddEq;
					RiskManagement.HighEquity = OptimumEquity * WdrawEq;
				}
			}
		}

		public double AddEq
		{
			get => Get<double>();
			set
			{
				Set(value);
				if (RiskManagement != null) RiskManagement.LowEquity = OptimumEquity * AddEq;
			}
		}
		public double WdrawEq
		{
			get => Get<double>();
			set
			{
				Set(value);
				if (RiskManagement != null) RiskManagement.HighEquity = OptimumEquity * WdrawEq;
			}
		}
	}
}

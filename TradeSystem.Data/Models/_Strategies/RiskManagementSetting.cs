using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class RiskManagementSetting : BaseEntity
    {
        [InvisibleColumn] public int RiskManagementId { get; set; }
        [InvisibleColumn] public RiskManagement RiskManagement { get; set; }

        public int MaxAccAge { get; set; }

		[DisplayName("Max P/L")]
		public double MaxPnL { get; set; }

        public double MaxSwaps { get; set; }

		public int MaxTicketDuration { get; set; }
		public double OptimumEquity { get; set; }
		public double AddEq { get; set; }
		public double WdrawEq { get; set; }
	}
}

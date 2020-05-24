using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeSystem.Data.Models
{
	public partial class BacktesterAccount
	{
		[NotMapped] [ReadOnly(true)] public DateTime UtcNow { get; set; }
	}
}

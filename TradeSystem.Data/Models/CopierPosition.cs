using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class CopierPosition : BaseEntity
	{
		public long MasterTicket { get; set; }
		public long SlaveTicket { get; set; }

		[InvisibleColumn] public int CopierId { get; set; }
		[InvisibleColumn] public Copier Copier { get; set; }
	}
}

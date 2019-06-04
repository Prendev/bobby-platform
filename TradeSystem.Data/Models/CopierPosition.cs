using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class CopierPosition : BaseEntity
	{
		public long MasterTicket { get; set; }
		public long SlaveTicket { get; set; }

		[InvisibleColumn] public int CopierId { get; set; }
		[InvisibleColumn] public Copier Copier { get; set; }

		[NotMapped] [InvisibleColumn] public bool Remove { get => Get<bool>(); set => Set(value); }
		public bool Paused { get => Get<bool>(); set => Set(value); }
	}
}

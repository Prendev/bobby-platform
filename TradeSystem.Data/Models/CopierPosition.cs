using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class CopierPosition : BaseEntity
	{
		public long MasterTicket { get => Get<long>(); set => Set(value); }
		public long SlaveTicket { get => Get<long>(); set => Set(value); }

		[InvisibleColumn] public int CopierId { get; set; }
		[InvisibleColumn] public Copier Copier { get; set; }

		[NotMapped] [InvisibleColumn] public bool Remove { get => Get<bool>(); set => Set(value); }
		public bool Paused { get => Get<bool>(); set => Set(value); }
	}
}

using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class CopierPosition : BaseEntity
	{
		public enum CopierPositionStates
		{
			Active,
			Paused,
			Inactive,
			ToBeRemoved
		}

		public long MasterTicket { get => Get<long>(); set => Set(value); }
		public long SlaveTicket { get => Get<long>(); set => Set(value); }

		[InvisibleColumn] public int CopierId { get; set; }
		[InvisibleColumn] public Copier Copier { get; set; }

		public CopierPositionStates State { get => Get<CopierPositionStates>(); set => Set(value); }
	}
}

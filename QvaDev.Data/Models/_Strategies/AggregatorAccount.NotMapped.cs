using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
	public partial class AggregatorAccount
	{
		[NotMapped] [ReadOnly(true)] [DisplayPriority(1, true)] public decimal? Ask { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] [ReadOnly(true)] [DisplayPriority(0, true)] public decimal? Bid { get => Get<decimal?>(); set => Set(value); }

		public AggregatorAccount()
		{
			SetAction<Account>(nameof(Account),
				a => { if (a != null) a.NewTick -= Account_NewTick; },
				a => { if (a != null) a.NewTick += Account_NewTick; });
		}

		private void Account_NewTick(object sender, NewTick newTick)
		{
			if (newTick?.Tick?.Symbol != Symbol) return;
			Ask = newTick?.Tick?.Ask;
			Bid = newTick?.Tick?.Bid;
		}
	}
}

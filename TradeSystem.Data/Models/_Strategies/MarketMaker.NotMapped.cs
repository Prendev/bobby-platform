using System;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{

	public partial class MarketMaker
	{
		public event EventHandler<NewTick> FeedNewTick;
		public event EventHandler<LimitFill> LimitFill;

		//[NotMapped]
		//[InvisibleColumn]
		//public bool IsBusy
		//{
		//	get => _isBusy;
		//	set => _isBusy = value;
		//}
		//private volatile bool _isBusy;

		public MarketMaker()
		{
			SetAction<Account>(nameof(FeedAccount),
				a => { if (a != null) a.NewTick -= FeedAccount_NewTick; },
				a => { if (a != null) a.NewTick += FeedAccount_NewTick; });

			SetAction<Account>(nameof(TradeAccount),
				a => { if (a != null) a.LimitFill -= TradeAccount_LimitFill; },
				a => { if (a != null) a.LimitFill += TradeAccount_LimitFill; });
		}

		private void FeedAccount_NewTick(object sender, NewTick newTick) => FeedNewTick?.Invoke(this, newTick);
		private void TradeAccount_LimitFill(object sender, LimitFill limitFill) => LimitFill?.Invoke(this, limitFill);
	}
}

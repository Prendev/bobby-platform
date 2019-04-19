using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public partial class MarketMaker
	{
		public class Level
		{
			public LimitResponse LongOpenLimitResponse { get; set; }
			public LimitResponse LongCloseLimitResponse { get; set; }

			public LimitResponse ShortOpenLimitResponse { get; set; }
			public LimitResponse ShortCloseLimitResponse { get; set; }
		}

		public event EventHandler<NewTick> FeedNewTick;
		public event EventHandler<LimitFill> LimitFill;

		[NotMapped]
		[InvisibleColumn]
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}
		private volatile bool _isBusy;

		[NotMapped] [InvisibleColumn] public decimal? BottomBase { get; set; }
		[NotMapped] [InvisibleColumn] public decimal? TopBase { get; set; }
		[NotMapped] [InvisibleColumn] public decimal? LowestLimit { get; set; }
		[NotMapped] [InvisibleColumn] public decimal? HighestLimit { get; set; }
		[NotMapped] [InvisibleColumn] public readonly ConcurrentBag<LimitResponse> MarketMakerLimits = new ConcurrentBag<LimitResponse>();
		[NotMapped] [InvisibleColumn] public readonly ConcurrentDictionary<int, Level> AntiLevels = new ConcurrentDictionary<int, Level>();

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

using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
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

		public event EventHandler<NewTick> NewTick;
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
		[NotMapped] [InvisibleColumn] public readonly ConcurrentDictionary<string, StopResponse> AntiMarketMakerTopStops = new ConcurrentDictionary<string, StopResponse>();
		[NotMapped] [InvisibleColumn] public readonly ConcurrentDictionary<string, StopResponse> AntiMarketMakerBottomStops = new ConcurrentDictionary<string, StopResponse>();
		[NotMapped] [InvisibleColumn] public CancellationToken Token { get; set; }

		public MarketMaker()
		{
			SetAction<Account>(nameof(Account),
				a =>
				{
					if (a == null) return;
					a.NewTick -= Account_NewTick;
					a.LimitFill -= Account_LimitFill;
				},
				a =>
				{
					if (a == null) return;
					a.NewTick += Account_NewTick;
					a.LimitFill += Account_LimitFill;

				});
		}

		private void Account_NewTick(object sender, NewTick newTick) => NewTick?.Invoke(this, newTick);
		private void Account_LimitFill(object sender, LimitFill limitFill) => LimitFill?.Invoke(this, limitFill);
	}
}

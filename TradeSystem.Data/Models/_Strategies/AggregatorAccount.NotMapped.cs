using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public partial class AggregatorAccount
	{
		private readonly List<Tick> _ticks = new List<Tick>();

		public event EventHandler<NewTick> NewTick;

		[NotMapped] [ReadOnly(true)] [DisplayPriority(2, true)] public decimal? Ask { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] [ReadOnly(true)] [DisplayPriority(1, true)] public decimal? Bid { get => Get<decimal?>(); set => Set(value); }
		[NotMapped] [ReadOnly(true)] [DisplayPriority(0, true)] public decimal? Avg { get => Get<decimal?>(); set => Set(value); }

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

			if (newTick?.Tick?.HasValue != true) return;
			if (Aggregator.AveragingPeriodInSeconds <= 0)
			{
				Avg = null;
			}
			else
			{
				lock (_ticks)
				{
					var doAvg = false;
					while (_ticks.Any() && newTick.Tick.Time - _ticks.First().Time >
					       TimeSpan.FromSeconds(Aggregator.AveragingPeriodInSeconds))
					{
						_ticks.RemoveAt(0);
						doAvg = true;
					}
					_ticks.Add(newTick.Tick);
					if (doAvg || Avg.HasValue) Avg = _ticks.Select(t => (t.Ask + t.Bid) / 2).Average();
				}
			}
			NewTick?.Invoke(this, newTick);
		}
	}
}

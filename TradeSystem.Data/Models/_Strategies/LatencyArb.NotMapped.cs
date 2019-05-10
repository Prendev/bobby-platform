﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public partial class LatencyArb
	{
		public event EventHandler<NewTick> NewTick;
		public event EventHandler<ConnectionStates> ConnectionChanged;

		[NotMapped] [InvisibleColumn] public bool IsConnected { get => Get<bool>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public Tick LastFeedTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastShortTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastLongTick { get; set; }

		public LatencyArb()
		{
			SetAction<Account>(nameof(FastFeedAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(ShortAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(LongAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });

			SetAction<Account>(nameof(FastFeedAccount),
				a => { if (a != null) a.NewTick -= Account_NewTick; },
				a => { if (a != null) a.NewTick += Account_NewTick; });
			SetAction<Account>(nameof(ShortAccount),
				a => { if (a != null) a.NewTick -= Account_NewTick; },
				a => { if (a != null) a.NewTick += Account_NewTick; });
			SetAction<Account>(nameof(LongAccount),
				a => { if (a != null) a.NewTick -= Account_NewTick; },
				a => { if (a != null) a.NewTick += Account_NewTick; });
		}

		private void Account_NewTick(object sender, NewTick newTick)
		{
			if (newTick?.Tick == null) return;
			if (sender == FastFeedAccount && newTick.Tick.Symbol != FastFeedSymbol) return;
			if (sender == ShortAccount && newTick.Tick.Symbol != ShortSymbol) return;
			if (sender == LongAccount && newTick.Tick.Symbol != LongSymbol) return;

			if (sender == FastFeedAccount) LastFeedTick = newTick.Tick;
			if (sender == ShortAccount) LastShortTick = newTick.Tick;
			if (sender == LongAccount) LastLongTick = newTick.Tick;

			if (LastFeedTick?.HasValue != true) return;
			if (LastShortTick?.HasValue != true) return;
			if (LastLongTick?.HasValue != true) return;

			NewTick?.Invoke(this, newTick);
		}

		private void Account_ConnectionChanged(object sender, ConnectionStates connectionStates)
		{
			IsConnected =
				FastFeedAccount?.Connector?.IsConnected == true &&
				ShortAccount?.Connector?.IsConnected == true &&
				LongAccount?.Connector?.IsConnected == true;
			ConnectionChanged?.Invoke(this, IsConnected ? ConnectionStates.Connected : ConnectionStates.Disconnected);
		}
	}
}
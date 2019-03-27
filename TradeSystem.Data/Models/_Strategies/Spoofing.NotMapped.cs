using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public partial class Spoofing
	{
		public event EventHandler<ConnectionStates> ConnectionChanged;

		[NotMapped] [InvisibleColumn] public Spoof Spoof { get; set; }
		[NotMapped] [InvisibleColumn] public IStratState SpoofState { get; set; }
		[NotMapped] [InvisibleColumn] public decimal PrevFilledQuantity { get; set; }

		[NotMapped] [InvisibleColumn] public Spoof Pull { get; set; }
		[NotMapped] [InvisibleColumn] public IStratState PullState { get; set; }

		[NotMapped] [InvisibleColumn] public Push Push { get; set; }
		[NotMapped] [InvisibleColumn] public IStratState PushState { get; set; }

		[NotMapped] [InvisibleColumn] public Sides BetaOpenSide { get; set; }
		[NotMapped] [InvisibleColumn] public Sides FirstCloseSide { get; set; }
		[NotMapped] [InvisibleColumn] public Position AlphaPosition { get; set; }
		[NotMapped] [InvisibleColumn] public Position BetaPosition { get; set; }
		[NotMapped] [InvisibleColumn] public ConcurrentBag<Position> HedgePositions { get; set; } = new ConcurrentBag<Position>();
		/// <summary>
		/// Use only from orchestration
		/// </summary>
		[NotMapped] [InvisibleColumn] public CancellationTokenSource PanicSource { get; set; }
		[NotMapped] [InvisibleColumn] public bool IsConnected { get => Get<bool>(); set => Set(value); }

		[NotMapped] [InvisibleColumn] public Tick LastFeedTick { get; set; }


		public Spoofing()
		{
			SetAction<Account>(nameof(FeedAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(SpoofAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(AlphaMaster),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(BetaMaster),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(FeedAccount),
				a => { if (a != null) a.NewTick -= Account_NewTick; },
				a => { if (a != null) a.NewTick += Account_NewTick; });
		}

		private void Account_NewTick(object sender, NewTick newTick)
		{
			if (newTick?.Tick == null) return;
			if (newTick.Tick.Symbol != FeedSymbol) return;
			LastFeedTick = newTick.Tick;
		}

		private void Account_ConnectionChanged(object sender, ConnectionStates connectionStates)
		{
			IsConnected =
				AlphaMaster?.Connector?.IsConnected == true &&
				BetaMaster?.Connector?.IsConnected == true &&
				FeedAccount?.Connector?.IsConnected == true &&
				SpoofAccount?.Connector?.IsConnected == true;
			ConnectionChanged?.Invoke(this, IsConnected ? ConnectionStates.Connected : ConnectionStates.Disconnected);
		}
	}
}

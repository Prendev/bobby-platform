using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public partial class MM
	{
		public event EventHandler<NewTick> NewTick;
		public event EventHandler<LimitFill> LimitFill;
		public event EventHandler<ConnectionStates> ConnectionChanged;

		[NotMapped]
		[InvisibleColumn]
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}
		private volatile bool _isBusy;

		[NotMapped] [InvisibleColumn] public DateTime? LastActionTime { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastMakerTick { get; set; }
		[NotMapped] [InvisibleColumn] public Tick LastTakerTick { get; set; }

		[NotMapped]
		[InvisibleColumn]
		public bool IsConnected
		{
			get => Get<bool>();
			set => Set(value);
		}

		[NotMapped] [InvisibleColumn] public DateTime UtcNow =>
			MakerAccount.BacktesterAccount?.UtcNow ?? HiResDatetime.UtcNow;
		[NotMapped] [InvisibleColumn] public AutoResetEvent WaitHandle { get; } = new AutoResetEvent(false);

		/// <summary>
		/// Const`
		/// </summary>
		public MM() : base(e => ((MM)e).MakerAccount, e => ((MM)e).TakerAccount)
		{
		}

		public void OnTickProcessed() => MakerAccount.Connector.OnTickProcessed();

		public void Reset()
		{
			LastActionTime = null;
			LastMakerTick = null;
			LastTakerTick = null;
		}

		/// <inheritdoc/>
		protected override bool AccountNewTickCore(Account account, NewTick newTick)
		{
			var tick = newTick?.Tick;
			if (tick == null) return false;

			var newTickFound = false;
			if (account == MakerAccount && tick.Symbol == MakerSymbol && LastMakerTick != tick)
			{
				newTickFound = true;
				LastMakerTick = tick;
			}
			if (account == TakerAccount && tick.Symbol == TakerSymbol && LastTakerTick != tick)
			{
				newTickFound = true;
				LastTakerTick = tick;
			}

			return newTickFound;
		}

		private void Account_LimitFill(object sender, LimitFill limitFill) => LimitFill?.Invoke(this, limitFill);

		private void Account_ConnectionChanged(object sender, ConnectionStates connectionStates)
		{
			IsConnected =
				MakerAccount?.Connector?.IsConnected == true &&
				TakerAccount?.Connector?.IsConnected == true;
			ConnectionChanged?.Invoke(this, IsConnected ? ConnectionStates.Connected : ConnectionStates.Disconnected);
		}
	}
}

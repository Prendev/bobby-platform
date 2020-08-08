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

		public MM()
		{
			SetAction<Account>(nameof(MakerAccount),
				a =>
				{
					if (a == null) return;
					a.NewTick -= Account_NewTick;
					a.LimitFill -= Account_LimitFill;
					a.ConnectionChanged -= Account_ConnectionChanged;
				},
				a =>
				{
					if (a == null) return;
					a.NewTick += Account_NewTick;
					a.LimitFill += Account_LimitFill;
					a.ConnectionChanged += Account_ConnectionChanged;

				});
			SetAction<Account>(nameof(TakerAccount),
				a =>
				{
					if (a == null) return;
					a.NewTick -= Account_NewTick;
					a.LimitFill -= Account_LimitFill;
					a.ConnectionChanged -= Account_ConnectionChanged;
				},
				a =>
				{
					if (a == null) return;
					a.NewTick += Account_NewTick;
					a.LimitFill += Account_LimitFill;
					a.ConnectionChanged += Account_ConnectionChanged;

				});
		}

		public void OnTickProcessed() => MakerAccount.Connector.OnTickProcessed();

		public void Reset()
		{
			LastActionTime = null;
			LastMakerTick = null;
			LastTakerTick = null;
		}

		private void Account_NewTick(object sender, NewTick newTick)
		{
			var tick = newTick?.Tick;
			if (tick == null) return;

			var newTickFound = false;
			if (sender == MakerAccount && tick.Symbol == MakerSymbol && LastMakerTick != tick)
			{
				newTickFound = true;
				LastMakerTick = tick;
			}
			if (sender == TakerAccount && tick.Symbol == TakerSymbol && LastTakerTick != tick)
			{
				newTickFound = true;
				LastTakerTick = tick;
			}

			if (!newTickFound) return;

			NewTick?.Invoke(this, newTick);
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

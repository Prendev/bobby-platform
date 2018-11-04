using System;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
    public partial class Pushing
	{
		public event EventHandler<ConnectionStates> ConnectionChanged;

		[NotMapped] [InvisibleColumn] public Sides BetaOpenSide { get; set; }
		[NotMapped] [InvisibleColumn] public Sides FirstCloseSide { get; set; }
		[NotMapped] [InvisibleColumn] public bool IsHedgeClose { get; set; } = true;
		[NotMapped] [InvisibleColumn] public Position AlphaPosition { get; set; }
		[NotMapped] [InvisibleColumn] public Position BetaPosition { get; set; }
		[NotMapped] [InvisibleColumn] public bool InPanic { get; set; }
		[NotMapped] [InvisibleColumn] public bool IsConnected { get => Get<bool>(); set => Set(value); }

		public Pushing()
		{
			SetAction<Account>(nameof(FutureAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(AlphaMaster),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(BetaMaster),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
			SetAction<Account>(nameof(HedgeAccount),
				a => { if (a != null) a.ConnectionChanged -= Account_ConnectionChanged; },
				a => { if (a != null) a.ConnectionChanged += Account_ConnectionChanged; });
		}

		private void Account_ConnectionChanged(object sender, ConnectionStates connectionStates)
		{
			IsConnected =
				FutureAccount?.Connector?.IsConnected == true &&
				AlphaMaster?.Connector?.IsConnected == true &&
				BetaMaster?.Connector?.IsConnected == true &&
				HedgeAccount?.Connector?.IsConnected == true;
			ConnectionChanged?.Invoke(this, IsConnected ? ConnectionStates.Connected : ConnectionStates.Disconnected);
		}
	}
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
    public class Pushing : BaseDescriptionEntity
	{
		public enum FutureExecutionModes
		{
			Confirmed,
			NonConfirmed
		}

		public event ConnectionChangedEventHandler ConnectionChanged;

		[InvisibleColumn] public int ProfileId { get; set; }
        [InvisibleColumn] public Profile Profile { get; set; }

		public FutureExecutionModes FutureExecutionMode { get; set; }
		public int FutureAccountId { get; set; }
		private Account _futureAccount;
        public Account FutureAccount
		{
			get => _futureAccount;
			set
			{
				if (_futureAccount != null)
				{
					_futureAccount.ConnectionChanged -= ConnectionChanged;
				}
				if (value != null)
				{
					value.ConnectionChanged -= Account_ConnectionChanged;
					value.ConnectionChanged += Account_ConnectionChanged;
				}
				_futureAccount = value;
			}
		}

		[Required] public string FutureSymbol { get; set; }

        public int AlphaMasterId { get; set; }
		private Account _alphaMaster;
		public Account AlphaMaster
		{
			get => _alphaMaster;
			set
			{
				if (_alphaMaster != null)
				{
					_alphaMaster.ConnectionChanged -= ConnectionChanged;
				}
				if (value != null)
				{
					value.ConnectionChanged -= Account_ConnectionChanged;
					value.ConnectionChanged += Account_ConnectionChanged;
				}
				_alphaMaster = value;
			}
		}
		[Required] public string AlphaSymbol { get; set; }

        public int BetaMasterId { get; set; }
		private Account _betaMaster;
		public Account BetaMaster
		{
			get => _betaMaster;
			set
			{
				if (_betaMaster != null)
				{
					_betaMaster.ConnectionChanged -= ConnectionChanged;
				}
				if (value != null)
				{
					value.ConnectionChanged -= Account_ConnectionChanged;
					value.ConnectionChanged += Account_ConnectionChanged;
				}
				_betaMaster = value;
			}
		}
		[Required] public string BetaSymbol { get; set; }

        public int HedgeAccountId { get; set; }
		private Account _hedgeAccount;
		public Account HedgeAccount
		{
			get => _hedgeAccount;
			set
			{
				if (_hedgeAccount != null)
				{
					_hedgeAccount.ConnectionChanged -= ConnectionChanged;
				}
				if (value != null)
				{
					value.ConnectionChanged -= Account_ConnectionChanged;
					value.ConnectionChanged += Account_ConnectionChanged;
				}
				_hedgeAccount = value;
			}
		}
		[Required] public string HedgeSymbol { get; set; }

        [InvisibleColumn] public int PushingDetailId { get; set; }
        [InvisibleColumn] public PushingDetail PushingDetail { get; set; }

        [NotMapped] [InvisibleColumn] public Sides BetaOpenSide { get; set; }
        [NotMapped] [InvisibleColumn] public Sides FirstCloseSide { get; set; }
        [NotMapped] [InvisibleColumn] public bool IsHedgeClose { get; set; } = true;
        [NotMapped] [InvisibleColumn] public Position AlphaPosition { get; set; }
        [NotMapped] [InvisibleColumn] public Position BetaPosition { get; set; }
        [NotMapped] [InvisibleColumn] public bool InPanic { get; set; }
		[NotMapped] [InvisibleColumn] public bool IsConnected { get => Get<bool>(); set => Set(value); }

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

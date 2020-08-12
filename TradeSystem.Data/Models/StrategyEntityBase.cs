using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Threading;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	/// <summary>
	/// Entity base class for trade strategies
	/// </summary>
	public abstract class StrategyEntityBase : BaseDescriptionEntity
	{
		private readonly Expression<Func<StrategyEntityBase, Account>>[] _accounts;

		/// <summary>
		/// Event handler for new tick
		/// </summary>
		public event EventHandler<NewTick> NewTick;
		/// <summary>
		/// Event handler for limit fill
		/// </summary>
		public event EventHandler<(Account Account, LimitFill LimitFill)> LimitFill;
		/// <summary>
		/// Event handler for connection change
		/// </summary>
		public event EventHandler<ConnectionStates> ConnectionChanged;

		/// <summary>
		/// Flag for strategy run
		/// </summary>
		[DisplayPriority(-2)]
		public bool Run { get => Get<bool>(); set => Set(value); }

		/// <summary>
		/// Flag for strategy running
		/// </summary>
		[ReadOnly(true)]
		[NotMapped]
		[DisplayPriority(-1)]
		public bool Running { get => Get<bool>(); set => Set(value); }

		/// <summary>
		/// Profile entity ID
		/// </summary>
		[InvisibleColumn]
		public int ProfileId { get; set; }
		/// <summary>
		/// Profile
		/// </summary>
		[InvisibleColumn]
		public Profile Profile { get; set; }

		/// <summary>
		/// Flag for being connected
		/// </summary>
		[ReadOnly(true)]
		[NotMapped]
		[InvisibleColumn]
		public bool IsConnected { get => Get<bool>(); set => Set(value); }

		/// <summary>
		/// Tick arrived auto reset event
		/// </summary>
		[Browsable(false)]
		[NotMapped]
		[InvisibleColumn]
		public AutoResetEvent WaitHandle { get; } = new AutoResetEvent(false);

		/// <summary>
		/// Limit fill queue
		/// </summary>
		[Browsable(false)]
		[NotMapped]
		[InvisibleColumn]
		public ConcurrentQueue<(Account Account, LimitFill LimitFill)> LimitFills { get; } =
			new ConcurrentQueue<(Account Account, LimitFill LimitFill)>();

		/// <summary>
		/// Strategy base class constructor
		/// </summary>
		/// <param name="accounts">Expression getters for related trade accounts</param>
		protected StrategyEntityBase(params Expression<Func<StrategyEntityBase, Account>>[] accounts)
		{
			this._accounts = accounts;
			foreach (var exp in accounts)
			{
				var propertyName = ((MemberExpression)exp.Body).Member.Name;
				SetAction<Account>(propertyName,
					a =>
					{
						if (a == null) return;
						a.ConnectionChanged -= AccountConnectionChanged;
						a.NewTick -= AccountNewTick;
						a.LimitFill -= AccountLimitFill;
					},
					a =>
					{
						if (a == null) return;
						a.ConnectionChanged += AccountConnectionChanged;
						a.NewTick += AccountNewTick;
						a.LimitFill += AccountLimitFill;
					});
			}
		}

		private void AccountConnectionChanged(object sender, ConnectionStates connectionStates)
		{
			var isConnected = true;
			foreach (var exp in _accounts)
			{
				var account = exp.Compile().Invoke(this);
				isConnected = account?.Connector?.IsConnected == true;
				if (!isConnected) break;
			}

			IsConnected = isConnected;
			ConnectionChanged?.Invoke(this, IsConnected ? ConnectionStates.Connected : ConnectionStates.Disconnected);
		}

		private void AccountNewTick(object sender, NewTick newTick)
		{
			if (AccountNewTickCore((Account) sender, newTick))
				NewTick?.Invoke(this, newTick);
		}

		private void AccountLimitFill(object sender, LimitFill limitFill) =>
			LimitFill?.Invoke(this, ((Account) sender, limitFill));

		/// <summary>
		/// Subscribe to account(s)
		/// </summary>
		public virtual void Subscribe() { }

		/// <summary>
		/// Overridable new tick event handler
		/// </summary>
		/// <param name="account">Sender account</param>
		/// <param name="newTick">New tick event data</param>
		/// <returns>True if event should be raised</returns>
		protected virtual bool AccountNewTickCore(Account account, NewTick newTick) => true;
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Data;

namespace TradeSystem.Orchestration.Services
{
	public interface IPushingService
	{
		IStratState Pushing(Push push, Sides side);
	}

	public class PushingService : IPushingService
	{
		private static readonly TaskCompletionManager<StratState> TaskCompletionManager = new TaskCompletionManager<StratState>(100, 2000);

		private class StratState : IStratState
		{
			private CancellationTokenSource _cancel;

			public Sides Side { get; }
			public ConcurrentBag<OrderResponse> Responses { get; } = new ConcurrentBag<OrderResponse>();
			public decimal FilledQuantity => Responses.Sum(r => r.FilledQuantity);

			public StratState(Sides side)
			{
				Side = side;
			}

			public CancellationToken Init()
			{
				_cancel = new CancellationTokenSource();
				return _cancel.Token;
			}

			public async Task Cancel()
			{
				try
				{
					Logger.Debug("PushingService canceling...");
					if (IsEnded) return;
					var task = TaskCompletionManager.CreateCompletableTask(this);
					_cancel.CancelEx();
					await task;
				}
				catch (Exception e)
				{
					Logger.Error("PushingService.Cancel exception", e);
				}
			}

			public event EventHandler<LimitFill> LimitFill;

			private volatile bool _isEnded;
			public bool IsEnded
			{
				get => _isEnded;
				set => _isEnded = value;
			}
		}

		public IStratState Pushing(Push push, Sides side)
		{
			if (!(push.TradeAccount?.Connector is IFixConnector)) return null;
			if (string.IsNullOrWhiteSpace(push.TradeSymbol)) return null;
			if (side == Sides.None) return null;
			if (push.Size <= 0) return null;
			if (push.MaxOrders < push.Size) return null;

			var state = new StratState(side);
			new Thread(() => Loop(push, state, state.Init())) { IsBackground = true }.Start();
			return state;
		}

		private void Loop(Push push, StratState state, CancellationToken token)
		{
			var tradeConnector = (IFixConnector)push.TradeAccount.Connector;

			var orders = new List<Task>();
			while (!token.IsCancellationRequested)
			{
				try
				{
					if (token.IsCancellationRequested) break;
					if (orders.Count >= push.MaxOrders) break;

					var task = tradeConnector
						.SendMarketOrderRequest(push.TradeSymbol, state.Side, push.Size)
						.ContinueWith(t => state.Responses.Add(t.Result));
					orders.Add(task);

					Thread.Sleep(push.IntervalInMs);
				}
				catch (Exception e)
				{
					Logger.Error("PushingService.Loop exception", e);
				}
			}

			try
			{
				Task.WhenAll(orders).Wait();
			}
			catch (Exception e)
			{
				Logger.Error("PushingService.Loop exception", e);
			}
			finally
			{
				state.IsEnded = true;
				TaskCompletionManager.SetCompleted(state);
				Logger.Debug("PushingService.Loop end");
			}
		}
	}
}

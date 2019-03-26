using System;
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
			public decimal FilledQuantity { get; set; }

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
					var task = TaskCompletionManager.CreateCompletableTask(this);
					_cancel.CancelEx();
					await task;
				}
				catch (Exception e)
				{
					Logger.Error("PushingService.Cancel exception", e);
				}
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

			var orders = new List<Task<OrderResponse>>();
			while (!token.IsCancellationRequested)
			{
				try
				{
					if (token.IsCancellationRequested) break;
					if (orders.Count >= push.MaxOrders) break;

					orders.Add(tradeConnector.SendMarketOrderRequest(push.TradeSymbol, state.Side, push.Size));
					Thread.Sleep(push.IntervalInMs);
				}
				catch (Exception e)
				{
					Logger.Error("PushingService.Loop exception", e);
				}
			}

			try
			{
				var results = Task.WhenAll(orders).Result;
				state.FilledQuantity = results.Sum(r => r.FilledQuantity);
			}
			catch (Exception e)
			{
				Logger.Error("PushingService.Loop exception", e);
			}
			finally
			{
				TaskCompletionManager.SetCompleted(state);
				Logger.Debug("PushingService.Loop end");
			}
		}
	}
}

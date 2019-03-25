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
	public interface IPushImpulseService
	{
		ISpoofingState Pushing(Push push, Sides side);
	}

	public class PushImpulseService : IPushImpulseService
	{
		private static readonly TaskCompletionManager<SpoofingState> TaskCompletionManager = new TaskCompletionManager<SpoofingState>(100, 2000);

		private class SpoofingState : ISpoofingState
		{
			private CancellationTokenSource _cancel;

			public Tick LastTick { get; set; }
			public DateTime LastBestTime { get; set; }
			public decimal? LastBestPrice { get; set; }
			public Sides Side { get; }
			public decimal FilledQuantity { get; set; }

			public SpoofingState(Sides side)
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

					Logger.Debug("PushImpulseService canceling...");
					var task = TaskCompletionManager.CreateCompletableTask(this);
					_cancel.CancelEx();
					await task;
				}
				catch (Exception e)
				{
					Logger.Error("PushImpulseService.Cancel exception", e);
				}
			}
		}

		public ISpoofingState Pushing(Push push, Sides side)
		{
			if (!(push.TradeAccount?.Connector is IFixConnector)) return null;
			if (push.FeedAccount == null) return null;
			if (string.IsNullOrWhiteSpace(push.TradeSymbol)) return null;
			if (string.IsNullOrWhiteSpace(push.FeedSymbol)) return null;
			if (side == Sides.None) return null;
			if ((push.TriggerInMs ?? 0) == 0) return null;
			if (push.Size <= 0) return null;
			if (push.MaxOrders < push.Size) return null;

			var state = new SpoofingState(side);
			new Thread(() => Loop(push, state, state.Init())) { IsBackground = true }.Start();
			return state;
		}

		private void Loop(Push push, SpoofingState state, CancellationToken token)
		{
			var tradeConnector = (IFixConnector)push.TradeAccount.Connector;

			var waitHandle = new AutoResetEvent(false);
			void NewTick(object s, NewTick e)
			{
				if (e.Tick.Symbol != push.FeedSymbol) return;
				state.LastTick = e.Tick;
				waitHandle.Set();
			}

			state.LastTick = push.FeedAccount.GetLastTick(push.FeedSymbol);
			push.FeedAccount.NewTick += NewTick;
			if (state.LastTick?.HasValue == true)
				waitHandle.Set();

			var orders = new List<Task<OrderResponse>>();
			while (!token.IsCancellationRequested)
			{
				try
				{
					if (token.IsCancellationRequested) break;
					if (orders.Count >= push.MaxOrders) break;
					if (Trigger(push, state))
					{
						orders.Add(tradeConnector.SendMarketOrderRequest(push.TradeSymbol, state.Side, push.Size));
						Thread.Sleep(push.IntervalInMs);
					}
					else Thread.Sleep(1);
				}
				catch (Exception e)
				{
					Logger.Error("PushImpulseService.Loop exception", e);
				}
			}

			try
			{
				push.FeedAccount.NewTick -= NewTick;
				var results = Task.WhenAll(orders).Result;
				state.FilledQuantity = results.Sum(r => r.FilledQuantity);
				waitHandle.Dispose();
			}
			catch (Exception e)
			{
				Logger.Error("PushImpulseService.Loop exception", e);
			}
			finally
			{
				TaskCompletionManager.SetCompleted(state);
				Logger.Debug("PushImpulseService.Loop end");
			}
		}


		private bool Trigger(Push push, SpoofingState state)
		{
			if (!push.TriggerInMs.HasValue) return false;
			if (!state.LastBestPrice.HasValue) return false;

			var lastPrice = GetPrice(state);
			if (state.Side == Sides.Buy && lastPrice > state.LastBestPrice)
			{
				state.LastBestTime = state.LastTick.Time;
				state.LastBestPrice = lastPrice;
			}
			else if (state.Side == Sides.Sell && lastPrice < state.LastBestPrice)
			{
				state.LastBestTime = state.LastTick.Time;
				state.LastBestPrice = lastPrice;
			}

			return HiResDatetime.UtcNow - state.LastBestTime >= TimeSpan.FromMilliseconds(push.TriggerInMs.Value);
		}

		private static decimal GetPrice(SpoofingState state)
		{
			return state.Side == Sides.Buy ? state.LastTick.Bid : state.LastTick.Ask;
		}
	}
}

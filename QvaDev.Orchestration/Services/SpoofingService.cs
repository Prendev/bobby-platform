﻿using System;
using System.Threading;
using System.Threading.Tasks;
using QvaDev.Common;
using QvaDev.Common.Integration;
using QvaDev.Data;

namespace QvaDev.Orchestration.Services
{
	public interface ISpoofingService
	{
		ISpoofingState Spoofing(Spoof spoof, Sides side);
	}

	public class SpoofingService : ISpoofingService
	{
		private static readonly TaskCompletionManager<SpoofingState> TaskCompletionManager = new TaskCompletionManager<SpoofingState>(100, 1000);

		private class SpoofingState : ISpoofingState
		{
			private CancellationTokenSource _cancel;

			public LimitResponse LimitResponse { get; set; }

			public CancellationToken Init()
			{
				_cancel = new CancellationTokenSource();
				return _cancel.Token;
			}

			public async Task Cancel()
			{
				try
				{
					var task = TaskCompletionManager.CreateCompletableTask(this);
					_cancel.CancelAndDispose();
					await task;
				}
				catch (Exception e)
				{
					Logger.Error("SpoofingService.Cancel exception", e);
				}
			}
		}

		public ISpoofingState Spoofing(Spoof spoof, Sides side)
		{
			if (!(spoof.TradeAccount?.Connector is IFixConnector)) return null;
			if (spoof.FeedAccount == null) return null;
			if (string.IsNullOrWhiteSpace(spoof.TradeSymbol)) return null;
			if (string.IsNullOrWhiteSpace(spoof.FeedSymbol)) return null;
			if (side == Sides.None) return null;
			if (spoof.Size <= 0) return null;

			var state = new SpoofingState();
			new Thread(() => Loop(spoof, side, state, state.Init())) { IsBackground = true }.Start();
			return state;
		}

		private void Loop(Spoof spoof, Sides side, SpoofingState state, CancellationToken token)
		{
			var tradeConnector = (IFixConnector)spoof.TradeAccount.Connector;

			var lastTick = spoof.FeedAccount.GetLastTick(spoof.FeedSymbol);
			var waitHandle = new AutoResetEvent(false);

			void NewTick(object s, NewTick e)
			{
				if (token.IsCancellationRequested) return;
				if (e.Tick.Symbol != spoof.FeedSymbol) return;
				lastTick = e.Tick;
				waitHandle.Set();
			}

			var changed = false;
			spoof.FeedAccount.Connector.NewTick += NewTick;
			if (lastTick?.HasValue == true) waitHandle.Set();

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (!waitHandle.WaitOne(10)) continue;
					if (token.IsCancellationRequested) break;
					if (HiResDatetime.UtcNow - lastTick.Time > TimeSpan.FromSeconds(10)) continue;

					var price = GetPrice(spoof, side, lastTick);
					if (state.LimitResponse == null)
						state.LimitResponse = tradeConnector.SendSpoofOrderRequest(spoof.TradeSymbol, side, spoof.Size, price).Result;
					else changed = tradeConnector.ChangeLimitPrice(state.LimitResponse, price).Result;
				}
				catch (Exception e)
				{
					Logger.Error("SpoofingService.Loop exception", e);
				}
			}
			spoof.FeedAccount.Connector.NewTick -= NewTick;

			try
			{
				var canceled = tradeConnector.CancelLimit(state.LimitResponse).Result;
				waitHandle.Dispose();
			}
			catch (Exception e)
			{
				Logger.Error("SpoofingService.Loop exception", e);
			}
			finally
			{
				TaskCompletionManager.SetCompleted(state);
			}
		}

		private static decimal GetPrice(Spoof spoof, Sides side, Tick tick)
		{
			return side == Sides.Buy ? (tick.Bid - spoof.Distance) : (tick.Ask + spoof.Distance);
		}
	}
}

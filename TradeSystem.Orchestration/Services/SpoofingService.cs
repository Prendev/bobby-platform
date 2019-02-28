using System;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Data;

namespace TradeSystem.Orchestration.Services
{
	public interface ISpoofingService
	{
		ISpoofingState Spoofing(Spoof spoof, Sides side, CancellationTokenSource stop = null);
	}

	public class SpoofingService : ISpoofingService
	{
		private static readonly TaskCompletionManager<SpoofingState> TaskCompletionManager = new TaskCompletionManager<SpoofingState>(100, 1000);

		private class SpoofingState : ISpoofingState
		{
			private CancellationTokenSource _cancel;

			public Tick LastTick { get; set; }
			public DateTime LastBestTime { get; set; }
			public decimal? LastBestPrice { get; set; }
			public Sides Side { get; }
			public LimitResponse LimitResponse { get; set; }

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

		public ISpoofingState Spoofing(Spoof spoof, Sides side, CancellationTokenSource stop = null)
		{
			if (!(spoof.TradeAccount?.Connector is IFixConnector)) return null;
			if (spoof.FeedAccount == null) return null;
			if (string.IsNullOrWhiteSpace(spoof.TradeSymbol)) return null;
			if (string.IsNullOrWhiteSpace(spoof.FeedSymbol)) return null;
			if (side == Sides.None) return null;
			if (spoof.Size <= 0) return null;

			var state = new SpoofingState(side);
			new Thread(() => Loop(spoof, state, state.Init(), stop)) { IsBackground = true }.Start();
			return state;
		}

		private void Loop(Spoof spoof, SpoofingState state, CancellationToken token, CancellationTokenSource stop = null)
		{
			var tradeConnector = (IFixConnector)spoof.TradeAccount.Connector;
			var changed = false;

			state.LastTick = spoof.FeedAccount.GetLastTick(spoof.FeedSymbol);
			var waitHandle = new AutoResetEvent(false);

			void NewTick(object s, NewTick e)
			{
				if (e.Tick.Symbol != spoof.FeedSymbol) return;
				state.LastTick = e.Tick;
				waitHandle.Set();
			}

			spoof.FeedAccount.NewTick += NewTick;
			if (state.LastTick?.HasValue == true) waitHandle.Set();

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (!waitHandle.WaitOne(10)) continue;
					if (token.IsCancellationRequested) break;
					if (state.LimitResponse?.RemainingQuantity == 0) continue;
					MomentumStop(spoof, state, stop);
					if (HiResDatetime.UtcNow - state.LastTick.Time > TimeSpan.FromSeconds(10)) continue;

					var price = GetPrice(state, spoof.Distance);
					if (state.LimitResponse == null)
					{
						state.LimitResponse =
							tradeConnector.SendSpoofOrderRequest(spoof.TradeSymbol, state.Side, spoof.Size, price).Result;
						state.LastBestTime = HiResDatetime.UtcNow;
						state.LastBestPrice = GetPrice(state, 0);
					}
					else changed = tradeConnector.ChangeLimitPrice(state.LimitResponse, price).Result;
				}
				catch (Exception e)
				{
					Logger.Error("SpoofingService.Loop exception", e);
				}
			}

			try
			{
				spoof.FeedAccount.NewTick -= NewTick;

				if (spoof.PutAway.HasValue && state.LimitResponse?.RemainingQuantity > 0)
				{
					var price = GetPrice(state, spoof.PutAway.Value);
					changed = tradeConnector.ChangeLimitPrice(state.LimitResponse, price).Result;
				}
				else if (state.LimitResponse?.RemainingQuantity > 0)
				{
					var canceled = tradeConnector.CancelLimit(state.LimitResponse).Result;
				}

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

		private static decimal GetPrice(SpoofingState state, decimal distance)
		{
			return state.Side == Sides.Buy ? (state.LastTick.Bid - distance) : (state.LastTick.Ask + distance);
		}

		private void MomentumStop(Spoof spoof, SpoofingState state, CancellationTokenSource stop)
		{
			if (!spoof.MomentumStop.HasValue) return;
			if (stop == null) return;
			if (!state.LastBestPrice.HasValue) return;

			var lastPrice = GetPrice(state, 0);
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

			if (HiResDatetime.UtcNow - state.LastBestTime < TimeSpan.FromMilliseconds(spoof.MomentumStop.Value)) return;

			stop.CancelAndDispose();
		}
	}
}

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
	public interface ISpoofingService
	{
		ISpoofingState Spoofing(Spoof spoof, Sides side, CancellationTokenSource stop = null);
	}

	public class SpoofingService : ISpoofingService
	{
		private static readonly TaskCompletionManager<SpoofingState> TaskCompletionManager = new TaskCompletionManager<SpoofingState>(100, 2000);

		private class SpoofingState : ISpoofingState
		{
			private CancellationTokenSource _cancel;

			public Tick LastTick { get; set; }
			public DateTime LastBestTime { get; set; }
			public decimal? LastBestPrice { get; set; }
			public Sides Side { get; }
			public List<LimitResponse> LimitResponses { get; } = new List<LimitResponse>();
			public decimal FilledQuantity => LimitResponses.Sum(r => r.FilledQuantity);
			public decimal RemainingQuantity => LimitResponses.Sum(r => r.RemainingQuantity);

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

					Logger.Debug("SpoofingService canceling...");
					var task = TaskCompletionManager.CreateCompletableTask(this);
					_cancel.CancelEx();
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

			var waitHandle = new AutoResetEvent(false);
			void NewTick(object s, NewTick e)
			{
				if (e.Tick.Symbol != spoof.FeedSymbol) return;
				state.LastTick = e.Tick;
				waitHandle.Set();
			}

			state.LastTick = spoof.FeedAccount.GetLastTick(spoof.FeedSymbol);
			spoof.FeedAccount.NewTick += NewTick;
			if (state.LastTick?.HasValue == true) waitHandle.Set();

			var b = false;
			while (!token.IsCancellationRequested)
			{
				try
				{
					if (!waitHandle.WaitOne(10)) continue;
					if (token.IsCancellationRequested) break;
					if (state.LimitResponses.Any() && state.RemainingQuantity == 0) continue;
					MomentumStop(spoof, state, stop);
					if (state.LimitResponses.Any(r => r.RemainingQuantity == 0)) stop.CancelEx();
					if (HiResDatetime.UtcNow - state.LastTick.Time > TimeSpan.FromMinutes(1)) continue;

					if (!state.LimitResponses.Any())
					{
						for (var i = spoof.Levels - 1; i >= 0; i--)
						{
							var price = GetPrice(state, spoof.Distance, spoof.Step, i);
							state.LimitResponses.Add(tradeConnector.SendSpoofOrderRequest(spoof.TradeSymbol, state.Side, spoof.Size, price).Result);
						}
						state.LastBestTime = HiResDatetime.UtcNow;
						state.LastBestPrice = GetPrice(state);
					}
					else
					{
						var lr = state.LimitResponses.Where(r => r.RemainingQuantity > 0).ToList();
						var firstPrice = GetPrice(state, spoof.Distance, spoof.Step, 0);
						if (state.Side == Sides.Buy)
						{
							var shift = lr.Any(r => r.OrderPrice == firstPrice && state.LastTick.BidVolume <= r.RemainingQuantity) ? 1 : 0;
							var topLimit = lr.OrderByDescending(r => r.OrderPrice).First();
							if (topLimit.OrderPrice < firstPrice)
								for (var i = 0; i < lr.Count; i++)
								{
									var price = GetPrice(state, spoof.Distance, spoof.Step, i + shift);
									if (lr.Any(r => r.OrderPrice == price)) continue;
									var remaining = lr.Where(r => r.OrderPrice < price)
										.OrderBy(r => r.OrderPrice)
										.FirstOrDefault();
									if (remaining == null) break;
									b = tradeConnector.ChangeLimitPrice(remaining, price).Result;
								}
							else if(topLimit.OrderPrice > firstPrice)
								for (var i = lr.Count - 1; i >= 0; i--)
								{
									var price = GetPrice(state, spoof.Distance, spoof.Step, i + shift);
									if (lr.Any(r => r.OrderPrice == price)) continue;
									var remaining = lr.Where(r => r.OrderPrice > price)
										.OrderByDescending(r => r.OrderPrice)
										.FirstOrDefault();
									if (remaining == null) break;
									b = tradeConnector.ChangeLimitPrice(remaining, price).Result;
								}
						}
						else if (state.Side == Sides.Sell)
						{
							var shift = lr.Any(r => r.OrderPrice == firstPrice && state.LastTick.AskVolume <= r.RemainingQuantity) ? 1 : 0;
							var topLimit = lr.OrderBy(r => r.OrderPrice).First();
							if (topLimit.OrderPrice > firstPrice)
								for (var i = 0; i < lr.Count; i++)
								{
									var price = GetPrice(state, spoof.Distance, spoof.Step, i + shift);
									if (lr.Any(r => r.OrderPrice == price)) continue;
									var remaining = lr.Where(r => r.OrderPrice > price)
										.OrderByDescending(r => r.OrderPrice)
										.FirstOrDefault();
									if (remaining == null) break;
									b = tradeConnector.ChangeLimitPrice(remaining, price).Result;
								}
							else if (topLimit.OrderPrice < firstPrice)
								for (var i = lr.Count - 1; i >= 0; i--)
								{
									var price = GetPrice(state, spoof.Distance, spoof.Step, i + shift);
									if (lr.Any(r => r.OrderPrice == price)) continue;
									var remaining = lr.Where(r => r.OrderPrice < price)
										.OrderBy(r => r.OrderPrice)
										.FirstOrDefault();
									if (remaining == null) break;
									b = tradeConnector.ChangeLimitPrice(remaining, price).Result;
								}
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error("SpoofingService.Loop exception", e);
				}
			}

			try
			{
				spoof.FeedAccount.NewTick -= NewTick;

				if (state.Side == Sides.Buy && state.RemainingQuantity > 0)
				{
					foreach (var limitResponse in state.LimitResponses.OrderByDescending(r => r.OrderPrice))
						if (limitResponse.RemainingQuantity > 0)
							b = tradeConnector.CancelLimit(limitResponse).Result;
				}
				else if (state.Side == Sides.Sell && state.RemainingQuantity > 0)
				{
					foreach (var limitResponse in state.LimitResponses.OrderBy(r => r.OrderPrice))
						if (limitResponse.RemainingQuantity > 0)
							b = tradeConnector.CancelLimit(limitResponse).Result;
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
				Logger.Debug("SpoofingService.Loop end");
			}
		}

		private static decimal GetPrice(SpoofingState state)
		{
			return state.Side == Sides.Buy ? state.LastTick.Bid : state.LastTick.Ask;
		}
		private static decimal GetPrice(SpoofingState state, decimal distance, decimal step, int level)
		{
			var price = GetPrice(state);
			if (state.Side == Sides.Buy)
			{
				distance += level * step;
				price -= distance;
			}
			else
			{
				distance += level * step;
				price += distance;
			}
			return price;
		}

		private void MomentumStop(Spoof spoof, SpoofingState state, CancellationTokenSource stop)
		{
			if (!spoof.MomentumStop.HasValue) return;
			if (stop == null) return;
			if (!state.LastBestPrice.HasValue) return;

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

			if (HiResDatetime.UtcNow - state.LastBestTime < TimeSpan.FromMilliseconds(spoof.MomentumStop.Value)) return;

			stop.CancelEx();
		}
	}
}

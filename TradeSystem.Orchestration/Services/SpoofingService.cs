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
		IStratState Spoofing(Spoof spoof, Sides side, CancellationTokenSource stop = null);
	}

	public class SpoofingService : ISpoofingService
	{
		private static readonly TaskCompletionManager<StratState> TaskCompletionManager = new TaskCompletionManager<StratState>(100, 2000);

		private class StratState : IStratState
		{
			private CancellationTokenSource _cancel;

			public Tick LastTick { get; set; }
			public DateTime LastBestTime { get; set; }
			public decimal? LastBestPrice { get; set; }
			public Sides Side { get; }
			public List<LimitResponse> LimitResponses { get; } = new List<LimitResponse>();
			public decimal FilledQuantity => LimitResponses.Sum(r => r.FilledQuantity);
			public decimal RemainingQuantity => LimitResponses.Sum(r => r.RemainingQuantity);

			public event EventHandler<LimitFill> LimitFill;

			private volatile bool _isEnded;
			public bool IsEnded
			{
				get => _isEnded;
				set => _isEnded = value;
			}

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
					Logger.Debug("SpoofingService canceling...");
					if (IsEnded) return;
					var task = TaskCompletionManager.CreateCompletableTask(this);
					_cancel.CancelEx();
					await task;
				}
				catch (Exception e)
				{
					Logger.Error("SpoofingService.Cancel exception", e);
				}
			}

			public void OnLimitFill(LimitFill e) => LimitFill?.Invoke(this, e);
		}

		public IStratState Spoofing(Spoof spoof, Sides side, CancellationTokenSource stop = null)
		{
			if (!(spoof.TradeAccount?.Connector is IFixConnector)) return null;
			if (spoof.FeedAccount == null) return null;
			if (string.IsNullOrWhiteSpace(spoof.TradeSymbol)) return null;
			if (string.IsNullOrWhiteSpace(spoof.FeedSymbol)) return null;
			if (side == Sides.None) return null;
			if (spoof.Size <= 0) return null;

			var state = new StratState(side);
			new Thread(() => Loop(spoof, state, state.Init(), stop)) { IsBackground = true }.Start();
			return state;
		}

		private void Loop(Spoof spoof, StratState state, CancellationToken token, CancellationTokenSource stop = null)
		{
			var tradeConnector = (IFixConnector)spoof.TradeAccount.Connector;

			var waitHandle = new AutoResetEvent(false);
			void NewTick(object s, NewTick e)
			{
				if (e.Tick.Symbol != spoof.FeedSymbol) return;
				state.LastTick = e.Tick;
				waitHandle.Set();
			}

			void LimitFill(object sender, LimitFill limitFill)
			{
				if (!state.LimitResponses.Contains(limitFill.LimitResponse)) return;
				state.OnLimitFill(limitFill);
			}

			spoof.TradeAccount.LimitFill += LimitFill;

			state.LastTick = spoof.FeedAccount.GetLastTick(spoof.FeedSymbol);
			spoof.FeedAccount.NewTick += NewTick;
			if (state.LastTick?.HasValue == true)
				waitHandle.Set();

			var b = false;
			while (!token.IsCancellationRequested)
			{
				try
				{
					MomentumStop(spoof, state, stop);
					if (state.LimitResponses.Any(r => r.RemainingQuantity == 0))
					{
						stop.CancelEx();
						if (state.RemainingQuantity == 0) continue;
					}

					if (!waitHandle.WaitOne(10)) continue;
					if (token.IsCancellationRequested) break;

					var lastTick = state.LastTick;
					if (HiResDatetime.UtcNow - lastTick.Time > TimeSpan.FromMinutes(1)) continue;

					if (!state.LimitResponses.Any()) CreateLimits(spoof, state, tradeConnector);
					else CheckLimits(spoof, state, lastTick, tradeConnector);
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

				spoof.TradeAccount.LimitFill -= LimitFill;
				waitHandle.Dispose();
			}
			catch (Exception e)
			{
				Logger.Error("SpoofingService.Loop exception", e);
			}
			finally
			{
				state.IsEnded = true;
				TaskCompletionManager.SetCompleted(state);
				Logger.Debug("SpoofingService.Loop end");
			}
		}

		private static void CreateLimits(Spoof spoof, StratState state, IFixConnector tradeConnector)
		{
			for (var i = spoof.Levels - 1; i >= 0; i--)
			{
				var price = GetPrice(state, spoof.MaxDistance, spoof.Step, i);
				var lr = tradeConnector.PutNewOrderRequest(spoof.TradeSymbol, state.Side, spoof.Size, price).Result;
				if (lr == null) continue;
				state.LimitResponses.Add(lr);
			}

			if (state.LimitResponses.Any())
			{
				state.LastBestTime = HiResDatetime.UtcNow;
				state.LastBestPrice = GetPrice(state);
			}
		}

		private static void CheckLimits(Spoof spoof, StratState state, Tick lastTick, IFixConnector tradeConnector)
		{
			if (spoof.IsPulling) CheckPulling(spoof, state, tradeConnector);
			else CheckNormal(spoof, state, lastTick, tradeConnector);
		}

		private static void CheckPulling(Spoof spoof, StratState state, IFixConnector tradeConnector)
		{
			var limit = state.LimitResponses.FirstOrDefault(r => r.RemainingQuantity > 0);
			if (limit == null) return;

			var minPrice = GetPrice(state, spoof.MinDistance, spoof.Step, 0);
			var maxPrice = GetPrice(state, spoof.MaxDistance, spoof.Step, 0);
			if (CheckPrice(limit.OrderPrice, minPrice, maxPrice)) return;

			var b = tradeConnector.ChangeLimitPrice(limit, maxPrice).Result;
		}

		private static void CheckNormal(Spoof spoof, StratState state, Tick lastTick, IFixConnector tradeConnector)
		{
			bool b;
			var lr = state.LimitResponses.Where(r => r.RemainingQuantity > 0).ToList();
			var firstPrice = GetPrice(state, spoof.MaxDistance, spoof.Step, 0);
			if (state.Side == Sides.Buy)
			{
				var shift = lr.Any(r => r.OrderPrice == firstPrice && lastTick.BidVolume <= r.RemainingQuantity) ? 1 : 0;
				var topLimit = lr.OrderByDescending(r => r.OrderPrice).First();
				if (topLimit.OrderPrice < firstPrice)
					for (var i = 0; i < lr.Count; i++)
					{
						var price = GetPrice(state, spoof.MaxDistance, spoof.Step, i + shift);
						if (lr.Any(r => r.OrderPrice == price)) continue;
						var remaining = lr.Where(r => r.OrderPrice < price)
							.OrderBy(r => r.OrderPrice)
							.FirstOrDefault();
						if (remaining == null) break;
						b = tradeConnector.ChangeLimitPrice(remaining, price).Result;
					}
				else if (topLimit.OrderPrice > firstPrice)
					for (var i = lr.Count - 1; i >= 0; i--)
					{
						var price = GetPrice(state, spoof.MaxDistance, spoof.Step, i + shift);
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
				var shift = lr.Any(r => r.OrderPrice == firstPrice && lastTick.AskVolume <= r.RemainingQuantity) ? 1 : 0;
				var topLimit = lr.OrderBy(r => r.OrderPrice).First();
				if (topLimit.OrderPrice > firstPrice)
					for (var i = 0; i < lr.Count; i++)
					{
						var price = GetPrice(state, spoof.MaxDistance, spoof.Step, i + shift);
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
						var price = GetPrice(state, spoof.MaxDistance, spoof.Step, i + shift);
						if (lr.Any(r => r.OrderPrice == price)) continue;
						var remaining = lr.Where(r => r.OrderPrice < price)
							.OrderBy(r => r.OrderPrice)
							.FirstOrDefault();
						if (remaining == null) break;
						b = tradeConnector.ChangeLimitPrice(remaining, price).Result;
					}
			}
		}

		private static decimal GetPrice(StratState state)
		{
			return state.Side == Sides.Buy ? state.LastTick.Bid : state.LastTick.Ask;
		}
		private static decimal GetPrice(StratState state, decimal distance, decimal step, int level)
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

		private void MomentumStop(Spoof spoof, StratState state, CancellationTokenSource stop)
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

		private static bool CheckPrice(decimal price, decimal minPrice, decimal maxPrice)
		{
			if (price < Math.Min(minPrice, maxPrice)) return false;
			if (price > Math.Max(minPrice, maxPrice)) return false;
			return true;
		}
	}
}

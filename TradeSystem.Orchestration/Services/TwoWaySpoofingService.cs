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
	public interface ITwoWaySpoofingService
	{
		IStratState Spoofing(TwoWaySpoof twoWay, Sides spoofSide);
	}

	public class TwoWaySpoofingService : ITwoWaySpoofingService
	{
		private static readonly TaskCompletionManager<StratState> TaskCompletionManager = new TaskCompletionManager<StratState>(100, 5000);

		private class StratState : IStratState
		{
			private CancellationTokenSource _cancel;

			public Tick LastTick { get; set; }
			public DateTime LastBestTime { get; set; }
			public decimal? LastBestPrice { get; set; }
			public Sides Side { get; }

			public List<LimitResponse> SpoofLimitResponses { get; } = new List<LimitResponse>();
			public decimal FilledQuantity => SpoofLimitResponses.Sum(r => r.FilledQuantity);
			public decimal RemainingQuantity => SpoofLimitResponses.Sum(r => r.RemainingQuantity);

			public LimitResponse PullLimitResponse { get; set; }

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

		public IStratState Spoofing(TwoWaySpoof twoWay, Sides spoofSide)
		{
			if (!(twoWay.TradeAccount?.Connector is IFixConnector)) return null;
			if (twoWay.FeedAccount == null) return null;
			if (string.IsNullOrWhiteSpace(twoWay.TradeSymbol)) return null;
			if (string.IsNullOrWhiteSpace(twoWay.FeedSymbol)) return null;
			if (spoofSide == Sides.None) return null;
			if (twoWay.SpoofSize <= 0 && twoWay.PullSize <= 0) return null;

			var state = new StratState(spoofSide);
			new Thread(() => Loop(twoWay, state, state.Init())) { IsBackground = true }.Start();
			return state;
		}

		private void Loop(TwoWaySpoof spoof, StratState state, CancellationToken token)
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
				if (!state.SpoofLimitResponses.Contains(limitFill.LimitResponse)) return;
				state.OnLimitFill(limitFill);
			}

			spoof.TradeAccount.LimitFill += LimitFill;

			state.LastTick = spoof.FeedAccount.GetLastTick(spoof.FeedSymbol);
			spoof.FeedAccount.NewTick += NewTick;
			if (state.LastTick?.HasValue == true)
				waitHandle.Set();

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (!waitHandle.WaitOne(10)) continue;
					if (token.IsCancellationRequested) break;
					if (state.SpoofLimitResponses.Any() && state.RemainingQuantity == 0) continue;
					if (HiResDatetime.UtcNow - state.LastTick.Time > TimeSpan.FromMinutes(1)) continue;

					if (!state.SpoofLimitResponses.Any()) CreateSpoofLimits(spoof, state, tradeConnector);
					else CheckSpoof(spoof, state, tradeConnector);

					if (state.PullLimitResponse == null) CreatePullLimit(spoof, state, tradeConnector);
					else CheckPulling(spoof, state, tradeConnector);
				}
				catch (Exception e)
				{
					Logger.Error("SpoofingService.Loop exception", e);
				}
			}

			try
			{
				spoof.FeedAccount.NewTick -= NewTick;

				var tasks = new List<Task>();
				// Cancel spoof limits
				var lr = state.Side == Sides.Buy
					? state.SpoofLimitResponses.OrderByDescending(r => r.OrderPrice)
					: state.SpoofLimitResponses.OrderBy(r => r.OrderPrice);
				foreach (var limitResponse in lr) tasks.Add(tradeConnector.CancelLimit(limitResponse));

				// Cancel pull limit
				if (state.PullLimitResponse != null) tasks.Add(tradeConnector.CancelLimit(state.PullLimitResponse));

				if (tasks.Any()) Task.WhenAll(tasks).Wait();

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

		private static void CreateSpoofLimits(TwoWaySpoof spoof, StratState state, IFixConnector tradeConnector)
		{
			if (spoof.SpoofSize <= 0) return;
			if (spoof.SpoofLevels <= 0) return;
			if (spoof.SpoofInitDistanceInTick < 0) return;
			if (spoof.SpoofFollowDistanceInTick < 0) return;

			var tasks = new List<Task<LimitResponse>>();
			for (var i = spoof.SpoofLevels - 1; i >= 0; i--)
			{
				var price = GetPrice(state, state.Side, spoof.SpoofInitDistanceInTick, spoof.TickSize, i);
				tasks.Add(tradeConnector.SendSpoofOrderRequest(spoof.TradeSymbol, state.Side, spoof.SpoofSize, price));
			}

			Task.WhenAll(tasks).Wait();
			state.SpoofLimitResponses.AddRange(tasks.Where(t => t.Result != null).Select(t => t.Result));

			if (state.SpoofLimitResponses.Any())
			{
				state.LastBestTime = HiResDatetime.UtcNow;
				state.LastBestPrice = GetPrice(state, state.Side);
			}
		}

		private static void CreatePullLimit(TwoWaySpoof spoof, StratState state, IFixConnector tradeConnector)
		{
			if (spoof.PullSize <= 0) return;
			if (spoof.PullMaxDistanceInTick < 0) return;
			if (spoof.PullMaxDistanceInTick <= 0) return;

			var price = GetPrice(state, state.Side.Inv(), spoof.PullMaxDistanceInTick, spoof.TickSize, 0);
			state.PullLimitResponse = tradeConnector.SendSpoofOrderRequest(spoof.TradeSymbol, state.Side.Inv(), spoof.PullSize, price).Result;
		}

		private static void CheckPulling(TwoWaySpoof spoof, StratState state, IFixConnector tradeConnector)
		{
			// Check if there is pull with pending quantity
			if (state.PullLimitResponse == null) return;
			if (state.PullLimitResponse.RemainingQuantity == 0) return;

			// Check if current price is in proper region
			var minPrice = GetPrice(state, state.Side.Inv(), spoof.PullMinDistanceInTick, spoof.TickSize, 0);
			var maxPrice = GetPrice(state, state.Side.Inv(), spoof.PullMaxDistanceInTick, spoof.TickSize, 0);
			if (state.PullLimitResponse.OrderPrice >= Math.Min(minPrice, maxPrice) &&
			    state.PullLimitResponse.OrderPrice <= Math.Max(minPrice, maxPrice)) return;

			// Modify price the max distance
			tradeConnector.ChangeLimitPrice(state.PullLimitResponse, maxPrice).Wait();
		}

		private static void CheckSpoof(TwoWaySpoof spoof, StratState state, IFixConnector tradeConnector)
		{
			// Check if there is any limit left with pending quantity
			var lr = state.SpoofLimitResponses.Where(r => r.RemainingQuantity > 0).ToList();
			if (!lr.Any()) return;

			var firstPrice = GetPrice(state, state.Side, spoof.SpoofFollowDistanceInTick, spoof.TickSize, 0);

			if (state.Side == Sides.Buy)
			{
				//var shift = lr.Any(r => r.OrderPrice == firstPrice && lastTick.BidVolume + spoof.SpoofSafetyVolumeDiff <= r.RemainingQuantity) ? 1 : 0;
				var topLimit = lr.OrderByDescending(r => r.OrderPrice).First();
				if (topLimit.OrderPrice < firstPrice)
					for (var i = 0; i < lr.Count; i++)
					{
						var price = GetPrice(state, state.Side, spoof.SpoofFollowDistanceInTick, spoof.TickSize, i);
						if (lr.Any(r => r.OrderPrice == price)) continue;
						var remaining = lr.Where(r => r.OrderPrice < price)
							.OrderBy(r => r.OrderPrice)
							.FirstOrDefault();
						if (remaining == null) break;
						tradeConnector.ChangeLimitPrice(remaining, price).Wait();
					}
				else if (topLimit.OrderPrice > firstPrice)
					for (var i = lr.Count - 1; i >= 0; i--)
					{
						var price = GetPrice(state, state.Side, spoof.SpoofFollowDistanceInTick, spoof.TickSize, i);
						if (lr.Any(r => r.OrderPrice == price)) continue;
						var remaining = lr.Where(r => r.OrderPrice > price)
							.OrderByDescending(r => r.OrderPrice)
							.FirstOrDefault();
						if (remaining == null) break;
						tradeConnector.ChangeLimitPrice(remaining, price).Wait();
					}
			}

			else if (state.Side == Sides.Sell)
			{
				//var shift = lr.Any(r => r.OrderPrice == firstPrice && lastTick.AskVolume + spoof.SpoofSafetyVolumeDiff <= r.RemainingQuantity) ? 1 : 0;
				var topLimit = lr.OrderBy(r => r.OrderPrice).First();
				if (topLimit.OrderPrice > firstPrice)
					for (var i = 0; i < lr.Count; i++)
					{
						var price = GetPrice(state, state.Side, spoof.SpoofFollowDistanceInTick, spoof.TickSize, i);
						if (lr.Any(r => r.OrderPrice == price)) continue;
						var remaining = lr.Where(r => r.OrderPrice > price)
							.OrderByDescending(r => r.OrderPrice)
							.FirstOrDefault();
						if (remaining == null) break;
						tradeConnector.ChangeLimitPrice(remaining, price).Wait();
					}
				else if (topLimit.OrderPrice < firstPrice)
					for (var i = lr.Count - 1; i >= 0; i--)
					{
						var price = GetPrice(state, state.Side, spoof.SpoofFollowDistanceInTick, spoof.TickSize, i);
						if (lr.Any(r => r.OrderPrice == price)) continue;
						var remaining = lr.Where(r => r.OrderPrice < price)
							.OrderBy(r => r.OrderPrice)
							.FirstOrDefault();
						if (remaining == null) break;
						tradeConnector.ChangeLimitPrice(remaining, price).Wait();
					}
			}
		}

		private static decimal GetPrice(StratState state, Sides side)
		{
			return side == Sides.Buy ? state.LastTick.Bid : state.LastTick.Ask;
		}
		private static decimal GetPrice(StratState state, Sides side, decimal firstDistance, decimal tickSize, int level)
		{
			firstDistance *= tickSize;
			var price = GetPrice(state, side);
			if (side == Sides.Buy)
			{
				firstDistance += level * tickSize;
				price -= firstDistance;
			}
			else
			{
				firstDistance += level * tickSize;
				price += firstDistance;
			}
			return price;
		}
	}
}

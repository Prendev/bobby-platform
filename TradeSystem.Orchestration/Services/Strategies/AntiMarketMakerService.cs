using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Collections;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface IAntiMarketMakerService
	{
		void Start(List<MarketMaker> sets);
		void Stop();
	}

	public class AntiMarketMakerService : IAntiMarketMakerService
	{

		private volatile CancellationTokenSource _cancellation;

		private List<MarketMaker> _sets;
		private readonly ConcurrentDictionary<int, FastBlockingCollection<Action>> _queues =
			new ConcurrentDictionary<int, FastBlockingCollection<Action>>();

		public void Start(List<MarketMaker> sets)
		{
			_cancellation?.Dispose();

			_sets = sets;
			_cancellation = new CancellationTokenSource();

			foreach (var set in _sets)
			{
				if (!set.Run) continue;
				new Thread(() => SetLoop(set, _cancellation.Token)) { Name = $"AntiMarketMaker_{set.Id}", IsBackground = true }
					.Start();
			}

			Logger.Info("Anti market makers are started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("Anti market makers are stopped");
		}

		private void SetLoop(MarketMaker set, CancellationToken token)
		{
			set.FeedNewTick -= Set_FeedNewTick;
			set.FeedNewTick += Set_FeedNewTick;

			var queue = _queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>());
			set.FeedAccount?.Connector.Subscribe(set.FeedSymbol);

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (set.State == MarketMaker.MarketMakerStates.None)
					{
						Thread.Sleep(1);
						continue;
					}
					if (set.State == MarketMaker.MarketMakerStates.Init) InitBases(set);

					var action = queue.Take(token);
					action();
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("MarketMakerService.Loop exception", e);
				}
			}

			set.FeedNewTick -= Set_FeedNewTick;
			_queues.TryRemove(set.Id, out queue);

			set.State = MarketMaker.MarketMakerStates.None;
			set.NextBottomDepth = 0;
			set.NextTopDepth = 0;
		}

		private void InitBases(MarketMaker set)
		{
			var lastTick = set.FeedAccount.GetLastTick(set.FeedSymbol);
			if (lastTick?.HasValue != true)
			{
				Thread.Sleep(1);
				return;
			}

			var bid = set.InitBidPrice.HasValue ? set.InitBidPrice.Value : lastTick.Bid;
			var ask = set.InitBidPrice.HasValue ? (set.InitBidPrice.Value + set.TickSize) : lastTick.Ask;
			set.BottomBase = bid - set.InitialDistanceInTick * set.TickSize;
			set.TopBase = ask + set.InitialDistanceInTick * set.TickSize;

			set.State = MarketMaker.MarketMakerStates.Trade;
		}

		private void Set_FeedNewTick(object sender, NewTick newTick)
		{
			if (_cancellation.IsCancellationRequested) return;
			var set = (MarketMaker)sender;

			if (!set.Run) return;
			if (set.State != MarketMaker.MarketMakerStates.Trade) return;
			if (!newTick.Tick.HasValue) return;

			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => Check(set));
		}

		private void Check(MarketMaker set)
		{
			lock (set)
			{
				//CheckClose(set, tick);
				CheckOpen(set);
			}
		}

		/*private void CheckClose(MarketMaker set, Tick tick)
		{
			foreach (var level in set.AntiLevels.OrderByDescending(l => l.Key).ToList())
			{
				CheckLongClose(set, tick, level.Key);
				CheckShortClose(set, tick, level.Key);
			}
		}
		private void CheckLongClose(MarketMaker set, Tick tick, int d)
		{
			if (!set.TopBase.HasValue) return;
			var stop = set.TopBase.Value + d * set.LimitGapsInTick * set.TickSize - set.TpOrSlInTick * set.TickSize;
			if (tick.Bid > stop - set.TpOrSlInTick * set.TickSize) return;
			if (tick.Bid == stop && set.DomTrigger > 0 && tick.BidVolume > set.DomTrigger) return;

			var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());
			if (!level.LongFilled) return;
			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => LongClose(set, level, stop));
		}
		private async void LongClose(MarketMaker set, MarketMaker.Level level, decimal stop)
		{
			try
			{
				if (!level.LongFilled) return;
				if (level.LongCloseLimitResponse?.FilledQuantity > 0)
				{
					level.LongCloseLimitResponse = null;
					level.LongOpenLimitResponse = null;
					level.LongOpenOrderResponse = null;
				}

				var lastTick = set.FeedAccount.GetLastTick(set.FeedSymbol);
				var price = lastTick.Bid;
				if (price > stop) return;
				if (price == stop && set.DomTrigger > 0 && lastTick.BidVolume > set.DomTrigger) return;
				var connector = (FixApiConnectorBase)set.TradeAccount.Connector;

				if (price > stop - set.MarketThresholdInTick * set.TickSize)
				{
					if (level.LongCloseLimitResponse != null) return;
					level.LongCloseLimitResponse = await connector.PutNewOrderRequest(set.TradeSymbol, Sides.Sell, set.ContractSize, stop);
				}
				// Switch to market
				else
				{
					if (level.LongCloseLimitResponse?.FilledQuantity > 0) return;
					if (level.LongCloseLimitResponse != null && !(await connector.CancelLimit(level.LongCloseLimitResponse))) return;
					level.LongCloseLimitResponse = null;

					var orderResponse = await connector.SendMarketOrderRequest(set.TradeSymbol, Sides.Sell, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					level.LongOpenLimitResponse = null;
					level.LongOpenOrderResponse = null;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.LongClose exception", e);
			}
		}
		private void CheckShortClose(MarketMaker set, Tick tick, int d)
		{
			if (!set.BottomBase.HasValue) return;
			var stop = set.BottomBase.Value - d * set.LimitGapsInTick * set.TickSize + set.TpOrSlInTick * set.TickSize;
			if (tick.Ask < stop - set.TpOrSlInTick * set.TickSize) return;
			if (tick.Ask == stop && set.DomTrigger > 0 && tick.AskVolume > set.DomTrigger) return;

			var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());
			if (!level.ShortFilled) return;
			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => ShortClose(set, level, stop));
		}
		private async void ShortClose(MarketMaker set, MarketMaker.Level level, decimal stop)
		{
			try
			{
				if (!level.ShortFilled) return;
				if (level.ShortCloseLimitResponse?.FilledQuantity > 0)
				{
					level.ShortCloseLimitResponse = null;
					level.ShortOpenLimitResponse = null;
					level.ShortOpenOrderResponse = null;
				}

				var lastTick = set.FeedAccount.GetLastTick(set.FeedSymbol);
				var price = lastTick.Ask;
				if (price < stop) return;
				if (price == stop && set.DomTrigger > 0 && lastTick.AskVolume > set.DomTrigger) return;
				var connector = (FixApiConnectorBase)set.TradeAccount.Connector;

				if (price < stop + set.MarketThresholdInTick * set.TickSize)
				{
					if (level.ShortCloseLimitResponse != null) return;
					level.ShortCloseLimitResponse = await connector.PutNewOrderRequest(set.TradeSymbol, Sides.Buy, set.ContractSize, stop);
				}
				// Switch to market
				else
				{
					if (level.ShortCloseLimitResponse?.FilledQuantity > 0) return;
					if (level.ShortCloseLimitResponse != null && !(await connector.CancelLimit(level.ShortCloseLimitResponse))) return;
					level.ShortCloseLimitResponse = null;

					var orderResponse = await connector.SendMarketOrderRequest(set.TradeSymbol, Sides.Buy, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					level.ShortOpenLimitResponse = null;
					level.ShortOpenOrderResponse = null;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.ShortClose exception", e);
			}
		}*/

		private void CheckOpen(MarketMaker set)
		{
			var d = set.NextTopDepth;
			while (set.MaxDepth <= 0 || d < set.MaxDepth)
			{
				if (_cancellation.IsCancellationRequested) return;
				if (!set.TopBase.HasValue) break;
				var limit = set.TopBase.Value + d * set.LimitGapsInTick * set.TickSize;
				var lastTick = set.FeedAccount.GetLastTick(set.FeedSymbol);
				if (lastTick.Ask < limit) break;
				if (lastTick.Ask == limit && set.DomTrigger > 0 && lastTick.AskVolume > set.DomTrigger) break;

				CheckLongOpen(set, d).Wait();
				d++;
			}
			d = set.NextBottomDepth;
			while (set.MaxDepth <= 0 || d < set.MaxDepth)
			{
				if (_cancellation.IsCancellationRequested) return;
				if (!set.BottomBase.HasValue) break;
				var limit = set.BottomBase.Value - d * set.LimitGapsInTick * set.TickSize;
				var lastTick = set.FeedAccount.GetLastTick(set.FeedSymbol);
				if (lastTick.Bid > limit) break;
				if (lastTick.Bid == limit && set.DomTrigger > 0 && lastTick.BidVolume > set.DomTrigger) break;

				CheckShortOpen(set, d).Wait();
				d++;
			}
		}
		private async Task CheckLongOpen(MarketMaker set, int d)
		{
			try
			{
				var limit = set.TopBase.Value + d * set.LimitGapsInTick * set.TickSize;
				var lastTick = set.FeedAccount.GetLastTick(set.FeedSymbol);
				var connector = (FixApiConnectorBase) set.TradeAccount.Connector;
				var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());

				if (lastTick.Ask < limit + set.MarketThresholdInTick * set.TickSize)
				{
					if (level.LongOpenLimitResponse != null)
					{
						if (level.LongOpenLimitResponse.RemainingQuantity != 0) return;
						set.NextTopDepth++;
						level.LongOpenLimitResponse = null;
					}
					else level.LongOpenLimitResponse = await connector.PutNewOrderRequest(set.TradeSymbol, Sides.Buy, set.ContractSize, limit);
				}
				// Switch to market
				else
				{
					if (level.LongOpenLimitResponse != null)
					{
						if (level.LongOpenLimitResponse.RemainingQuantity == 0)
						{
							set.NextTopDepth++;
							level.LongOpenLimitResponse = null;
							return;
						}

						if (!(await connector.CancelLimit(level.LongOpenLimitResponse))) return;
						if (level.LongOpenLimitResponse.FilledQuantity != 0)
						{
							set.NextTopDepth++;
							level.LongOpenLimitResponse = null;
							return;
						}
						level.LongOpenLimitResponse = null;
					}
					var orderResponse = await connector.SendMarketOrderRequest(set.TradeSymbol, Sides.Buy, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					set.NextTopDepth++;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.LongOpen exception", e);
			}
		}
		private async Task CheckShortOpen(MarketMaker set, int d)
		{
			try
			{
				var limit = set.BottomBase.Value - d * set.LimitGapsInTick * set.TickSize;
				var lastTick = set.FeedAccount.GetLastTick(set.FeedSymbol);
				var connector = (FixApiConnectorBase)set.TradeAccount.Connector;
				var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());

				if (lastTick.Bid > limit - set.MarketThresholdInTick * set.TickSize)
				{
					if (level.ShortOpenLimitResponse != null)
					{
						if (level.ShortOpenLimitResponse.RemainingQuantity != 0) return;
						set.NextBottomDepth++;
						level.ShortOpenLimitResponse = null;
					}
					else level.ShortOpenLimitResponse = await connector.PutNewOrderRequest(set.TradeSymbol, Sides.Sell, set.ContractSize, limit);
				}
				// Switch to market
				else
				{
					if (level.ShortOpenLimitResponse != null)
					{
						if (level.ShortOpenLimitResponse.RemainingQuantity == 0)
						{
							set.NextBottomDepth++;
							level.ShortOpenLimitResponse = null;
							return;
						}

						if (!(await connector.CancelLimit(level.ShortOpenLimitResponse))) return;
						if (level.ShortOpenLimitResponse.FilledQuantity != 0)
						{
							set.NextBottomDepth++;
							level.ShortOpenLimitResponse = null;
							return;
						}
						level.ShortOpenLimitResponse = null;
					}
					var orderResponse = await connector.SendMarketOrderRequest(set.TradeSymbol, Sides.Sell, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					set.NextBottomDepth++;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.ShortOpen exception", e);
			}
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

			CheckClose(set, newTick.Tick);
			CheckOpen(set, newTick.Tick);
		}

		private void CheckClose(MarketMaker set, Tick tick)
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
		}


		private void CheckOpen(MarketMaker set, Tick tick)
		{
			var d = 0;
			while (set.MaxDepth <= 0 || d < set.MaxDepth)
				if (CheckLongOpen(set, tick, d)) break;
			while (set.MaxDepth <= 0 || d < set.MaxDepth)
				if (CheckShortOpen(set, tick, d)) break;
		}
		private bool CheckLongOpen(MarketMaker set, Tick tick, int d)
		{
			if (!set.TopBase.HasValue) return true;
			var price = set.TopBase.Value + d * set.LimitGapsInTick * set.TickSize;
			if (tick.Ask < price) return true;
			if (tick.Ask == price && set.DomTrigger > 0 && tick.AskVolume > set.DomTrigger) return true;

			var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());
			if (level.LongOpenOrderResponse?.IsFilled == true) return false;
			if (level.LongOpenLimitResponse?.FilledQuantity > 0) return false;

			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => LongOpen(set, level, price));
			return false;
		}
		private async void LongOpen(MarketMaker set, MarketMaker.Level level, decimal limit)
		{
			try
			{
				var lastTick = set.FeedAccount.GetLastTick(set.FeedSymbol);
				var price = lastTick.Ask;
				if (price < limit) return;
				if (price == limit && set.DomTrigger > 0 && lastTick.AskVolume > set.DomTrigger) return;
				var connector = (FixApiConnectorBase) set.TradeAccount.Connector;

				if (price < limit + set.MarketThresholdInTick * set.TickSize)
				{
					if (level.LongOpenLimitResponse != null) return;
					if (level.LongOpenOrderResponse != null) return;
					level.LongOpenLimitResponse = await connector.PutNewOrderRequest(set.TradeSymbol, Sides.Buy, set.ContractSize, limit);
				}
				// Switch to market
				else
				{
					if (level.LongOpenOrderResponse != null) return;
					if (level.LongOpenLimitResponse != null && level.LongOpenLimitResponse.FilledQuantity != 0) return;
					if (level.LongOpenLimitResponse != null && !(await connector.CancelLimit(level.LongOpenLimitResponse))) return;
					level.LongOpenLimitResponse = null;

					var orderResponse = await connector.SendMarketOrderRequest(set.TradeSymbol, Sides.Buy, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					level.LongOpenOrderResponse = orderResponse;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.LongOpen exception", e);
			}
		}

		private bool CheckShortOpen(MarketMaker set, Tick tick, int d)
		{
			if (!set.BottomBase.HasValue) return true;
			var price = set.BottomBase.Value - d * set.LimitGapsInTick * set.TickSize;
			if (tick.Bid > price) return true;
			if (tick.Bid == price && set.DomTrigger > 0 && tick.BidVolume > set.DomTrigger) return true;

			var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());
			if (level.ShortOpenOrderResponse?.IsFilled == true) return false;
			if (level.ShortOpenLimitResponse?.FilledQuantity > 0) return false;

			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => ShortOpen(set, level, price));
			return false;
		}
		private async void ShortOpen(MarketMaker set, MarketMaker.Level level, decimal limit)
		{
			try
			{
				var lastTick = set.FeedAccount.GetLastTick(set.FeedSymbol);
				var price = lastTick.Bid;
				if (price < limit) return;
				if (price == limit && set.DomTrigger > 0 && lastTick.BidVolume > set.DomTrigger) return;
				var connector = (FixApiConnectorBase)set.TradeAccount.Connector;

				if (price > limit - set.MarketThresholdInTick * set.TickSize)
				{
					if (level.ShortOpenLimitResponse != null) return;
					if (level.ShortOpenOrderResponse != null) return;
					level.ShortOpenLimitResponse = await connector.PutNewOrderRequest(set.TradeSymbol, Sides.Sell, set.ContractSize, limit);
				}
				// Switch to market
				else
				{
					if (level.ShortOpenOrderResponse != null) return;
					if (level.ShortOpenLimitResponse != null && level.ShortOpenLimitResponse.FilledQuantity != 0) return;
					if (level.ShortOpenLimitResponse != null && !(await connector.CancelLimit(level.ShortOpenLimitResponse))) return;
					level.ShortOpenLimitResponse = null;

					var orderResponse = await connector.SendMarketOrderRequest(set.TradeSymbol, Sides.Sell, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					level.ShortOpenOrderResponse = orderResponse;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.ShortOpen exception", e);
			}
		}
	}
}

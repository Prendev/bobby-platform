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

		private readonly IStopOrderService _stopOrderService;

		public AntiMarketMakerService(IStopOrderService stopOrderService)
		{
			_stopOrderService = stopOrderService;
			_stopOrderService.Fill += _stopOrderService_Fill;
		}

		public void Start(List<MarketMaker> sets)
		{
			_cancellation?.Dispose();

			_sets = sets;
			_cancellation = new CancellationTokenSource();

			foreach (var set in _sets)
			{
				if (!set.Run) continue;
				set.Token = _cancellation.Token;
				new Thread(() => SetLoop(set)) { Name = $"AntiMarketMaker_{set.Id}", IsBackground = true }
					.Start();
			}

			Logger.Info("Anti market makers are started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			_stopOrderService.Stop();
			Logger.Info("Anti market makers are stopped");
		}

		private void SetLoop(MarketMaker set)
		{
			set.NewTick -= Set_NewTick;
			var queue = _queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>());

			set.NewTick += Set_NewTick;
			set.Account.Connector.Subscribe(set.Symbol);

			while (!set.Token.IsCancellationRequested)
			{
				try
				{
					var action = queue.Take(set.Token);
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

			set.NewTick -= Set_NewTick;
			_queues.TryRemove(set.Id, out queue);

			set.State = MarketMaker.MarketMakerStates.None;
		}

		private void Set_NewTick(object sender, NewTick newTick)
		{
			if (!(sender is MarketMaker set)) return;
			if (set.Token.IsCancellationRequested) return;
			CheckStop(set);
			CheckInit(set);
		}

		private void CheckStop(MarketMaker set)
		{
			if (!set.Run || set.State == MarketMaker.MarketMakerStates.None)
				_stopOrderService.ClearLimits(set);
		}

		private void CheckInit(MarketMaker set)
		{
			if (!set.Run) return;
			if (set.State != MarketMaker.MarketMakerStates.Init && set.State != MarketMaker.MarketMakerStates.Continue) return;

			lock (set)
			{
				if (set.IsBusy) return;
				set.IsBusy = true;
			}

			if (set.State == MarketMaker.MarketMakerStates.Init)
			{
				set.InitBidPrice = null;
				set.NextTopDepth = 0;
				set.NextBottomDepth = 0;
			}
			set.State = MarketMaker.MarketMakerStates.PreTrade;
			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => InitLimits(set));
		}

		private void InitLimits(MarketMaker set)
		{
			_stopOrderService.ClearLimits(set);
			_stopOrderService.Start(set);

			set.InitBidPrice = set.InitBidPrice ?? set.Account.GetLastTick(set.Symbol).Bid;
			set.BottomBase = set.InitBidPrice - set.InitialDistanceInTick * set.TickSize;
			set.TopBase = set.InitBidPrice + set.InitialDistanceInTick * set.TickSize;
			set.AntiMarketMakerTopStops.Clear();
			set.AntiMarketMakerBottomStops.Clear();

			var stop = set.TpOrSlInTick * set.TickSize;
			var gap = set.LimitGapsInTick * set.TickSize;
			var agg = set.AggressiveThresholdInTick * set.TickSize;

			for (var d = set.NextTopDepth - 1; d > 0; d--)
			{
				var buyStop = _stopOrderService.SendStopOrder(set, Sides.Sell, set.TopBase.Value + d * gap - stop,
					set.TopBase.Value + d * gap - stop - agg, $"{d:0000}_Top");
				set.AntiMarketMakerTopStops.AddOrUpdate(buyStop.UserId, buyStop, (s, o) => buyStop);
			}
			var buy = _stopOrderService.SendStopOrder(set, Sides.Buy, set.TopBase.Value + set.NextTopDepth * gap,
				set.TopBase.Value + set.NextTopDepth * gap + agg, $"{set.NextTopDepth:0000}_Top");
			set.AntiMarketMakerTopStops.AddOrUpdate(buy.UserId, buy, (s, o) => buy);

			for (var d = set.NextBottomDepth - 1; d > 0; d--)
			{
				var sellStop = _stopOrderService.SendStopOrder(set, Sides.Buy, set.BottomBase.Value - d * gap + stop,
					set.BottomBase.Value - d * gap + stop + agg, $"{d:0000}_Bottom");
				set.AntiMarketMakerBottomStops.AddOrUpdate(sellStop.UserId, sellStop, (s, o) => sellStop);
			}
			var sell = _stopOrderService.SendStopOrder(set, Sides.Sell, set.BottomBase.Value - set.NextBottomDepth * gap,
				set.BottomBase.Value - set.NextBottomDepth * gap - agg, $"{set.NextBottomDepth:0000}_Bottom");
			set.AntiMarketMakerBottomStops.AddOrUpdate(sell.UserId, sell, (s, o) => sell);

			set.State = MarketMaker.MarketMakerStates.Trade;
			set.IsBusy = false;
		}

		private void _stopOrderService_Fill(object sender, StopResponse e)
		{
			if (!(sender is MarketMaker set)) return;
			if (set.Token.IsCancellationRequested) return;
			if (!set.Run) return;
			if (set.State == MarketMaker.MarketMakerStates.None) return;

			if (set.AntiMarketMakerTopStops.ContainsKey(e.UserId))
				_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => PostFillTop(set, e, Int32.Parse(e.UserId.Split('_').First())));
			else if (set.AntiMarketMakerBottomStops.ContainsKey(e.UserId))
				_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => PostFillBottom(set, e, Int32.Parse(e.UserId.Split('_').First())));

		}

		private void PostFillTop(MarketMaker set, StopResponse response, int depth)
		{
			var stop = set.TpOrSlInTick * set.TickSize;
			var agg = set.AggressiveThresholdInTick * set.TickSize;
			var gap = set.LimitGapsInTick * set.TickSize;

			// Opening side
			if (response.Side == Sides.Buy)
			{
				if (depth == set.NextTopDepth) set.NextTopDepth++;
				// Set stop
				var buyStop = _stopOrderService.SendStopOrder(set, Sides.Sell, response.StopPrice - stop, response.StopPrice - stop - agg, response.UserId);
				set.AntiMarketMakerTopStops.AddOrUpdate(buyStop.UserId, buyStop, (s, o) => buyStop);

				// Check new level
				if (depth + 1 != set.NextTopDepth) return;
				if (set.NextTopDepth >= set.MaxDepth) return;
				if (!set.TopBase.HasValue) return;
				var newDepth = set.TopBase.Value + set.NextTopDepth * gap;
				var buy = _stopOrderService.SendStopOrder(set, Sides.Buy, newDepth, newDepth + agg, $"{set.NextTopDepth:0000}_Top");
				set.AntiMarketMakerTopStops.AddOrUpdate(buy.UserId, buy, (s, o) => buy);
			}
			// Closing side
			else if (response.Side == Sides.Sell)
			{
				if (depth + 1 == set.NextTopDepth) set.NextTopDepth--;
				// Reput open pending
				var buyStop = _stopOrderService.SendStopOrder(set, Sides.Buy, response.StopPrice + stop, response.StopPrice + stop + agg, response.UserId);
				set.AntiMarketMakerTopStops.AddOrUpdate(buyStop.UserId, buyStop, (s, o) => buyStop);
			}
		}

		private void PostFillBottom(MarketMaker set, StopResponse response, int depth)
		{
			var stop = set.TpOrSlInTick * set.TickSize;
			var agg = set.AggressiveThresholdInTick * set.TickSize;
			var gap = set.LimitGapsInTick * set.TickSize;

			// Closing side
			if (response.Side == Sides.Buy)
			{
				if (depth + 1 == set.NextBottomDepth) set.NextBottomDepth--;
				// Reput open pending
				var sellStop = _stopOrderService.SendStopOrder(set, Sides.Sell, response.StopPrice - stop, response.StopPrice - stop - agg, response.UserId);
				set.AntiMarketMakerBottomStops.AddOrUpdate(sellStop.UserId, sellStop, (s, o) => sellStop);
			}
			// Opening side
			else if (response.Side == Sides.Sell)
			{
				if (depth == set.NextBottomDepth) set.NextBottomDepth++;
				// Set stop
				var sellStop = _stopOrderService.SendStopOrder(set, Sides.Buy, response.StopPrice + stop, response.StopPrice + stop + agg, response.UserId);
				set.AntiMarketMakerBottomStops.AddOrUpdate(sellStop.UserId, sellStop, (s, o) => sellStop);

				// Check new level
				if (depth + 1 != set.NextBottomDepth) return;
				if (set.NextBottomDepth >= set.MaxDepth) return;
				if (!set.BottomBase.HasValue) return;
				var newDepth = set.BottomBase.Value - set.NextBottomDepth * gap;
				var sell = _stopOrderService.SendStopOrder(set, Sides.Sell, newDepth, newDepth - agg, $"{set.NextBottomDepth:0000}_Bottom");
				set.AntiMarketMakerBottomStops.AddOrUpdate(sell.UserId, sell, (s, o) => sell);
			}
		}
	}
}

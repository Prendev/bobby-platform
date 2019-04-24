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
			set.NextBottomDepth = 0;
			set.NextTopDepth = 0;
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
			if (set.State != MarketMaker.MarketMakerStates.Init) return;

			lock (set)
			{
				if (set.IsBusy) return;
				set.IsBusy = true;
			}

			set.State = MarketMaker.MarketMakerStates.PreTrade;
			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => InitLimits(set));
		}

		private void InitLimits(MarketMaker set)
		{
			_stopOrderService.ClearLimits(set);
			_stopOrderService.Start(set);

			var bid = set.InitBidPrice.HasValue ? set.InitBidPrice.Value : set.Account.GetLastTick(set.Symbol).Bid;
			var ask = set.InitBidPrice.HasValue ? (set.InitBidPrice.Value + set.TickSize) : set.Account.GetLastTick(set.Symbol).Ask;
			set.BottomBase = bid - set.InitialDistanceInTick * set.TickSize;
			set.TopBase = ask + set.InitialDistanceInTick * set.TickSize;
			set.LowestLimit = null;
			set.HighestLimit = null;

			var gap = set.LimitGapsInTick * set.TickSize;
			var market = set.MarketThresholdInTick * set.TickSize;
			for (var i = 0; i < set.InitDepth; i++)
			{
				var buy = _stopOrderService.SendStopOrder(set, Sides.Buy, set.TopBase.Value + i * gap,
					set.TopBase.Value + i * gap + market, $"Top{i}");
				set.HighestLimit = Math.Max(buy.StopPrice, set.HighestLimit ?? buy.StopPrice);
				set.AntiMarketMakerStops.Add(buy);

				var sell = _stopOrderService.SendStopOrder(set, Sides.Sell, set.BottomBase.Value - i * gap,
					set.BottomBase.Value - i * gap - market, $"Bottom{i}");
				set.LowestLimit = Math.Min(sell.StopPrice, set.LowestLimit ?? sell.StopPrice);
				set.AntiMarketMakerStops.Add(sell);
			}

			set.State = MarketMaker.MarketMakerStates.Trade;
			set.IsBusy = false;
		}

		private void _stopOrderService_Fill(object sender, StopResponse e)
		{
			if (!(sender is MarketMaker set)) return;
			if (set.Token.IsCancellationRequested) return;
			if (!set.Run) return;
			if (!set.AntiMarketMakerStops.Contains(e)) return;
			if (set.State == MarketMaker.MarketMakerStates.None) return;

			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => PostFill(set, e));
		}

		private void PostFill(MarketMaker set, StopResponse response)
		{
			var stop = set.TpOrSlInTick * set.TickSize;
			var market = set.MarketThresholdInTick * set.TickSize;
			var gap = set.LimitGapsInTick * set.TickSize;

			if (response.Side == Sides.Buy)
			{
				var sellStop = _stopOrderService.SendStopOrder(set, Sides.Sell, response.StopPrice - stop, response.StopPrice - stop - market, response.Description);
				set.AntiMarketMakerStops.Add(sellStop);

				if (!set.TopBase.HasValue) return;
				if (!set.HighestLimit.HasValue) return;
				var newDepth = response.StopPrice + set.InitDepth * gap;
				if (newDepth <= set.HighestLimit) return;
				if (newDepth >= set.TopBase + set.MaxDepth * gap) return;

				var i = (int)((newDepth - set.TopBase.Value) / gap);
				var buyStop = _stopOrderService.SendStopOrder(set, Sides.Buy, newDepth, newDepth + market, $"Top{i}");
				set.HighestLimit = Math.Max(buyStop.StopPrice, set.HighestLimit ?? buyStop.StopPrice);
				set.AntiMarketMakerStops.Add(buyStop);
			}
			else if (response.Side == Sides.Sell)
			{
				var buyStop = _stopOrderService.SendStopOrder(set, Sides.Buy, response.StopPrice + stop, response.StopPrice + stop + market, response.Description);
				set.AntiMarketMakerStops.Add(buyStop);

				if (!set.BottomBase.HasValue) return;
				if (!set.LowestLimit.HasValue) return;
				var newDepth = response.StopPrice - set.InitDepth * gap;
				if (newDepth >= set.LowestLimit) return;
				if (newDepth <= set.BottomBase - set.MaxDepth * gap) return;

				var i = (int)((set.BottomBase.Value - newDepth) / gap);
				var sellStop = _stopOrderService.SendStopOrder(set, Sides.Buy, newDepth, newDepth + market, $"Bottom{i}");
				set.LowestLimit = Math.Min(sellStop.StopPrice, set.LowestLimit ?? sellStop.StopPrice);
				set.AntiMarketMakerStops.Add(sellStop);
			}
		}
	}
}

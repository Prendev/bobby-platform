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
	public interface IMarketMakerService
	{
		void Start(List<MarketMaker> sets);
		void Stop();
	}

	public class MarketMakerService : IMarketMakerService
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
				new Thread(() => SetLoop(set, _cancellation.Token)) { Name = $"MarketMaker_{set.Id}", IsBackground = true }
					.Start();
			}

			Logger.Info("Market makers are started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("Market makers are stopped");
		}

		private void SetLoop(MarketMaker set, CancellationToken token)
		{
			set.NewTick -= Set_FeedNewTick;
			set.LimitFill -= Set_LimitFill;
			var queue = _queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>());

			if(!set.InitBidPrice.HasValue && set.Account != null)
			{
				set.NewTick += Set_FeedNewTick;
				set.Account.Connector.Subscribe(set.Symbol);
			}

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (set.InitBidPrice.HasValue)
					{
						if (set.State == MarketMaker.MarketMakerStates.None)
						{
							Thread.Sleep(1);
							continue;
						}

						if (set.State == MarketMaker.MarketMakerStates.Init) CheckInit(set);
					}

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

			set.NewTick -= Set_FeedNewTick;
			set.LimitFill -= Set_LimitFill;
			_queues.TryRemove(set.Id, out queue);

			set.State = MarketMaker.MarketMakerStates.None;
			ClearLimits(set);
		}

		private void ClearLimits(MarketMaker set)
		{
			var connector = (FixApiConnectorBase)set.Account.Connector;

			var i = 0;
			while (set.MarketMakerLimits.TryTake(out var limit))
			{
				if (limit.RemainingQuantity == 0) continue;
				try
				{
					if (++i > 0 && set.ThrottlingLimit > 0 && i % set.ThrottlingLimit == 0)
						Thread.Sleep(set.ThrottlingIntervalInMs);

					connector.CancelLimit(limit).Wait();
				}
				catch (Exception e)
				{
					Logger.Error("MarketMakerService.Loop exception", e);
				}
			}
		}

		private void Set_FeedNewTick(object sender, NewTick newTick)
		{
			if (_cancellation.IsCancellationRequested) return;
			CheckInit((MarketMaker) sender);
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

		private async void InitLimits(MarketMaker set)
		{
			ClearLimits(set);

			set.LimitFill -= Set_LimitFill;
			set.LimitFill += Set_LimitFill;

			var connector = (FixApiConnectorBase)set.Account.Connector;
			var bid = set.InitBidPrice.HasValue ? set.InitBidPrice.Value : set.Account.GetLastTick(set.Symbol).Bid;
			var ask = set.InitBidPrice.HasValue ? (set.InitBidPrice.Value + set.TickSize) : set.Account.GetLastTick(set.Symbol).Ask;
			set.BottomBase = bid - set.InitialDistanceInTick * set.TickSize;
			set.TopBase = ask + set.InitialDistanceInTick * set.TickSize;
			set.LowestLimit = null;
			set.HighestLimit = null;

			var gap = set.LimitGapsInTick * set.TickSize;
			var quant = set.ContractSize;
			var sym = set.Symbol;
			for (var i = 0; i < set.InitDepth; i++)
			{
				if (i > 0 && set.ThrottlingLimit > 0 && i % (set.ThrottlingLimit / 2) == 0)
					Thread.Sleep(set.ThrottlingIntervalInMs);

				try
				{
					var buy = await connector.PutNewOrderRequest(sym, Sides.Buy, quant, set.BottomBase.Value - i * gap);
					set.LowestLimit = Math.Min(buy.OrderPrice, set.LowestLimit ?? buy.OrderPrice);
					set.MarketMakerLimits.Add(buy);
				}
				catch (Exception e)
				{
					Logger.Error("MarketMakerService.InitLimits exception", e);
				}

				try
				{
					var sell = await connector.PutNewOrderRequest(sym, Sides.Sell, quant, set.TopBase.Value + i * gap);
					set.HighestLimit = Math.Max(sell.OrderPrice, set.HighestLimit ?? sell.OrderPrice);
					set.MarketMakerLimits.Add(sell);
				}
				catch (Exception e)
				{
					Logger.Error("MarketMakerService.InitLimits exception", e);
				}
			}

			set.State = MarketMaker.MarketMakerStates.Trade;
			set.IsBusy = false;
		}

		private void Set_LimitFill(object sender, LimitFill limitFill)
		{
			if (_cancellation.IsCancellationRequested) return;
			var set = (MarketMaker)sender;

			if (!set.Run) return;
			if (!set.MarketMakerLimits.Contains(limitFill.LimitResponse)) return;
			// if (set.State == MarketMaker.MarketMakerStates.Cancel) return;
			if (set.State == MarketMaker.MarketMakerStates.None) return;

			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => PostLimitFill(set, limitFill));
		}

		private async void PostLimitFill(MarketMaker set, LimitFill limitFill)
		{
			var connector = (FixApiConnectorBase)set.Account.Connector;
			var sym = set.Symbol;
			var quant = limitFill.Quantity;
			var tp = set.TpOrSlInTick * set.TickSize;
			var gap = set.LimitGapsInTick * set.TickSize;

			if (limitFill.LimitResponse.Side == Sides.Buy)
			{
				var tpLimit = await connector.PutNewOrderRequest(sym, Sides.Sell, quant, limitFill.Price + tp);
				set.MarketMakerLimits.Add(tpLimit);

				if (!set.BottomBase.HasValue) return;
				if (!set.LowestLimit.HasValue) return;
				var newDepth = limitFill.Price - set.InitDepth * gap;
				if (newDepth >= set.LowestLimit) return;
				if (newDepth <= set.BottomBase - set.MaxDepth * gap) return;

				var newLimit = await connector.PutNewOrderRequest(sym, Sides.Buy, set.ContractSize, newDepth);
				set.LowestLimit = Math.Min(newLimit.OrderPrice, set.LowestLimit ?? newLimit.OrderPrice);
				set.MarketMakerLimits.Add(newLimit);
			}
			else if (limitFill.LimitResponse.Side == Sides.Sell)
			{
				var tpLimit = await connector.PutNewOrderRequest(sym, Sides.Buy, quant, limitFill.Price - tp);
				set.MarketMakerLimits.Add(tpLimit);

				if (!set.TopBase.HasValue) return;
				if (!set.HighestLimit.HasValue) return;
				var newDepth = limitFill.Price + set.InitDepth * gap;
				if (newDepth <= set.HighestLimit) return;
				if (newDepth >= set.TopBase + set.MaxDepth * gap) return;

				var newLimit = await connector.PutNewOrderRequest(sym, Sides.Sell, set.ContractSize, newDepth);
				set.HighestLimit = Math.Max(newLimit.OrderPrice, set.HighestLimit ?? newLimit.OrderPrice);
				set.MarketMakerLimits.Add(newLimit);
			}
		}
	}
}

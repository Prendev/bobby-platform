﻿using System;
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
			set.FeedNewTick -= Set_FeedNewTick;
			set.LimitFill -= Set_LimitFill;
			var queue = _queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>());

			if(!set.InitBidPrice.HasValue && set.FeedAccount != null)
			{
				set.FeedNewTick += Set_FeedNewTick;
				set.FeedAccount?.Connector.Subscribe(set.FeedSymbol);
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

			set.FeedNewTick -= Set_FeedNewTick;
			set.LimitFill -= Set_LimitFill;
			_queues.TryRemove(set.Id, out queue);

			set.State = MarketMaker.MarketMakerStates.None;
			ClearLimits(set);
		}

		private void ClearLimits(MarketMaker set)
		{
			var connector = (FixApiConnectorBase)set.TradeAccount.Connector;

			var i = 0;
			while (set.Limits.TryTake(out var limit))
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

			var connector = (FixApiConnectorBase)set.TradeAccount.Connector;
			var bid = set.InitBidPrice.HasValue ? set.InitBidPrice.Value : connector.GetLastTick(set.FeedSymbol).Bid;
			var ask = set.InitBidPrice.HasValue ? (set.InitBidPrice.Value + set.TickSize) : connector.GetLastTick(set.FeedSymbol).Ask;
			set.LongBase = bid - set.InitialDistanceInTick * set.TickSize;
			set.ShortBase = ask + set.InitialDistanceInTick * set.TickSize;
			set.MinLongLimit = null;
			set.MaxShortLimit = null;

			var gap = set.LimitGapsInTick * set.TickSize;
			var quant = set.ContractSize;
			var sym = set.TradeSymbol;
			for (var i = 0; i < set.InitDepth; i++)
			{
				if (i > 0 && set.ThrottlingLimit > 0 && i % (set.ThrottlingLimit / 2) == 0)
					Thread.Sleep(set.ThrottlingIntervalInMs);

				try
				{
					var buy = await connector.SendSpoofOrderRequest(sym, Sides.Buy, quant, set.LongBase.Value - i * gap);
					set.MinLongLimit = Math.Min(buy.OrderPrice, set.MinLongLimit ?? buy.OrderPrice);
					set.Limits.Add(buy);
				}
				catch (Exception e)
				{
					Logger.Error("MarketMakerService.InitLimits exception", e);
				}

				try
				{
					var sell = await connector.SendSpoofOrderRequest(sym, Sides.Sell, quant, set.ShortBase.Value + i * gap);
					set.MaxShortLimit = Math.Max(sell.OrderPrice, set.MaxShortLimit ?? sell.OrderPrice);
					set.Limits.Add(sell);
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
			if (!set.Limits.Contains(limitFill.LimitResponse)) return;
			// if (set.State == MarketMaker.MarketMakerStates.Cancel) return;
			if (set.State == MarketMaker.MarketMakerStates.None) return;

			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => PostLimitFill(set, limitFill));
		}

		private async void PostLimitFill(MarketMaker set, LimitFill limitFill)
		{
			var connector = (FixApiConnectorBase)set.TradeAccount.Connector;
			var sym = set.TradeSymbol;
			var quant = limitFill.Quantity;
			var tp = set.TpInTick * set.TickSize;

			if (limitFill.LimitResponse.Side == Sides.Buy)
			{
				var tpLimit = await connector.SendSpoofOrderRequest(sym, Sides.Sell, quant, limitFill.Price + tp);
				set.Limits.Add(tpLimit);

				if (!set.LongBase.HasValue) return;
				if (!set.MinLongLimit.HasValue) return;
				var newDepth = limitFill.Price - set.InitDepth * set.TickSize;
				if (newDepth >= set.MinLongLimit) return;
				if (newDepth <= set.LongBase - set.MaxDepth * set.TickSize) return;

				var newLimit = await connector.SendSpoofOrderRequest(sym, Sides.Buy, set.ContractSize, newDepth);
				set.MinLongLimit = Math.Min(newLimit.OrderPrice, set.MinLongLimit ?? newLimit.OrderPrice);
				set.Limits.Add(newLimit);
			}
			else if (limitFill.LimitResponse.Side == Sides.Sell)
			{
				var tpLimit = await connector.SendSpoofOrderRequest(sym, Sides.Buy, quant, limitFill.Price - tp);
				set.Limits.Add(tpLimit);

				if (!set.ShortBase.HasValue) return;
				if (!set.MaxShortLimit.HasValue) return;
				var newDepth = limitFill.Price + set.InitDepth * set.TickSize;
				if (newDepth <= set.MaxShortLimit) return;
				if (newDepth >= set.ShortBase + set.MaxDepth * set.TickSize) return;

				var newLimit = await connector.SendSpoofOrderRequest(sym, Sides.Sell, set.ContractSize, newDepth);
				set.MaxShortLimit = Math.Max(newLimit.OrderPrice, set.MaxShortLimit ?? newLimit.OrderPrice);
				set.Limits.Add(newLimit);
			}
		}
	}
}

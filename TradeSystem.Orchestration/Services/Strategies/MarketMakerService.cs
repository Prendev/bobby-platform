using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
		private readonly object _syncRoot = new object();

		private readonly ConcurrentDictionary<int, FastBlockingCollection<Action>> _queues =
			new ConcurrentDictionary<int, FastBlockingCollection<Action>>();
		private readonly List<LimitResponse> _limits = new List<LimitResponse>();

		public void Start(List<MarketMaker> sets)
		{
			_cancellation?.Dispose();

			_sets = sets;
			_cancellation = new CancellationTokenSource();

			foreach (var set in _sets)
			{
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
			set.FeedNewTick += Set_FeedNewTick;
			set.FeedAccount.Connector.Subscribe(set.FeedSymbol);

			while (!token.IsCancellationRequested)
			{
				try
				{
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

			try
			{
				var connector = (FixApiConnectorBase)set.TradeAccount.Connector;
				foreach (var limit in _limits) connector.CancelLimit(limit).Wait();
			}
			catch (Exception e)
			{
				Logger.Error("MarketMakerService.Loop exception", e);
			}
			finally
			{
				_limits.Clear();
			}
		}

		private void Set_FeedNewTick(object sender, NewTick newTick)
		{
			if (_cancellation.IsCancellationRequested) return;
			var set = (MarketMaker)sender;

			if (!set.Run) return;
			if (set.State != MarketMaker.MarketMakerStates.Init) return;

			lock (set)
			{
				if (set.IsBusy) return;
				set.IsBusy = true;
			}
			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => InitLimits(set, newTick));
		}

		private async void InitLimits(MarketMaker set, NewTick newTick)
		{
			set.LimitFill -= Set_LimitFill;
			set.LimitFill += Set_LimitFill;

			var connector = (FixApiConnectorBase)set.TradeAccount.Connector;
			var sym = set.TradeSymbol;
			set.LongBase = newTick.Tick.Bid - set.InitialDistanceInTick * set.TickSize;
			set.ShortBase = newTick.Tick.Ask + set.InitialDistanceInTick * set.TickSize;
			var gap = set.LimitGapsInTick * set.TickSize;
			var quant = set.ContractSize;

			for (var i = 0; i < set.InitDepth; i++)
			{
				var buy = await connector.SendSpoofOrderRequest(sym, Sides.Buy, quant, set.LongBase.Value - i * gap);
				set.MinLongLimit = Math.Min(buy.OrderPrice, set.MinLongLimit ?? buy.OrderPrice);
				_limits.Add(buy);
				var sell = await connector.SendSpoofOrderRequest(sym, Sides.Sell, quant, set.ShortBase.Value + i * gap);
				set.MaxShortLimit = Math.Max(sell.OrderPrice, set.MaxShortLimit ?? sell.OrderPrice);
				_limits.Add(sell);
			}

			set.State = MarketMaker.MarketMakerStates.Trade;
			set.IsBusy = false;
		}

		private void Set_LimitFill(object sender, LimitFill limitFill)
		{
			if (_cancellation.IsCancellationRequested) return;
			var set = (MarketMaker)sender;

			if (!set.Run) return;
			if (!_limits.Contains(limitFill.LimitResponse)) return;
			if (set.State == MarketMaker.MarketMakerStates.Cancel) return;
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
				_limits.Add(tpLimit);

				if (!set.LongBase.HasValue) return;
				if (!set.MinLongLimit.HasValue) return;
				var newDepth = limitFill.Price - set.InitDepth * set.TickSize;
				if (newDepth >= set.MinLongLimit) return;
				if (newDepth <= set.LongBase - set.MaxDepth * set.TickSize) return;

				var newLimit = await connector.SendSpoofOrderRequest(sym, Sides.Buy, set.ContractSize, newDepth);
				set.MinLongLimit = Math.Min(newLimit.OrderPrice, set.MinLongLimit ?? newLimit.OrderPrice);
				_limits.Add(newLimit);
			}
			else if (limitFill.LimitResponse.Side == Sides.Sell)
			{
				var tpLimit = await connector.SendSpoofOrderRequest(sym, Sides.Buy, quant, limitFill.Price - tp);
				_limits.Add(tpLimit);

				if (!set.ShortBase.HasValue) return;
				if (!set.MaxShortLimit.HasValue) return;
				var newDepth = limitFill.Price + set.InitDepth * set.TickSize;
				if (newDepth <= set.MaxShortLimit) return;
				if (newDepth >= set.ShortBase + set.MaxDepth * set.TickSize) return;

				var newLimit = await connector.SendSpoofOrderRequest(sym, Sides.Sell, set.ContractSize, newDepth);
				set.MaxShortLimit = Math.Max(newLimit.OrderPrice, set.MaxShortLimit ?? newLimit.OrderPrice);
				_limits.Add(newLimit);
			}
		}
	}
}

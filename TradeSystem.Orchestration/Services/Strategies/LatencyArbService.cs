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
	public interface ILatencyArbService
	{
		void Start(List<LatencyArb> sets);
		void Stop();
	}

	public class LatencyArbService : ILatencyArbService
	{
		private volatile CancellationTokenSource _cancellation;

		private List<LatencyArb> _sets;
		private readonly ConcurrentDictionary<int, FastBlockingCollection<Action>> _queues =
			new ConcurrentDictionary<int, FastBlockingCollection<Action>>();

		public void Start(List<LatencyArb> sets)
		{
			_cancellation?.Dispose();

			_sets = sets;
			_cancellation = new CancellationTokenSource();

			foreach (var set in _sets)
			{
				if (!set.Run) continue;
				if (!set.IsConnected) continue;
				new Thread(() => SetLoop(set, _cancellation.Token)) { Name = $"MarketMaker_{set.Id}", IsBackground = true }
					.Start();
			}

			Logger.Info("Latency arbs are started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("Latency arbs are stopped");
		}

		private void SetLoop(LatencyArb set, CancellationToken token)
		{
			var queue = _queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>());

			set.NewTick -= Set_NewTick;
			set.FastFeedAccount.Connector.Subscribe(set.FastFeedSymbol);
			set.LongAccount.Connector.Subscribe(set.LongSymbol);
			set.ShortAccount.Connector.Subscribe(set.ShortSymbol);
			set.NewTick += Set_NewTick;

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (set.State == LatencyArb.LatencyArbStates.None)
					{
						Thread.Sleep(1);
						continue;
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
					Logger.Error("LatencyArbService.Loop exception", e);
				}
			}

			_queues.TryRemove(set.Id, out queue);
		}

		private void Set_NewTick(object sender, NewTick e)
		{
			if (_cancellation.IsCancellationRequested) return;
			var set = (LatencyArb)sender;
			if (!set.Run) return;
			if (set.State == LatencyArb.LatencyArbStates.None) return;

			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => Check(set));
		}

		private void Check(LatencyArb set)
		{
			CheckPositions(set);
			if (set.State == LatencyArb.LatencyArbStates.None) return;
			if (set.State == LatencyArb.LatencyArbStates.Opening) CheckOpening(set);
		}


		private void CheckOpening(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Opening) return;
			var last = set.LatencyArbPositions.FirstOrDefault(p => !p.LongTicket.HasValue || !p.ShortTicket.HasValue);

			// Check for new signal
			if (last == null)
			{
				if (set.LatencyArbPositions.Count >= set.MaxCount) return;
				// Long signal
				if (set.LastFeedTick.Ask >= set.LastLongTick.Ask + set.SignalDiffInPip * set.PipSize)
				{
					var pos = SendLongOrder(set);
					if (pos == null) return;
					set.LatencyArbPositions.Add(new LatencyArbPosition()
					{
						LatencyArb = set,
						LongTicket = pos.Id,
						OpenPrice = pos.OpenPrice,
						Trailing = pos.OpenPrice - set.TrailingInPip * set.PipSize,
					});
				}
				// Short signal
				else if (set.LastFeedTick.Bid <= set.LastShortTick.Bid - set.SignalDiffInPip * set.PipSize)
				{
					var pos = SendShortOrder(set);
					if (pos == null) return;
					set.LatencyArbPositions.Add(new LatencyArbPosition()
					{
						LatencyArb = set,
						ShortTicket = pos.Id,
						OpenPrice = pos.OpenPrice,
						Trailing = pos.OpenPrice + set.TrailingInPip * set.PipSize,
					});
				}
			}
			// Long side opened
			else if (!last.ShortTicket.HasValue)
			{
				var close = false;
				var price = set.LastShortTick.Bid;
				if (price - set.TrailingInPip * set.PipSize > last.Trailing) last.Trailing = price - set.TrailingInPip * set.PipSize;
				if (price >= last.OpenPrice + set.TpInPip * set.PipSize) close = true;
				if (price <= last.OpenPrice - set.SlInPip * set.PipSize) close = true;
				if (price <= last.Trailing) close = true;
				if (!close) return;
				var pos = SendShortOrder(set);
				if (pos == null) return;
				last.ShortTicket = pos.Id;
			}
			// Short side opened
			else if (!last.LongTicket.HasValue)
			{
				var close = false;
				var price = set.LastLongTick.Ask;
				if (price + set.TrailingInPip * set.PipSize < last.Trailing) last.Trailing = price + set.TrailingInPip * set.PipSize;
				if (price <= last.OpenPrice + set.TpInPip * set.PipSize) close = true;
				if (price >= last.OpenPrice - set.SlInPip * set.PipSize) close = true;
				if (price >= last.Trailing) close = true;
				if (!close) return;
				var pos = SendLongOrder(set);
				if (pos == null) return;
				last.LongTicket = pos.Id;
			}
		}

		private Position SendLongOrder(LatencyArb set)
		{
			return ((Mt4Integration.Connector) set.LongAccount.Connector).SendMarketOrderRequest(set.LongSymbol, Sides.Buy,
				(double) set.Size, 0, null);
		}
		private Position SendShortOrder(LatencyArb set)
		{
			return ((Mt4Integration.Connector)set.ShortAccount.Connector).SendMarketOrderRequest(set.ShortSymbol, Sides.Sell,
				(double)set.Size, 0, null);
		}

		private void CheckPositions(LatencyArb set)
		{
			foreach (var pos in set.LatencyArbPositions)
			{
				var longPositions = ((Mt4Integration.Connector)set.LongAccount.Connector).Positions;
				var shortPositions = ((Mt4Integration.Connector)set.ShortAccount.Connector).Positions;

				if (pos.LongTicket.HasValue &&
				    (!longPositions.TryGetValue(pos.LongTicket.Value, out var longPos) || longPos.IsClosed))
					pos.LongClosed = true;

				if (pos.ShortTicket.HasValue &&
				    (!shortPositions.TryGetValue(pos.ShortTicket.Value, out var shortPos) || shortPos.IsClosed))
					pos.ShortClosed = true;

				if (pos.LongClosed && (!pos.ShortTicket.HasValue || !shortPositions.TryGetValue(pos.ShortTicket.Value, out var _)))
					pos.ShortClosed = true;
				if (pos.ShortClosed && (!pos.LongTicket.HasValue || !longPositions.TryGetValue(pos.LongTicket.Value, out var _)))
					pos.LongClosed = true;
			}
			set.LatencyArbPositions.RemoveAll(p => p.LongClosed && p.ShortClosed);
		}
	}
}

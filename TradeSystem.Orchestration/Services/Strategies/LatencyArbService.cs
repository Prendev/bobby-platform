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
				new Thread(() => SetLoop(set, _cancellation.Token)) { Name = $"LatencyArb_{set.Id}", IsBackground = true }
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

			set.State = LatencyArb.LatencyArbStates.None;
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
			if (set.State == LatencyArb.LatencyArbStates.None) return;

			CheckPositions(set);
			CheckOpening(set);
			CheckClosing(set);
		}


		private void CheckOpening(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Opening) return;
			var last = set.LatencyArbPositions.OrderByDescending(p => p.Level).FirstOrDefault();

			// Check for new signal
			var checkNew = last == null || (last.LongTicket.HasValue && last.ShortTicket.HasValue);
			if (checkNew)
			{
				if (set.LatencyArbPositions.Count >= set.MaxCount) return;
				// Long signal
				if (set.LastFeedTick.Ask >= set.LastLongTick.Ask + set.SignalDiffInPip * set.PipSize)
				{
					var level = set.LatencyArbPositions.Count + 1;
					var pos = SendLongOrder(set);
					if (pos == null)
					{
						Logger.Warn($"{set} latency arb - {level}. long first side open error");
						return;
					}
					set.LatencyArbPositions.Add(new LatencyArbPosition()
					{
						LatencyArb = set,
						LongTicket = pos.Id,
						Price = pos.OpenPrice,
						Level = level
					});
					Logger.Info($"{set} latency arb - {level}. long first side opened at {pos.OpenPrice}");
				}
				// Short signal
				else if (set.LastFeedTick.Bid <= set.LastShortTick.Bid - set.SignalDiffInPip * set.PipSize)
				{
					var level = set.LatencyArbPositions.Count + 1;
					var pos = SendShortOrder(set);
					if (pos == null)
					{
						Logger.Warn($"{set} latency arb - {level}. short first side open error");
						return;
					}
					set.LatencyArbPositions.Add(new LatencyArbPosition()
					{
						LatencyArb = set,
						ShortTicket = pos.Id,
						Price = pos.OpenPrice,
						Level = level
					});
					Logger.Info($"{set} latency arb - {level}. short first side opened at {pos.OpenPrice}");
				}
			}
			// Long side opened
			else if (last.LongTicket.HasValue)
			{
				var hedge = false;
				var price = set.LastShortTick.Bid;

				if (last.Trailing.HasValue || price >= last.Price + set.TrailingSwitchInPip * set.PipSize)
					last.Trailing = Math.Max(price - set.TrailingDistanceInPip * set.PipSize,
						last.Trailing ?? price - set.TrailingDistanceInPip * set.PipSize);
				if (last.Trailing.HasValue && price <= last.Trailing) hedge = true;

				if (price >= last.Price + set.TpInPip * set.PipSize) hedge = true;
				if (price <= last.Price - set.SlInPip * set.PipSize) hedge = true;
				if (!hedge) return;

				var pos = SendShortOrder(set);
				if (pos == null)
				{
					Logger.Warn($"{set} latency arb - {last.Level}. short hedge side open error");
					return;
				}
				last.ShortTicket = pos.Id;
				Logger.Info($"{set} latency arb - {last.Level}. short hedge side opened at {pos.OpenPrice} with {(pos.OpenPrice - last.Price)/set.PipSize} pips");
			}
			// Short side opened
			else if (last.ShortTicket.HasValue)
			{
				var hedge = false;
				var price = set.LastLongTick.Ask;

				if (last.Trailing.HasValue || price <= last.Price - set.TrailingSwitchInPip * set.PipSize)
					last.Trailing = Math.Min(price + set.TrailingDistanceInPip * set.PipSize,
						last.Trailing ?? price + set.TrailingDistanceInPip * set.PipSize);
				if (last.Trailing.HasValue && price >= last.Trailing) hedge = true;

				if (price <= last.Price - set.TpInPip * set.PipSize) hedge = true;
				if (price >= last.Price + set.SlInPip * set.PipSize) hedge = true;
				if (!hedge) return;

				var pos = SendLongOrder(set);
				if (pos == null)
				{
					Logger.Warn($"{set} latency arb - {last.Level}. long hedge side open error");
					return;
				}
				last.LongTicket = pos.Id;
				Logger.Info($"{set} latency arb - {last.Level}. long hedge side opened at {pos.OpenPrice} with {(last.Price - pos.OpenPrice) / set.PipSize} pips");
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

		private void CheckClosing(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Closing) return;

			var first = set.LatencyArbPositions.OrderBy(p => p.Level).FirstOrDefault(p => p.LongTicket.HasValue && p.ShortTicket.HasValue);
			if (first == null) return;
			if (!first.LongTicket.HasValue) return;
			if (!first.ShortTicket.HasValue) return;

			// No closed side
			if (!first.LongClosed && !first.ShortClosed)
			{
				// Long close signal
				if (set.LastFeedTick.Bid <= set.LastLongTick.Bid - set.SignalDiffInPip * set.PipSize)
				{
					var pos = CloseLong(set, first.LongTicket.Value);
					if (pos?.IsClosed != true)
						return;
					first.LongClosed = true;
					first.Price = pos.ClosePrice;
					first.Trailing = null;
					Logger.Info($"{set} latency arb - {first.Level}. long first side closed at {pos.ClosePrice}");
				}
				// Short close signal
				else if (set.LastFeedTick.Ask >= set.LastShortTick.Ask + set.SignalDiffInPip * set.PipSize)
				{
					var pos = CloseShort(set, first.ShortTicket.Value);
					if (pos?.IsClosed != true)
						return;
					first.ShortClosed = true;
					first.Price = pos.ClosePrice;
					first.Trailing = null;
					Logger.Info($"{set} latency arb - {first.Level}. short first side closed at {pos.ClosePrice}");
				}
			}
			// Long side closed
			else if (first.LongClosed)
			{
				var hedge = false;
				var price = set.LastShortTick.Ask;

				if (first.Trailing.HasValue || price <= first.Price - set.TrailingSwitchInPip * set.PipSize)
					first.Trailing = Math.Min(price + set.TrailingDistanceInPip * set.PipSize,
						first.Trailing ?? price + set.TrailingDistanceInPip * set.PipSize);
				if (first.Trailing.HasValue && price >= first.Trailing) hedge = true;

				if (price <= first.Price - set.TpInPip * set.PipSize) hedge = true;
				if (price >= first.Price + set.SlInPip * set.PipSize) hedge = true;
				if (!hedge) return;

				var pos = CloseShort(set, first.ShortTicket.Value);
				if (pos?.IsClosed != true) return;
				first.ShortClosed = true;
				Logger.Info($"{set} latency arb - {first.Level}. short hedge side closed at {pos.ClosePrice} with {(first.Price - pos.ClosePrice)/set.PipSize} pips");
			}
			// Short side closed
			else if (first.ShortClosed)
			{
				var hedge = false;
				var price = set.LastLongTick.Bid;

				if (first.Trailing.HasValue || price >= first.Price + set.TrailingSwitchInPip * set.PipSize)
					first.Trailing = Math.Max(price - set.TrailingDistanceInPip * set.PipSize,
						first.Trailing ?? price - set.TrailingDistanceInPip * set.PipSize);
				if (first.Trailing.HasValue && price <= first.Trailing) hedge = true;

				if (price >= first.Price + set.TpInPip * set.PipSize) hedge = true;
				if (price <= first.Price - set.SlInPip * set.PipSize) hedge = true;
				if (!hedge) return;

				var pos = CloseLong(set, first.LongTicket.Value);
				if (pos?.IsClosed != true) return;
				first.LongClosed = true;
				Logger.Info($"{set} latency arb - {first.Level}. long hedge side closed at {pos.ClosePrice} with {(pos.ClosePrice - first.Price) / set.PipSize} pips");
			}
		}

		private Position CloseLong(LatencyArb set, long ticket)
		{
			var connector = (Mt4Integration.Connector) set.LongAccount.Connector;
			connector.Positions.TryGetValue(ticket, out var pos);
			if (pos == null) return null;
			connector.SendClosePositionRequests(pos);
			return pos;
		}
		private Position CloseShort(LatencyArb set, long ticket)
		{
			var connector = (Mt4Integration.Connector)set.ShortAccount.Connector;
			connector.Positions.TryGetValue(ticket, out var pos);
			if (pos == null) return null;
			connector.SendClosePositionRequests(pos);
			return pos;
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

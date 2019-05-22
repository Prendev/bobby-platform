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
	public interface ILatencyArbService
	{
		void Start(List<LatencyArb> sets);
		void Stop();
	}

	public class LatencyArbService : ILatencyArbService
	{
		private class OpenResult
		{
			public long? Ticket { get; set; }
			public StratPosition StratPosition { get; set; }
			public decimal OpenPrice { get; set; }
		}

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
			CheckReopening(set);
			CheckClosing(set);
		}

		private void CheckOpening(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Opening &&
			    set.State != LatencyArb.LatencyArbStates.ReopeningShort &&
			    set.State != LatencyArb.LatencyArbStates.ReopeningLong) return;

			var last = set.LatencyArbPositions
				.OrderByDescending(p => p.Level)
				.FirstOrDefault(p => !p.HasLong || !p.HasShort);

			// Check for new signal
			if (last == null)
			{
				if (set.State != LatencyArb.LatencyArbStates.Opening) return;
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
						LongTicket = pos.Ticket,
						LongPosition = pos.StratPosition,
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
						ShortTicket = pos.Ticket,
						ShortPosition = pos.StratPosition,
						Price = pos.OpenPrice,
						Level = level
					});
					Logger.Info($"{set} latency arb - {level}. short first side opened at {pos.OpenPrice}");
				}
			}
			// Long side opened
			else if (last.HasLong)
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
				last.ShortTicket = pos.Ticket;
				last.ShortPosition = pos.StratPosition;
				Logger.Info($"{set} latency arb - {last.Level}. short hedge side opened at {pos.OpenPrice} with {(pos.OpenPrice - last.Price)/set.PipSize} pips");
			}
			// Short side opened
			else if (last.HasShort)
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
				last.LongTicket = pos.Ticket;
				last.LongPosition = pos.StratPosition;
				Logger.Info($"{set} latency arb - {last.Level}. long hedge side opened at {pos.OpenPrice} with {(last.Price - pos.OpenPrice) / set.PipSize} pips");
			}
		}
		private OpenResult SendLongOrder(LatencyArb set)
		{
			if (set.LongAccount.Connector is Mt4Integration.Connector connector)
			{
				var pos = connector.SendMarketOrderRequest(set.LongSymbol, Sides.Buy, (double)set.LongSize, 0, null);
				return pos == null ? null : new OpenResult {Ticket = pos.Id, OpenPrice = pos.OpenPrice};
			}

			if (set.LongAccount.Connector is FixApiIntegration.Connector fixConnector)
			{
				var result = fixConnector.SendMarketOrderRequest(set.LongSymbol, Sides.Buy, set.LongSize).Result;
				if (result?.IsFilled != true) return null;
				return new OpenResult
				{
					OpenPrice = result.AveragePrice.Value,
					StratPosition = new StratPosition()
					{
						Account = set.LongAccount,
						AvgPrice = result.AveragePrice.Value,
						OpenTime = HiResDatetime.UtcNow,
						Side = result.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell,
						Symbol = set.LongSymbol
					}
				};
			}

			return null;
		}
		private OpenResult SendShortOrder(LatencyArb set)
		{
			if (set.ShortAccount.Connector is Mt4Integration.Connector connector)
			{
				var pos = connector.SendMarketOrderRequest(set.ShortSymbol, Sides.Sell, (double)set.ShortSize, 0, null);
				return pos == null ? null : new OpenResult { Ticket = pos.Id, OpenPrice = pos.OpenPrice };
			}

			if (set.ShortAccount.Connector is FixApiIntegration.Connector fixConnector)
			{
				var result = fixConnector.SendMarketOrderRequest(set.ShortSymbol, Sides.Sell, set.ShortSize).Result;
				if (result?.IsFilled != true) return null;
				return new OpenResult
				{
					OpenPrice = result.AveragePrice.Value,
					StratPosition = new StratPosition()
					{
						Account = set.ShortAccount,
						AvgPrice = result.AveragePrice.Value,
						OpenTime = HiResDatetime.UtcNow,
						Side = result.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell,
						Symbol = set.ShortSymbol
					}
				};
			}

			return null;
		}

		private void CheckReopening(LatencyArb set)
		{
			if (set.LatencyArbPositions.Any(p => !p.HasShort || !p.HasLong)) return;
			CheckReopeningShort(set);
			CheckReopeningLong(set);
		}
		private void CheckReopeningShort(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.ReopeningShort) return;

			LatencyArbPosition first = null;
			var positions = set.LatencyArbPositions.OrderBy(p => p.Level).Where(p => p.HasBothSides);
			foreach (var position in positions)
			{
				if (!position.HasBothSides) continue;
				var openTime = GetShortOpenTime(set, position);
				if (!openTime.HasValue) continue;
				if ((HiResDatetime.UtcNow - openTime.Value).Days < set.ReopenThresholdInDay) continue;
				first = position;
				break;
			}
			if (first == null) return;
			if (!first.HasBothSides) return;
			if (set.LastFeedTick.Ask < set.LastShortTick.Ask + set.SignalDiffInPip * set.PipSize) return;

			var closePrice = CloseShort(set, first);
			if (!closePrice.HasValue) return;
			first.Price = closePrice;
			first.Trailing = null;
			first.ShortTicket = null;
			first.ShortPosition = null;
			Logger.Info($"{set} latency arb - {first.Level}. short side closed for reopening at {closePrice}");
		}
		private void CheckReopeningLong(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.ReopeningLong) return;

			LatencyArbPosition first = null;
			var positions = set.LatencyArbPositions.OrderBy(p => p.Level).Where(p => p.HasBothSides);
			foreach (var position in positions)
			{
				if (!position.HasBothSides) continue;
				var openTime = GetLongOpenTime(set, position);
				if (!openTime.HasValue) continue;
				if ((HiResDatetime.UtcNow - openTime.Value).Days < set.ReopenThresholdInDay) continue;
				first = position;
				break;
			}
			if (first == null) return;
			if (!first.HasBothSides) return;
			if (set.LastFeedTick.Bid > set.LastLongTick.Bid - set.SignalDiffInPip * set.PipSize) return;

			var closePrice = CloseLong(set, first);
			if (!closePrice.HasValue) return;
			first.Price = closePrice;
			first.Trailing = null;
			first.LongTicket = null;
			first.LongPosition = null;
			Logger.Info($"{set} latency arb - {first.Level}. long side closed for reopening at {closePrice}");
		}
		private DateTime? GetShortOpenTime(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.ShortAccount.Connector is Mt4Integration.Connector connector)
			{
				if (!arbPos.ShortTicket.HasValue) return null;
				connector.Positions.TryGetValue(arbPos.ShortTicket.Value, out var pos);
				if (pos == null) return null;
				if (pos.IsClosed) return null;
				return pos.OpenTime;
			}

			return arbPos.ShortPosition?.OpenTime;
		}
		private DateTime? GetLongOpenTime(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.LongAccount.Connector is Mt4Integration.Connector connector)
			{
				if (!arbPos.LongTicket.HasValue) return null;
				connector.Positions.TryGetValue(arbPos.LongTicket.Value, out var pos);
				if (pos == null) return null;
				if (pos.IsClosed) return null;
				return pos.OpenTime;
			}

			return arbPos.LongPosition?.OpenTime;
		}

		private void CheckClosing(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Closing) return;

			var first = set.LatencyArbPositions.OrderBy(p => p.Level).FirstOrDefault(p => p.HasBothSides);
			if (first == null) return;
			if (!first.HasBothSides) return;

			// No closed side
			if (!first.LongClosed && !first.ShortClosed)
			{
				// Long close signal
				if (set.LastFeedTick.Bid <= set.LastLongTick.Bid - set.SignalDiffInPip * set.PipSize)
				{
					var closePrice = CloseLong(set, first);
					if (!closePrice.HasValue) return;
					first.LongClosed = true;
					first.Price = closePrice;
					first.Trailing = null;
					Logger.Info($"{set} latency arb - {first.Level}. long first side closed at {closePrice}");
				}
				// Short close signal
				else if (set.LastFeedTick.Ask >= set.LastShortTick.Ask + set.SignalDiffInPip * set.PipSize)
				{
					var closePrice = CloseShort(set, first);
					if (!closePrice.HasValue) return;
					first.ShortClosed = true;
					first.Price = closePrice;
					first.Trailing = null;
					Logger.Info($"{set} latency arb - {first.Level}. short first side closed at {closePrice}");
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

				var closePrice = CloseShort(set, first);
				if (!closePrice.HasValue) return;
				first.ShortClosed = true;
				Logger.Info($"{set} latency arb - {first.Level}. short hedge side closed at {closePrice} with {(first.Price - closePrice) /set.PipSize} pips");
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

				var closePrice = CloseLong(set, first);
				if (!closePrice.HasValue) return;
				first.LongClosed = true;
				Logger.Info($"{set} latency arb - {first.Level}. long hedge side closed at {closePrice} with {(closePrice - first.Price) / set.PipSize} pips");
			}
		}

		private decimal? CloseLong(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.LongAccount.Connector is Mt4Integration.Connector connector)
			{
				if (!arbPos.LongTicket.HasValue) return null;
				connector.Positions.TryGetValue(arbPos.LongTicket.Value, out var pos);
				if (pos == null) return null;
				connector.SendClosePositionRequests(pos);
				return pos.ClosePrice;
			}
			if (set.LongAccount.Connector is FixApiIntegration.Connector fixConnector)
			{
				if (arbPos.LongPosition == null) return null;
				var result = fixConnector.SendMarketOrderRequest(set.LongSymbol, Sides.Sell, set.LongSize).Result;
				if (result?.IsFilled != true) return null;
				return result.AveragePrice;
			}
			return null;
		}
		private decimal? CloseShort(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.ShortAccount.Connector is Mt4Integration.Connector connector)
			{
				if (!arbPos.ShortTicket.HasValue) return null;
				connector.Positions.TryGetValue(arbPos.ShortTicket.Value, out var pos);
				if (pos == null) return null;
				connector.SendClosePositionRequests(pos);
				return pos.ClosePrice;
			}
			if (set.ShortAccount.Connector is FixApiIntegration.Connector fixConnector)
			{
				if (arbPos.ShortPosition == null) return null;
				var result = fixConnector.SendMarketOrderRequest(set.ShortSymbol, Sides.Buy, set.ShortSize).Result;
				if (result?.IsFilled != true) return null;
				return result.AveragePrice;
			}
			return null;
		}

		private void CheckPositions(LatencyArb set)
		{
			var longPositions = (set.LongAccount.Connector as Mt4Integration.Connector)?.Positions;
			var shortPositions = (set.ShortAccount.Connector as Mt4Integration.Connector)?.Positions;

			foreach (var pos in set.LatencyArbPositions)
			{
				if (pos.HasLong && longPositions != null && pos.LongTicket.HasValue &&
					(!longPositions.TryGetValue(pos.LongTicket.Value, out var longPos) || longPos.IsClosed))
					pos.LongClosed = true;

				if (pos.HasShort && shortPositions != null && pos.ShortTicket.HasValue &&
					(!shortPositions.TryGetValue(pos.ShortTicket.Value, out var shortPos) || shortPos.IsClosed))
					pos.ShortClosed = true;

				if (pos.LongClosed && shortPositions != null && pos.ShortTicket.HasValue &&
					(!pos.HasShort || !shortPositions.TryGetValue(pos.ShortTicket.Value, out var _)))
					pos.ShortClosed = true;
				if (pos.ShortClosed && longPositions != null && pos.LongTicket.HasValue &&
					(!pos.HasLong || !longPositions.TryGetValue(pos.LongTicket.Value, out var _)))
					pos.LongClosed = true;
			}
			set.LatencyArbPositions.RemoveAll(p => p.LongClosed && p.ShortClosed);
		}
	}
}

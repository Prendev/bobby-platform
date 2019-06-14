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
			if (set.State == LatencyArb.LatencyArbStates.Reset)
			{
				foreach (var arbPos in set.LatencyArbPositions)
				{
					arbPos.Archived = true;
					RemoveCopierPosition(set, arbPos);
				}
				set.State = LatencyArb.LatencyArbStates.None;
				return;
			}

			if (!set.HasPrices) return;
			if (set.HasTiming && IsTime(HiResDatetime.UtcNow.TimeOfDay, set.LatestTradeTime, set.EarliestTradeTime)) return;

			CheckPositions(set);
			CheckOpening(set);
			CheckReopening(set);
			CheckClosing(set);
		}

		private void CheckPositions(LatencyArb set)
		{
			var longPositions = (set.LongAccount.Connector as Mt4Integration.Connector)?.Positions;
			var shortPositions = (set.ShortAccount.Connector as Mt4Integration.Connector)?.Positions;
			var error = false;

			foreach (var pos in set.LivePositions)
			{
				var longClosed = false;
				var shortClosed = false;
				if (pos.HasLong && longPositions != null && pos.LongTicket.HasValue &&
				    (!longPositions.TryGetValue(pos.LongTicket.Value, out var longPos) || longPos.IsClosed))
					longClosed = true;

				if (pos.HasShort && shortPositions != null && pos.ShortTicket.HasValue &&
				    (!shortPositions.TryGetValue(pos.ShortTicket.Value, out var shortPos) || shortPos.IsClosed))
					shortClosed = true;

				if (longClosed && shortPositions != null && pos.ShortTicket.HasValue &&
				    (!pos.HasShort || !shortPositions.TryGetValue(pos.ShortTicket.Value, out var _)))
					shortClosed = true;
				if (shortClosed && longPositions != null && pos.LongTicket.HasValue &&
				    (!pos.HasLong || !longPositions.TryGetValue(pos.LongTicket.Value, out var _)))
					longClosed = true;

				if (shortClosed && longClosed)
				{
					pos.Archived = true;
					set.State = LatencyArb.LatencyArbStates.Error;
					error = true;
				}
			}

			if (error) Logger.Error($"{set} latency arb - unexpected closed or missing position(s)");
			Sync(set);
		}

		private void CheckOpening(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Opening &&
			    set.State != LatencyArb.LatencyArbStates.ReopeningShort &&
			    set.State != LatencyArb.LatencyArbStates.ReopeningLong) return;

			var last = set.LivePositions
				.OrderByDescending(p => p.Level)
				.FirstOrDefault(p => !p.HasLong || !p.HasShort);

			// Check for new signal
			if (last == null)
			{
				if (set.State != LatencyArb.LatencyArbStates.Opening) return;
				if (set.LivePositions.Count >= set.MaxCount) return;
				if (!set.ShortSpreadCheck) return;
				if (!set.LongSpreadCheck) return;
				// Long signal
				if (set.FirstSide != LatencyArb.LatencyArbFirstSides.Short && set.NormFeedAsk >= set.NormLongAsk + set.SignalDiffInPip * set.PipSize)
				{
					var level = set.LivePositions.Count + 1;
					var pos = SendLongOrder(set, true);
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
						LongOpenPrice = pos.OpenPrice,
						Level = level
					});
					Logger.Info($"{set} latency arb - {level}. long first side opened at {pos.OpenPrice}");
				}
				// Short signal
				else if (set.FirstSide != LatencyArb.LatencyArbFirstSides.Long && set.NormFeedBid <= set.NormShortBid - set.SignalDiffInPip * set.PipSize)
				{
					var level = set.LivePositions.Count + 1;
					var pos = SendShortOrder(set, true);
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
						ShortOpenPrice = pos.OpenPrice,
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

				if (last.Trailing.HasValue || price >= last.LongOpenPrice + set.TrailingSwitchInPip * set.PipSize)
					last.Trailing = Math.Max(price - set.TrailingDistanceInPip * set.PipSize,
						last.Trailing ?? price - set.TrailingDistanceInPip * set.PipSize);
				if (last.Trailing.HasValue && price <= last.Trailing) hedge = true;

				if (price >= last.LongOpenPrice + set.TpInPip * set.PipSize) hedge = true;
				if (price <= last.LongOpenPrice - set.SlInPip * set.PipSize) hedge = true;
				if (!hedge) return;
				
				var pos = SendShortOrder(set, false);
				if (pos == null)
				{
					Logger.Warn($"{set} latency arb - {last.Level}. short hedge side open error");
					return;
				}
				last.ShortTicket = pos.Ticket;
				last.ShortPosition = pos.StratPosition;
				last.ShortOpenPrice = pos.OpenPrice;
				AddCopierPosition(set, last);
				Logger.Info($"{set} latency arb - {last.Level}. short hedge side opened at {pos.OpenPrice} with {(pos.OpenPrice - last.LongOpenPrice)/set.PipSize} pips");
				// Switch state if rotating
				if (set.Rotating && set.State == LatencyArb.LatencyArbStates.Opening &&
				    set.LivePositions.Count >= set.MaxCount)
					set.State = LatencyArb.LatencyArbStates.Closing;
			}
			// Short side opened
			else if (last.HasShort)
			{
				var hedge = false;
				var price = set.LastLongTick.Ask;

				if (last.Trailing.HasValue || price <= last.ShortOpenPrice - set.TrailingSwitchInPip * set.PipSize)
					last.Trailing = Math.Min(price + set.TrailingDistanceInPip * set.PipSize,
						last.Trailing ?? price + set.TrailingDistanceInPip * set.PipSize);
				if (last.Trailing.HasValue && price >= last.Trailing) hedge = true;

				if (price <= last.ShortOpenPrice - set.TpInPip * set.PipSize) hedge = true;
				if (price >= last.ShortOpenPrice + set.SlInPip * set.PipSize) hedge = true;
				if (!hedge) return;
				
				var pos = SendLongOrder(set, false);
				if (pos == null)
				{
					Logger.Warn($"{set} latency arb - {last.Level}. long hedge side open error");
					return;
				}
				last.LongTicket = pos.Ticket;
				last.LongPosition = pos.StratPosition;
				last.LongOpenPrice = pos.OpenPrice;
				AddCopierPosition(set, last);
				Logger.Info($"{set} latency arb - {last.Level}. long hedge side opened at {pos.OpenPrice} with {(last.ShortOpenPrice - pos.OpenPrice) / set.PipSize} pips");
				// Switch state if rotating
				if (set.Rotating && set.State == LatencyArb.LatencyArbStates.Opening &&
				    set.LivePositions.Count >= set.MaxCount)
					set.State = LatencyArb.LatencyArbStates.Closing;
			}
		}
		private OpenResult SendLongOrder(LatencyArb set, bool isFirst)
		{
			if (set.LongAccount.Connector is Mt4Integration.Connector connector)
			{
				var pos = connector.SendMarketOrderRequest(set.LongSymbol, Sides.Buy, (double)set.LongSize, 0, null);
				return pos == null ? null : new OpenResult {Ticket = pos.Id, OpenPrice = pos.OpenPrice};
			}

			if (set.LongAccount.Connector is FixApiIntegration.Connector fixConnector)
			{
				OrderResponse result = null;
				if (!isFirst || set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Market)
					result = fixConnector.SendMarketOrderRequest(set.LongSymbol, Sides.Buy, set.LongSize).Result;
				else if (set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive)
					result = fixConnector.SendAggressiveOrderRequest(set.LongSymbol, Sides.Buy, set.LongSize, set.LastLongTick.Ask,
						set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;

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
						Symbol = set.LongSymbol,
						Size = result.FilledQuantity
					}
				};
			}

			return null;
		}
		private OpenResult SendShortOrder(LatencyArb set, bool isFirst)
		{
			if (set.ShortAccount.Connector is Mt4Integration.Connector connector)
			{
				var pos = connector.SendMarketOrderRequest(set.ShortSymbol, Sides.Sell, (double)set.ShortSize, 0, null);
				return pos == null ? null : new OpenResult {Ticket = pos.Id, OpenPrice = pos.OpenPrice};
			}

			if (set.ShortAccount.Connector is FixApiIntegration.Connector fixConnector)
			{
				OrderResponse result = null;
				if (!isFirst || set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Market)
					result = fixConnector.SendMarketOrderRequest(set.ShortSymbol, Sides.Sell, set.ShortSize).Result;
				else if (set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive)
					result = fixConnector.SendAggressiveOrderRequest(set.ShortSymbol, Sides.Sell, set.ShortSize, set.LastShortTick.Bid,
						set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;

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
						Symbol = set.ShortSymbol,
						Size = result.FilledQuantity
					}
				};
			}

			return null;
		}

		private void CheckReopening(LatencyArb set)
		{
			if (set.ReopenCount <= 0) return;
			if (set.LivePositions.Any(p => !p.HasShort || !p.HasLong)) return;
			CheckReopeningShort(set);
			CheckReopeningLong(set);
		}
		private void CheckReopeningShort(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.ReopeningShort) return;
			if (!set.ShortSpreadCheck) return;

			LatencyArbPosition first = null;
			var positions = set.LivePositions.OrderBy(p => p.Level).Where(p => p.HasBothSides);
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
			if (set.NormFeedAsk < set.NormShortAsk + set.SignalDiffInPip * set.PipSize) return;

			if (set.Copier != null) set.Copier.Run = false;
			var closePrice = CloseShort(set, first, true);
			if (closePrice.HasValue) RemoveCopierPosition(set, first);
			if (set.Copier != null) set.Copier.Run = true;
			if (!closePrice.HasValue) return;

			set.ReopenCount--;
			first.ShortOpenPrice = closePrice;
			first.Trailing = null;
			first.ShortTicket = null;
			first.ShortPosition = null;
			Logger.Info($"{set} latency arb - {first.Level}. short side closed for reopening at {closePrice}");
		}
		private void CheckReopeningLong(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.ReopeningLong) return;
			if (!set.LongSpreadCheck) return;

			LatencyArbPosition first = null;
			var positions = set.LivePositions.OrderBy(p => p.Level).Where(p => p.HasBothSides);
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
			if (set.NormFeedBid > set.NormLongBid - set.SignalDiffInPip * set.PipSize) return;

			if (set.Copier != null) set.Copier.Run = false;
			var closePrice = CloseLong(set, first, true);
			if (closePrice.HasValue) RemoveCopierPosition(set, first);
			if (set.Copier != null) set.Copier.Run = true;
			if (!closePrice.HasValue) return;

			set.ReopenCount--;
			first.LongOpenPrice = closePrice;
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

			var first = set.LivePositions.OrderBy(p => p.Level).FirstOrDefault(p => p.HasBothSides);
			if (first == null) return;
			if (!first.HasBothSides) return;

			// No closed side
			if (!first.LongClosed && !first.ShortClosed)
			{
				if (!set.ShortSpreadCheck) return;
				if (!set.LongSpreadCheck) return;
				// Long close signal
				if (set.FirstSide != LatencyArb.LatencyArbFirstSides.Short && set.NormFeedBid <= set.NormLongBid - set.SignalDiffInPip * set.PipSize)
				{
					if (set.Copier != null) set.Copier.Run = false;
					var closePrice = CloseLong(set, first, true);
					if (closePrice.HasValue) RemoveCopierPosition(set, first);
					if (set.Copier != null) set.Copier.Run = true;
					if (!closePrice.HasValue) return;

					first.LongClosePrice = closePrice;
					first.Trailing = null;
					Logger.Info($"{set} latency arb - {first.Level}. long first side closed at {closePrice}");
				}
				// Short close signal
				else if (set.FirstSide != LatencyArb.LatencyArbFirstSides.Long && set.NormFeedAsk >= set.NormShortAsk + set.SignalDiffInPip * set.PipSize)
				{
					if (set.Copier != null) set.Copier.Run = false;
					var closePrice = CloseShort(set, first, true);
					if (closePrice.HasValue) RemoveCopierPosition(set, first);
					if (set.Copier != null) set.Copier.Run = true;
					if (!closePrice.HasValue) return;

					first.ShortClosePrice = closePrice;
					first.Trailing = null;
					Logger.Info($"{set} latency arb - {first.Level}. short first side closed at {closePrice}");
				}
			}
			// Long side closed
			else if (first.LongClosed)
			{
				var hedge = false;
				var price = set.LastShortTick.Ask;

				if (first.Trailing.HasValue || price <= first.LongClosePrice - set.TrailingSwitchInPip * set.PipSize)
					first.Trailing = Math.Min(price + set.TrailingDistanceInPip * set.PipSize,
						first.Trailing ?? price + set.TrailingDistanceInPip * set.PipSize);
				if (first.Trailing.HasValue && price >= first.Trailing) hedge = true;

				if (price <= first.LongClosePrice - set.TpInPip * set.PipSize) hedge = true;
				if (price >= first.LongClosePrice + set.SlInPip * set.PipSize) hedge = true;
				if (!hedge) return;

				var closePrice = CloseShort(set, first, false);
				if (!closePrice.HasValue) return;
				first.ShortClosePrice = closePrice;
				first.Archived = true;
				Logger.Info($"{set} latency arb - {first.Level}. short hedge side closed at {closePrice} with {(first.LongClosePrice - closePrice) /set.PipSize} pips");
				// Switch state if rotating
				if (set.Rotating && set.State == LatencyArb.LatencyArbStates.Closing &&
				    set.LatencyArbPositions.Count == 0)
					set.State = LatencyArb.LatencyArbStates.Opening;
			}
			// Short side closed
			else if (first.ShortClosed)
			{
				var hedge = false;
				var price = set.LastLongTick.Bid;

				if (first.Trailing.HasValue || price >= first.ShortClosePrice + set.TrailingSwitchInPip * set.PipSize)
					first.Trailing = Math.Max(price - set.TrailingDistanceInPip * set.PipSize,
						first.Trailing ?? price - set.TrailingDistanceInPip * set.PipSize);
				if (first.Trailing.HasValue && price <= first.Trailing) hedge = true;

				if (price >= first.ShortClosePrice + set.TpInPip * set.PipSize) hedge = true;
				if (price <= first.ShortClosePrice - set.SlInPip * set.PipSize) hedge = true;
				if (!hedge) return;

				var closePrice = CloseLong(set, first, false);
				if (!closePrice.HasValue) return;
				first.LongClosePrice = closePrice;
				first.Archived = true;
				Logger.Info($"{set} latency arb - {first.Level}. long hedge side closed at {closePrice} with {(closePrice - first.ShortClosePrice) / set.PipSize} pips");
				// Switch state if rotating
				if (set.Rotating && set.State == LatencyArb.LatencyArbStates.Closing &&
				    set.LatencyArbPositions.Count == 0)
					set.State = LatencyArb.LatencyArbStates.Opening;
			}
		}
		private decimal? CloseLong(LatencyArb set, LatencyArbPosition arbPos, bool isFirst)
		{
			if (set.LongAccount.Connector is Mt4Integration.Connector connector)
			{
				if (!arbPos.LongTicket.HasValue) return null;
				connector.Positions.TryGetValue(arbPos.LongTicket.Value, out var pos);
				if (pos == null) return null;
				connector.SendClosePositionRequests(pos);
				return pos.IsClosed ? pos.ClosePrice : (decimal?) null;
			}
			if (set.LongAccount.Connector is FixApiIntegration.Connector fixConnector && arbPos.LongPosition != null)
			{
				OrderResponse result = null;
				if (!isFirst || set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Market)
					result = fixConnector.SendMarketOrderRequest(set.LongSymbol, Sides.Sell, arbPos.LongPosition.Size).Result;
				else if (set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive)
					result = fixConnector.SendAggressiveOrderRequest(set.LongSymbol, Sides.Sell, arbPos.LongPosition.Size, set.LastLongTick.Bid,
						set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;

				if (result?.IsFilled != true) return null;
				return result.AveragePrice;
			}
			return null;
		}
		private decimal? CloseShort(LatencyArb set, LatencyArbPosition arbPos, bool isFirst)
		{
			if (set.ShortAccount.Connector is Mt4Integration.Connector connector)
			{
				if (!arbPos.ShortTicket.HasValue) return null;
				connector.Positions.TryGetValue(arbPos.ShortTicket.Value, out var pos);
				if (pos == null) return null;
				connector.SendClosePositionRequests(pos);
				return pos.IsClosed ? pos.ClosePrice : (decimal?)null;
			}
			if (set.ShortAccount.Connector is FixApiIntegration.Connector fixConnector && arbPos.ShortPosition != null)
			{
				OrderResponse result = null;
				if (!isFirst || set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Market)
					result = fixConnector.SendMarketOrderRequest(set.ShortSymbol, Sides.Buy, arbPos.ShortPosition.Size).Result;
				else if (set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive)
					result = fixConnector.SendAggressiveOrderRequest(set.ShortSymbol, Sides.Buy, arbPos.ShortPosition.Size, set.LastShortTick.Ask,
						set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;

				if (result?.IsFilled != true) return null;
				return result.AveragePrice;
			}
			return null;
		}

		private void Sync(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Sync) return;
			var longPositions = (set.LongAccount.Connector as Mt4Integration.Connector)?.Positions;
			var shortPositions = (set.ShortAccount.Connector as Mt4Integration.Connector)?.Positions;
			if (longPositions == null) return;
			if (shortPositions == null) return;

			foreach (var longPos in longPositions)
			{
				if (longPos.Value.Side != Sides.Buy) continue;
				if (longPos.Value.Lots != set.LongSize) continue;
				if (longPos.Value.Symbol != set.LongSymbol) continue;
				if (set.LivePositions.Any(p => p.LongTicket == longPos.Key)) continue;

				KeyValuePair<long, Position>? match = null;
				foreach (var shortPos in shortPositions)
				{
					if (shortPos.Value.Side != Sides.Sell) continue;
					if (shortPos.Value.Lots != set.ShortSize) continue;
					if (shortPos.Value.Symbol != set.ShortSymbol) continue;
					if (set.LivePositions.Any(p => p.ShortTicket == shortPos.Key)) continue;
					match = shortPos;
					break;
				}

				if (!match.HasValue) break;
				var level = set.LivePositions.Count + 1;
				var arbPos = new LatencyArbPosition()
				{
					LatencyArb = set,
					LongTicket = longPos.Key,
					ShortTicket = match.Value.Key,
					Level = level
				};
				set.LatencyArbPositions.Add(arbPos);
			}
			foreach (var arbPos in set.LivePositions)
				AddCopierPosition(set, arbPos);

			set.State = LatencyArb.LatencyArbStates.None;
		}
		private void AddCopierPosition(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.Copier == null) return;
			if (!arbPos.LongTicket.HasValue) return;
			if (!arbPos.ShortTicket.HasValue) return;
			if (set.Copier.Slave.Account != set.LongAccount &&
			    set.Copier.Slave.Account != set.ShortAccount) return;
			if (set.Copier.Slave.Master.Account != set.LongAccount &&
			    set.Copier.Slave.Master.Account != set.ShortAccount) return;

			if (set.Copier.Slave.Account == set.LongAccount)
			{
				if (set.Copier.CopierPositions.Any(cp =>
					cp.SlaveTicket == arbPos.LongTicket || cp.MasterTicket == arbPos.ShortTicket)) return;
				set.Copier.CopierPositions.Add(new CopierPosition()
				{
					Copier = set.Copier,
					MasterTicket = arbPos.ShortTicket.Value,
					SlaveTicket = arbPos.LongTicket.Value
				});
			}
			else if (set.Copier.Slave.Account == set.ShortAccount)
			{
				if (set.Copier.CopierPositions.Any(cp =>
					cp.SlaveTicket == arbPos.ShortTicket || cp.MasterTicket == arbPos.LongTicket)) return;
				set.Copier.CopierPositions.Add(new CopierPosition()
				{
					Copier = set.Copier,
					MasterTicket = arbPos.LongTicket.Value,
					SlaveTicket = arbPos.ShortTicket.Value
				});
			}
		}
		private void RemoveCopierPosition(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.Copier == null) return;
			if (!arbPos.LongTicket.HasValue) return;
			if (!arbPos.ShortTicket.HasValue) return;
			if (set.Copier.Slave.Account != set.LongAccount &&
			    set.Copier.Slave.Account != set.ShortAccount) return;
			if (set.Copier.Slave.Master.Account != set.LongAccount &&
			    set.Copier.Slave.Master.Account != set.ShortAccount) return;

			if (set.Copier.Slave.Account == set.LongAccount)
				set.Copier.CopierPositions.RemoveAll(cp =>
					cp.SlaveTicket == arbPos.LongTicket && cp.MasterTicket == arbPos.ShortTicket);
			else if (set.Copier.Slave.Account == set.ShortAccount)
				set.Copier.CopierPositions.RemoveAll(cp =>
					cp.SlaveTicket == arbPos.ShortTicket && cp.MasterTicket == arbPos.LongTicket);
		}

		private bool IsTime(TimeSpan current, TimeSpan? start, TimeSpan? end)
		{
			if (!start.HasValue || !end.HasValue) return false;
			var startOk = current >= start;
			var endOk = current < end;

			if (end < start) return startOk || endOk;
			return startOk && endOk;
		}
	}
}

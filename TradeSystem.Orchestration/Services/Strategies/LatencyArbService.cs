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
			public decimal Slippage { get; set; }
			public long ExecutionTime { get; set; }
		}

		private class CloseResult
		{
			public decimal ClosePrice { get; set; }
			public decimal Slippage { get; set; }
			public long ExecutionTime { get; set; }
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

			//set.State = LatencyArb.LatencyArbStates.None;
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
			EmergencyImmediateExit(set);
			Emergency(set);
			Reset(set);
			Sync(set);
			if (set.State == LatencyArb.LatencyArbStates.None) return;

			if (!set.HasPrices) return;
			if (set.HasTiming && IsTime(HiResDatetime.UtcNow.TimeOfDay, set.LatestTradeTime, set.EarliestTradeTime)) return;
			if (HiResDatetime.UtcNow - set.LastFeedTick.Time > TimeSpan.FromSeconds(Math.Max(60, set.AveragingPeriodInSeconds))) return;
			if (HiResDatetime.UtcNow - set.LastLongTick.Time > TimeSpan.FromSeconds(Math.Max(60, set.AveragingPeriodInSeconds))) return;
			if (HiResDatetime.UtcNow - set.LastShortTick.Time > TimeSpan.FromSeconds(Math.Max(60, set.AveragingPeriodInSeconds))) return;

			CheckPositions(set);
			CheckOpening(set);
			CheckReopening(set);
			CheckClosing(set);
		}

		private void CheckPositions(LatencyArb set)
		{
			var longPositions = (set.LongAccount.Connector as Mt4Integration.Connector)?.Positions;
			var shortPositions = (set.ShortAccount.Connector as Mt4Integration.Connector)?.Positions;

			foreach (var pos in set.LivePositions)
			{
				var posError = false;
				{
					if (!pos.ShortOpenPrice.HasValue)
					{
						pos.ShortOpenPrice = pos.ShortPosition?.AvgPrice;
						if (pos.ShortTicket.HasValue && shortPositions != null &&
						    shortPositions.TryGetValue(pos.ShortTicket.Value, out var shortPos))
						{
							pos.ShortOpenPrice = shortPos.OpenPrice;
							if (!pos.ShortClosed && shortPos.IsClosed)
							{
								pos.Archived = true;
								set.State = LatencyArb.LatencyArbStates.Error;
								posError = true;
							}
						}
					}

					if (!pos.LongOpenPrice.HasValue)
					{
						pos.LongOpenPrice = pos.LongPosition?.AvgPrice;
						if (pos.LongTicket.HasValue && longPositions != null &&
						    longPositions.TryGetValue(pos.LongTicket.Value, out var longPos))
						{
							pos.LongOpenPrice = longPos.OpenPrice;
							if (!pos.LongClosed && longPos.IsClosed)
							{
								pos.Archived = true;
								set.State = LatencyArb.LatencyArbStates.Error;
								posError = true;
							}
						}
					}
				}
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
						posError = true;
					}
				}
				if (!posError) continue;
				Logger.Error($"{set} latency arb - {pos.Level}. - unexpected closed or missing position(s)");
			}
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
				if (set.FirstSide != LatencyArb.LatencyArbFirstSides.Short && set.NormFeedAsk >= set.NormLongAsk + set.LongSignalDiffInPip * set.PipSize)
				{
					var level = set.LivePositions.Count + 1;
					if (set.Copier != null) set.Copier.Run = false;
					if (set.FixApiCopier != null) set.FixApiCopier.Run = false;
					var pos = SendLongOrder(set, set.LongSize, true);
					if (set.Copier != null) set.Copier.Run = true;
					if (set.FixApiCopier != null) set.FixApiCopier.Run = true;
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
					Logger.Info($"{set} latency arb - {level}. long first side opened at {pos.OpenPrice}" +
					            $"{Environment.NewLine}\tExecution time is {pos.ExecutionTime} ms with {pos.Slippage / set.PipSize:F2} pip slippage");
				}
				// Short signal
				else if (set.FirstSide != LatencyArb.LatencyArbFirstSides.Long && set.NormFeedBid <= set.NormShortBid - set.ShortSignalDiffInPip * set.PipSize)
				{
					var level = set.LivePositions.Count + 1;
					if (set.Copier != null) set.Copier.Run = false;
					if (set.FixApiCopier != null) set.FixApiCopier.Run = false;
					var pos = SendShortOrder(set, set.ShortSize, true);
					if (set.Copier != null) set.Copier.Run = true;
					if (set.FixApiCopier != null) set.FixApiCopier.Run = true;
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
					Logger.Info($"{set} latency arb - {level}. short first side opened at {pos.OpenPrice}" +
					            $"{Environment.NewLine}\tExecution time is {pos.ExecutionTime} ms with {pos.Slippage / set.PipSize:F2} pip slippage");
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

				if (set.Copier != null) set.Copier.Run = false;
				if (set.FixApiCopier != null) set.FixApiCopier.Run = false;
				var quantity = (last.LongPosition?.Size ?? set.LongSize) / set.LongSize * set.ShortSize;
				var pos = SendShortOrder(set, quantity, false);
				if (set.Copier != null) set.Copier.Run = true;
				if (set.FixApiCopier != null) set.FixApiCopier.Run = true;
				if (pos == null)
				{
					Logger.Warn($"{set} latency arb - {last.Level}. short hedge side open error");
					return;
				}
				last.ShortTicket = pos.Ticket;
				last.ShortPosition = pos.StratPosition;
				last.ShortOpenPrice = pos.OpenPrice;
				AddCopierPosition(set, last);
				var pip = (pos.OpenPrice - last.LongOpenPrice) / set.PipSize;
				if (pip < set.EmergencyOpenThresholdInPip && set.EmergencyOff > 0) set.EmergencyCount++;
				else set.EmergencyCount = 0;
				Logger.Info($"{set} latency arb - {last.Level}. short hedge side opened at {pos.OpenPrice} with {pip} pips" +
				            $"{Environment.NewLine}\tExecution time is {pos.ExecutionTime} ms with {pos.Slippage / set.PipSize:F2} pip slippage");
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

				if (set.Copier != null) set.Copier.Run = false;
				if (set.FixApiCopier != null) set.FixApiCopier.Run = false;
				var quantity = (last.ShortPosition?.Size ?? set.ShortSize) / set.ShortSize * set.LongSize;
				var pos = SendLongOrder(set, quantity, false);
				if (set.Copier != null) set.Copier.Run = true;
				if (set.FixApiCopier != null) set.FixApiCopier.Run = true;
				if (pos == null)
				{
					Logger.Warn($"{set} latency arb - {last.Level}. long hedge side open error");
					return;
				}
				last.LongTicket = pos.Ticket;
				last.LongPosition = pos.StratPosition;
				last.LongOpenPrice = pos.OpenPrice;
				AddCopierPosition(set, last);
				var pip = (last.ShortOpenPrice - pos.OpenPrice) / set.PipSize;
				if (pip < set.EmergencyOpenThresholdInPip && set.EmergencyOff > 0) set.EmergencyCount++;
				else set.EmergencyCount = 0;
				Logger.Info($"{set} latency arb - {last.Level}. long hedge side opened at {pos.OpenPrice} with {pip} pips" +
				            $"{Environment.NewLine}\tExecution time is {pos.ExecutionTime} ms with {pos.Slippage/set.PipSize:F2} pip slippage");
				// Switch state if rotating
				if (set.Rotating && set.State == LatencyArb.LatencyArbStates.Opening &&
				    set.LivePositions.Count >= set.MaxCount)
					set.State = LatencyArb.LatencyArbStates.Closing;
			}
		}

		private OpenResult SendLongOrder(LatencyArb set, decimal quantity, bool isFirst)
		{
			var signalPrice = set.LastLongTick.Ask;
			set.Stopwatch.Restart();
			try
			{
				if (set.LongAccount.Connector is Mt4Integration.Connector connector)
				{
					var pos = connector.SendMarketOrderRequest(set.LongSymbol, Sides.Buy, (double) quantity, 0, set.Comment);
					CheckUnfinished(set, pos);
					if (pos?.Pos == null) return null;
					return new OpenResult
					{
						Slippage = signalPrice - pos.Pos.OpenPrice,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						Ticket = pos.Pos.Id,
						OpenPrice = pos.Pos.OpenPrice
					};
				}

				if (set.LongAccount.Connector is FixApiIntegration.Connector fixConnector)
				{
					OrderResponse result = null;
					if (isFirst && set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Market ||
					    !isFirst && set.HedgeOrderType == LatencyArb.LatencyArbOrderTypes.Market)
						result = fixConnector.SendMarketOrderRequest(set.LongSymbol, Sides.Buy, quantity, set.MarketTimeWindowInMs, 0, 0).Result;

					else if (isFirst && set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive ||
					         !isFirst && set.HedgeOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive)
					{
						result = fixConnector.SendAggressiveOrderRequest(set.LongSymbol, Sides.Buy, quantity,
							set.LastLongTick.Ask, set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;
						CheckUnfinished(set, result);
						if (!isFirst && result?.FilledQuantity != quantity)
						{
							var fq = quantity - (result?.FilledQuantity ?? 0);
							var fallbackResult = fixConnector.SendMarketOrderRequest(set.LongSymbol, Sides.Buy, fq, set.MarketTimeWindowInMs, 0, 0).Result;
							CheckUnfinished(set, fallbackResult);
							if (result?.IsFilled != true) result = fallbackResult;
							else if (fallbackResult?.IsFilled == true)
							{
								result.AveragePrice =
									(result.AveragePrice * result.FilledQuantity + fallbackResult.AveragePrice * fallbackResult.FilledQuantity) /
									(result.FilledQuantity + fallbackResult.FilledQuantity);
								result.FilledQuantity += fallbackResult.FilledQuantity;
							}
						}
					}
					CheckUnfinished(set, result);
					if (result?.IsFilled != true) return null;
					return new OpenResult
					{
						Slippage = signalPrice - result.AveragePrice.Value,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
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
			}
			finally
			{
				set.Stopwatch.Stop();
			}

			return null;
		}

		private OpenResult SendShortOrder(LatencyArb set, decimal quantity, bool isFirst)
		{
			var signalPrice = set.LastShortTick.Bid;
			set.Stopwatch.Restart();
			try
			{
				if (set.ShortAccount.Connector is Mt4Integration.Connector connector)
				{
					var pos = connector.SendMarketOrderRequest(set.ShortSymbol, Sides.Sell, (double) quantity, 0, set.Comment);
					CheckUnfinished(set, pos);
					if (pos?.Pos == null) return null;
					return new OpenResult
					{
						Slippage = pos.Pos.OpenPrice - signalPrice,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						Ticket = pos.Pos.Id,
						OpenPrice = pos.Pos.OpenPrice
					};
				}

				if (set.ShortAccount.Connector is FixApiIntegration.Connector fixConnector)
				{
					OrderResponse result = null;
					if (isFirst && set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Market ||
					    !isFirst && set.HedgeOrderType == LatencyArb.LatencyArbOrderTypes.Market)
						result = fixConnector.SendMarketOrderRequest(set.ShortSymbol, Sides.Sell, quantity, set.MarketTimeWindowInMs, 0, 0).Result;

					else if (isFirst && set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive ||
					         !isFirst && set.HedgeOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive)
					{
						result = fixConnector.SendAggressiveOrderRequest(set.ShortSymbol, Sides.Sell, quantity,
							set.LastShortTick.Bid, set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;
						CheckUnfinished(set, result);
						if (!isFirst && result?.FilledQuantity != quantity)
						{
							var fq = quantity - (result?.FilledQuantity ?? 0);
							var fallbackResult = fixConnector.SendMarketOrderRequest(set.ShortSymbol, Sides.Sell, fq, set.MarketTimeWindowInMs, 0, 0).Result;
							CheckUnfinished(set, fallbackResult);
							if (result?.IsFilled != true) result = fallbackResult;
							else if (fallbackResult?.IsFilled == true)
							{
								result.AveragePrice =
									(result.AveragePrice * result.FilledQuantity + fallbackResult.AveragePrice * fallbackResult.FilledQuantity) /
									(result.FilledQuantity + fallbackResult.FilledQuantity);
								result.FilledQuantity += fallbackResult.FilledQuantity;
							}
						}
					}
					CheckUnfinished(set, result);
					if (result?.IsFilled != true) return null;
					return new OpenResult
					{
						Slippage = result.AveragePrice.Value - signalPrice,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
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
			}
			finally
			{
				set.Stopwatch.Stop();
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
				// TODO end of year
				if (HiResDatetime.UtcNow.DayOfYear - openTime.Value.DayOfYear < set.ReopenThresholdInDay) continue;
				first = position;
				break;
			}
			if (first == null) return;
			if (!first.HasBothSides) return;
			if (set.NormFeedAsk < set.NormShortAsk + set.ShortSignalDiffInPip * set.PipSize) return;

			if (set.Copier != null) set.Copier.Run = false;
			if (set.FixApiCopier != null) set.FixApiCopier.Run = false;
			var closePos = CloseShort(set, first, true);
			if (closePos != null) RemoveCopierPosition(set, first);
			if (set.Copier != null) set.Copier.Run = true;
			if (set.FixApiCopier != null) set.FixApiCopier.Run = true;
			if (closePos == null) return;

			set.ReopenCount--;
			first.LongOpenPrice = closePos.ClosePrice;
			first.Trailing = null;
			first.ShortTicket = null;
			first.ShortPosition = null;
			first.ShortOpenPrice = null;
			Logger.Info($"{set} latency arb - {first.Level}. short side closed for reopening at {closePos.ClosePrice}" +
			            $"{Environment.NewLine}\tExecution time is {closePos.ExecutionTime} ms with {closePos.Slippage / set.PipSize:F2} pip slippage");
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
				// TODO end of year
				if (HiResDatetime.UtcNow.DayOfYear - openTime.Value.DayOfYear < set.ReopenThresholdInDay) continue;
				first = position;
				break;
			}
			if (first == null) return;
			if (!first.HasBothSides) return;
			if (set.NormFeedBid > set.NormLongBid - set.LongSignalDiffInPip * set.PipSize) return;

			if (set.Copier != null) set.Copier.Run = false;
			if (set.FixApiCopier != null) set.FixApiCopier.Run = false;
			var closePos = CloseLong(set, first, true);
			if (closePos != null) RemoveCopierPosition(set, first);
			if (set.Copier != null) set.Copier.Run = true;
			if (set.FixApiCopier != null) set.FixApiCopier.Run = true;
			if (closePos == null) return;

			set.ReopenCount--;
			first.ShortOpenPrice = closePos.ClosePrice;
			first.Trailing = null;
			first.LongTicket = null;
			first.LongPosition = null;
			first.LongOpenPrice = null;
			Logger.Info($"{set} latency arb - {first.Level}. long side closed for reopening at {closePos.ClosePrice}" +
			            $"{Environment.NewLine}\tExecution time is {closePos.ExecutionTime} ms with {closePos.Slippage / set.PipSize:F2} pip slippage");
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
			if (set.State != LatencyArb.LatencyArbStates.Closing &&
			    set.State != LatencyArb.LatencyArbStates.ImmediateExit) return;

			var first = set.LivePositions.OrderBy(p => p.Level)
				.FirstOrDefault(p => p.HasBothSides || set.State == LatencyArb.LatencyArbStates.ImmediateExit);
			if (first == null) return;

			// No closed side
			if (!first.LongClosed && !first.ShortClosed)
			{
				if (set.State != LatencyArb.LatencyArbStates.ImmediateExit && !set.ShortSpreadCheck) return;
				if (set.State != LatencyArb.LatencyArbStates.ImmediateExit && !set.LongSpreadCheck) return;
				// Long close signal
				if (set.FirstSide != LatencyArb.LatencyArbFirstSides.Short &&
				    (set.State == LatencyArb.LatencyArbStates.ImmediateExit ||
				     set.NormFeedBid <= set.NormLongBid - set.LongCloseSignalDiffInPip * set.PipSize))
				{
					if (set.Copier != null) set.Copier.Run = false;
					if (set.FixApiCopier != null) set.FixApiCopier.Run = false;
					var closePos = CloseLong(set, first, true);
					if (closePos != null) RemoveCopierPosition(set, first);
					if (set.Copier != null) set.Copier.Run = true;
					if (set.FixApiCopier != null) set.FixApiCopier.Run = true;
					if (closePos == null) return;

					first.LongClosePrice = closePos.ClosePrice;
					first.Trailing = null;
					Logger.Info($"{set} latency arb - {first.Level}. long first side closed at {closePos.ClosePrice}" +
					            $"{Environment.NewLine}\tExecution time is {closePos.ExecutionTime} ms with {closePos.Slippage / set.PipSize:F2} pip slippage");
				}
				// Short close signal
				else if (set.FirstSide != LatencyArb.LatencyArbFirstSides.Long &&
				         (set.State == LatencyArb.LatencyArbStates.ImmediateExit ||
				          set.NormFeedAsk >= set.NormShortAsk + set.ShortCloseSignalDiffInPip * set.PipSize))
				{
					if (set.Copier != null) set.Copier.Run = false;
					if (set.FixApiCopier != null) set.FixApiCopier.Run = false;
					var closePos = CloseShort(set, first, true);
					if (closePos != null) RemoveCopierPosition(set, first);
					if (set.Copier != null) set.Copier.Run = true;
					if (set.FixApiCopier != null) set.FixApiCopier.Run = true;
					if (closePos == null) return;

					first.ShortClosePrice = closePos.ClosePrice;
					first.Trailing = null;
					Logger.Info($"{set} latency arb - {first.Level}. short first side closed at {closePos.ClosePrice}" +
					            $"{Environment.NewLine}\tExecution time is {closePos.ExecutionTime} ms with {closePos.Slippage / set.PipSize:F2} pip slippage");
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
				if (set.State != LatencyArb.LatencyArbStates.ImmediateExit && !hedge) return;

				var closePos = CloseShort(set, first, false);
				if (closePos == null) return;
				first.ShortClosePrice = closePos.ClosePrice;
				first.Archived = true;
				var pip = (first.LongClosePrice - closePos.ClosePrice) / set.PipSize;
				if (pip < set.EmergencyOpenThresholdInPip && set.EmergencyOff > 0) set.EmergencyCount++;
				else set.EmergencyCount = 0;
				Logger.Info($"{set} latency arb - {first.Level}. short hedge side closed at {closePos.ClosePrice} with {pip} pips" +
				            $"{Environment.NewLine}\tExecution time is {closePos.ExecutionTime} ms with {closePos.Slippage / set.PipSize:F2} pip slippage");
				// Switch state if rotating
				if (set.Rotating && set.State == LatencyArb.LatencyArbStates.Closing &&
				    set.LivePositions.Count == 0)
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
				if (set.State != LatencyArb.LatencyArbStates.ImmediateExit && !hedge) return;

				var closePos = CloseLong(set, first, false);
				if (closePos == null) return;
				first.LongClosePrice = closePos.ClosePrice;
				first.Archived = true;
				var pip = (closePos.ClosePrice - first.ShortClosePrice) / set.PipSize;
				if (pip < set.EmergencyOpenThresholdInPip && set.EmergencyOff > 0) set.EmergencyCount++;
				else set.EmergencyCount = 0;
				Logger.Info($"{set} latency arb - {first.Level}. long hedge side closed at {closePos.ClosePrice} with {pip} pips" +
				            $"{Environment.NewLine}\tExecution time is {closePos.ExecutionTime} ms with {closePos.Slippage / set.PipSize:F2} pip slippage");
				// Switch state if rotating
				if (set.Rotating && set.State == LatencyArb.LatencyArbStates.Closing &&
				    set.LivePositions.Count == 0)
					set.State = LatencyArb.LatencyArbStates.Opening;
			}
		}

		private CloseResult CloseLong(LatencyArb set, LatencyArbPosition arbPos, bool isFirst)
		{
			var signalPrice = set.LastLongTick.Bid;
			set.Stopwatch.Restart();
			try
			{
				if (set.LongAccount.Connector is Mt4Integration.Connector connector)
				{
					if (!arbPos.LongTicket.HasValue) return null;
					connector.Positions.TryGetValue(arbPos.LongTicket.Value, out var pos);
					if (pos == null || pos.IsClosed)
					{
						arbPos.Archived = true;
						set.State = LatencyArb.LatencyArbStates.Error;
						Logger.Error($"{set} latency arb - {arbPos.Level}. - unexpected closed or missing position(s)");
						return null;
					}
					connector.SendClosePositionRequests(pos);
					if (!pos.IsClosed) return null;
					return new CloseResult
					{
						Slippage = pos.ClosePrice - signalPrice,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						ClosePrice = pos.ClosePrice
					};
				}

				if (set.LongAccount.Connector is FixApiIntegration.Connector fixConnector && arbPos.LongPosition != null)
				{
					OrderResponse result = null;
					if (isFirst && set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Market ||
					    !isFirst && set.HedgeOrderType == LatencyArb.LatencyArbOrderTypes.Market)
						result = fixConnector.SendMarketOrderRequest(set.LongSymbol, Sides.Sell, arbPos.LongPosition.Size, set.MarketTimeWindowInMs, 0, 0).Result;

					else if (isFirst && set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive ||
					         !isFirst && set.HedgeOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive)
					{
						result = fixConnector.SendAggressiveOrderRequest(set.LongSymbol, Sides.Sell, arbPos.LongPosition.Size,
							set.LastLongTick.Bid, set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;
						CheckUnfinished(set, result);
						if (result?.FilledQuantity != arbPos.LongPosition.Size)
						{
							var quantity = arbPos.LongPosition.Size - (result?.FilledQuantity ?? 0);
							var fallbackResult = fixConnector.SendMarketOrderRequest(set.LongSymbol, Sides.Sell, quantity, set.MarketTimeWindowInMs, 0, 0).Result;
							CheckUnfinished(set, fallbackResult);
							if (result?.IsFilled != true) result = fallbackResult;
							else if (fallbackResult?.IsFilled == true)
							{
								result.AveragePrice =
									(result.AveragePrice * result.FilledQuantity + fallbackResult.AveragePrice * fallbackResult.FilledQuantity) /
									(result.FilledQuantity + fallbackResult.FilledQuantity);
								result.FilledQuantity += fallbackResult.FilledQuantity;
							}
						}
					}
					CheckUnfinished(set, result);
					if (result?.IsFilled != true) return null;
					return new CloseResult
					{
						Slippage = result.AveragePrice.Value - signalPrice,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						ClosePrice = result.AveragePrice.Value
					};
				}
			}
			finally
			{
				set.Stopwatch.Stop();
			}

			return null;
		}

		private CloseResult CloseShort(LatencyArb set, LatencyArbPosition arbPos, bool isFirst)
		{
			var signalPrice = set.LastShortTick.Ask;
			set.Stopwatch.Restart();
			try
			{
				if (set.ShortAccount.Connector is Mt4Integration.Connector connector)
				{
					if (!arbPos.ShortTicket.HasValue) return null;
					connector.Positions.TryGetValue(arbPos.ShortTicket.Value, out var pos);
					if (pos == null || pos.IsClosed)
					{
						arbPos.Archived = true;
						set.State = LatencyArb.LatencyArbStates.Error;
						Logger.Error($"{set} latency arb - {arbPos.Level}. - unexpected closed or missing position(s)");
						return null;
					}
					connector.SendClosePositionRequests(pos);
					if (!pos.IsClosed) return null;
					return new CloseResult
					{
						Slippage = signalPrice - pos.ClosePrice,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						ClosePrice = pos.ClosePrice
					};
				}

				if (set.ShortAccount.Connector is FixApiIntegration.Connector fixConnector && arbPos.ShortPosition != null)
				{
					OrderResponse result = null;
					if (isFirst && set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Market ||
					    !isFirst && set.HedgeOrderType == LatencyArb.LatencyArbOrderTypes.Market)
						result = fixConnector.SendMarketOrderRequest(set.ShortSymbol, Sides.Buy, arbPos.ShortPosition.Size, set.MarketTimeWindowInMs, 0, 0).Result;

					else if (isFirst && set.FirstOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive ||
					         !isFirst && set.HedgeOrderType == LatencyArb.LatencyArbOrderTypes.Aggressive)
					{
						result = fixConnector.SendAggressiveOrderRequest(set.ShortSymbol, Sides.Buy, arbPos.ShortPosition.Size,
							set.LastShortTick.Ask, set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;
						CheckUnfinished(set, result);
						if (result?.FilledQuantity != arbPos.ShortPosition.Size)
						{
							var quantity = arbPos.ShortPosition.Size - (result?.FilledQuantity ?? 0);
							var fallbackResult = fixConnector.SendMarketOrderRequest(set.ShortSymbol, Sides.Buy, quantity, set.MarketTimeWindowInMs, 0, 0).Result;
							CheckUnfinished(set, fallbackResult);
							if (result?.IsFilled != true) result = fallbackResult;
							else if (fallbackResult?.IsFilled == true)
							{
								result.AveragePrice =
									(result.AveragePrice * result.FilledQuantity + fallbackResult.AveragePrice * fallbackResult.FilledQuantity) /
									(result.FilledQuantity + fallbackResult.FilledQuantity);
								result.FilledQuantity += fallbackResult.FilledQuantity;
							}
						}
					}
					CheckUnfinished(set, result);
					if (result?.IsFilled != true) return null;
					return new CloseResult
					{
						Slippage = signalPrice - result.AveragePrice.Value,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						ClosePrice = result.AveragePrice.Value
					};
				}
			}
			finally
			{
				set.Stopwatch.Stop();
			}

			return null;
		}

		private void Reset(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Reset) return;
			set.LivePositions.ForEach(p =>
			{
				p.Archived = true;
				RemoveCopierPosition(set, p);
			});
			set.State = LatencyArb.LatencyArbStates.None;
		}
		private void EmergencyImmediateExit(LatencyArb set)
		{
			if (set.State == LatencyArb.LatencyArbStates.None) return;
			if (set.State == LatencyArb.LatencyArbStates.ImmediateExit) return;
			if (set.State == LatencyArb.LatencyArbStates.Sync) return;
			if (set.State == LatencyArb.LatencyArbStates.Reset) return;
			if (set.State == LatencyArb.LatencyArbStates.Error) return;
			if (!set.LivePositions.Any()) return;
			if (set.EmergencyShortExitInPip <= 0 && set.EmergencyLongExitInPip <= 0) return;

			var avg = set.LivePositions
				.Where(p => p.LongOpenPrice.HasValue && p.ShortOpenPrice.HasValue)
				.Average(p => (p.LongOpenPrice + p.ShortOpenPrice) / 2);
			var shortExit = avg - set.EmergencyShortExitInPip * set.PipSize;
			var longExit = avg + set.EmergencyLongExitInPip * set.PipSize;

			if (set.EmergencyShortExitInPip > 0)
			{
				if (set.LastFeedTick.HasValue && set.LastFeedTick.Bid <= shortExit ||
				    set.LastLongTick.HasValue && set.LastLongTick.Bid <= shortExit ||
				    set.LastShortTick.HasValue && set.LastShortTick.Bid <= shortExit)
					set.State = LatencyArb.LatencyArbStates.ImmediateExit;
			}
			if (set.EmergencyLongExitInPip > 0)
			{
				if (set.LastFeedTick.HasValue && set.LastFeedTick.Bid >= longExit ||
				    set.LastLongTick.HasValue && set.LastLongTick.Bid >= longExit ||
				    set.LastShortTick.HasValue && set.LastShortTick.Bid >= longExit)
					set.State = LatencyArb.LatencyArbStates.ImmediateExit;
			}

			if (set.State == LatencyArb.LatencyArbStates.ImmediateExit)
				Logger.Warn($"{set} latency arb. - EmergencyImmediateExit");
		}
		private void Emergency(LatencyArb set)
		{
			if (set.State == LatencyArb.LatencyArbStates.None) return;
			if (set.State == LatencyArb.LatencyArbStates.ImmediateExit) return;
			if (set.State == LatencyArb.LatencyArbStates.Sync) return;
			if (set.State == LatencyArb.LatencyArbStates.Reset) return;
			if (set.State == LatencyArb.LatencyArbStates.Error) return;
			if (set.EmergencyOff <= 0) return;
			if (set.EmergencyCount < set.EmergencyOff) return;
			set.EmergencyCount = 0;
			set.State = LatencyArb.LatencyArbStates.None;
			Logger.Warn($"{set} latency arb. - emergency OFF");
		}

		private void Sync(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Sync) return;
			CheckPositions(set);
			set.LivePositions.ForEach(p =>
			{
				p.Archived = true;
				RemoveCopierPosition(set, p);
			});
			SyncOnlyMt4(set);
			SyncFixMt4(set);
		}
		private void SyncOnlyMt4(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Sync) return;
			set.LatencyArbPositions.ForEach(p => p.Archived = true);
			var longPositions = (set.LongAccount.Connector as Mt4Integration.Connector)?.Positions;
			var shortPositions = (set.ShortAccount.Connector as Mt4Integration.Connector)?.Positions;
			if (longPositions == null) return;
			if (shortPositions == null) return;

			foreach (var longPos in longPositions)
			{
				if (longPos.Value.IsClosed) continue;
				if (longPos.Value.Side != Sides.Buy) continue;
				if (longPos.Value.Lots != set.LongSize) continue;
				if (longPos.Value.Symbol != set.LongSymbol) continue;
				if (set.LivePositions.Any(p => p.LongTicket == longPos.Key)) continue;
				if (!string.IsNullOrWhiteSpace(set.Comment) && longPos.Value.Comment != set.Comment) continue;

				KeyValuePair<long, Position>? match = null;
				foreach (var shortPos in shortPositions)
				{
					if (shortPos.Value.IsClosed) continue;
					if (shortPos.Value.Side != Sides.Sell) continue;
					if (shortPos.Value.Lots != set.ShortSize) continue;
					if (shortPos.Value.Symbol != set.ShortSymbol) continue;
					if (set.LivePositions.Any(p => p.ShortTicket == shortPos.Key)) continue;
					if (!string.IsNullOrWhiteSpace(set.Comment) && shortPos.Value.Comment != set.Comment) continue;
					match = shortPos;
					break;
				}

				if (!match.HasValue) break;
				var level = set.LivePositions.Count + 1;
				var arbPos = new LatencyArbPosition()
				{
					LatencyArb = set,
					LongTicket = longPos.Key,
					LongOpenPrice = longPos.Value.OpenPrice,
					ShortTicket = match.Value.Key,
					ShortOpenPrice = match.Value.Value.OpenPrice,
					Level = level
				};
				set.LatencyArbPositions.Add(arbPos);
			}
			foreach (var arbPos in set.LivePositions)
				AddCopierPosition(set, arbPos);

			set.State = LatencyArb.LatencyArbStates.None;
			Logger.Debug($"{set} latency arb. - SyncOnlyMt4");
		}
		private void SyncFixMt4(LatencyArb set)
		{
			if (set.State != LatencyArb.LatencyArbStates.Sync) return;
			set.LatencyArbPositions.ForEach(p => p.Archived = true);
			if (!(set.LongAccount.Connector is FixApiIntegration.Connector) &&
			    !(set.ShortAccount.Connector is FixApiIntegration.Connector))
				return;

			var longPositions = (set.LongAccount.Connector as Mt4Integration.Connector)?.Positions;
			var shortPositions = (set.ShortAccount.Connector as Mt4Integration.Connector)?.Positions;
			if (longPositions == null && shortPositions == null) return;
			
			foreach (var longPos in longPositions ?? new ConcurrentDictionary<long, Position>())
			{
				if (longPos.Value.IsClosed) continue;
				if (longPos.Value.Side != Sides.Buy) continue;
				if (longPos.Value.Lots != set.LongSize) continue;
				if (longPos.Value.Symbol != set.LongSymbol) continue;
				if (set.LivePositions.Any(p => p.LongTicket == longPos.Key)) continue;
				if (!string.IsNullOrWhiteSpace(set.Comment) && longPos.Value.Comment != set.Comment) continue;

				var level = set.LivePositions.Count + 1;
				var arbPos = new LatencyArbPosition()
				{
					LatencyArb = set,
					LongTicket = longPos.Key,
					LongOpenPrice = longPos.Value.OpenPrice,
					ShortOpenPrice = longPos.Value.OpenPrice,
					ShortPosition = new StratPosition()
					{
						Account = set.ShortAccount,
						AvgPrice = longPos.Value.OpenPrice,
						OpenTime = longPos.Value.OpenTime,
						Symbol = set.ShortSymbol,
						Size = set.ShortSize
					},
					Level = level
				};
				set.LatencyArbPositions.Add(arbPos);
			}
			foreach (var shortPos in shortPositions ?? new ConcurrentDictionary<long, Position>())
			{
				if (shortPos.Value.IsClosed) continue;
				if (shortPos.Value.Side != Sides.Sell) continue;
				if (shortPos.Value.Lots != set.ShortSize) continue;
				if (shortPos.Value.Symbol != set.ShortSymbol) continue;
				if (set.LivePositions.Any(p => p.ShortTicket == shortPos.Key)) continue;
				if (!string.IsNullOrWhiteSpace(set.Comment) && shortPos.Value.Comment != set.Comment) continue;

				var level = set.LivePositions.Count + 1;
				var arbPos = new LatencyArbPosition()
				{
					LatencyArb = set,
					LongPosition = new StratPosition()
					{
						Account = set.LongAccount,
						AvgPrice = shortPos.Value.OpenPrice,
						OpenTime = shortPos.Value.OpenTime,
						Symbol = set.LongSymbol,
						Size = set.LongSize
					},
					LongOpenPrice = shortPos.Value.OpenPrice,
					ShortTicket = shortPos.Key,
					ShortOpenPrice = shortPos.Value.OpenPrice,
					Level = level
				};
				set.LatencyArbPositions.Add(arbPos);
			}
			foreach (var arbPos in set.LivePositions)
				AddCopierPosition(set, arbPos);

			set.State = LatencyArb.LatencyArbStates.None;
			Logger.Debug($"{set} latency arb. - SyncFixMt4");
		}
		private void AddCopierPosition(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (!arbPos.HasBothSides) return;
			AddCopierPositionMt4(set, arbPos);
			AddCopierPositionFix(set, arbPos);
		}
		private void AddCopierPositionMt4(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.Copier == null) return;
			if (set.Copier.Slave.Account != set.LongAccount &&
			    set.Copier.Slave.Account != set.ShortAccount) return;
			if (set.Copier.Slave.Master.Account != set.LongAccount &&
			    set.Copier.Slave.Master.Account != set.ShortAccount) return;
			if (!arbPos.LongTicket.HasValue) return;
			if (!arbPos.ShortTicket.HasValue) return;

			if (set.Copier.Slave.Master.Account == set.ShortAccount)
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
			else if (set.Copier.Slave.Master.Account == set.LongAccount)
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
		private void AddCopierPositionFix(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.FixApiCopier == null) return;
			if (set.FixApiCopier.Slave.Account != set.LongAccount &&
			    set.FixApiCopier.Slave.Account != set.ShortAccount) return;
			if (set.FixApiCopier.Slave.Master.Account != set.LongAccount &&
			    set.FixApiCopier.Slave.Master.Account != set.ShortAccount) return;

			// Short MT4 master
			if (set.FixApiCopier.Slave.Master.Account == set.ShortAccount &&
			    arbPos.ShortTicket.HasValue && arbPos.LongPosition != null)
			{
				if (set.FixApiCopier.FixApiCopierPositions.Any(cp => cp.MasterPositionId == arbPos.ShortTicket)) return;
				set.FixApiCopier.FixApiCopierPositions.Add(new FixApiCopierPosition()
				{
					FixApiCopier = set.FixApiCopier,
					MasterPositionId = arbPos.ShortTicket.Value,
					OpenPosition = arbPos.LongPosition
				});
			}
			// Long MT4 master
			else if (set.FixApiCopier.Slave.Master.Account == set.LongAccount &&
			         arbPos.LongTicket.HasValue && arbPos.ShortPosition != null)
			{
				if (set.FixApiCopier.FixApiCopierPositions.Any(cp => cp.MasterPositionId == arbPos.LongTicket)) return;
				set.FixApiCopier.FixApiCopierPositions.Add(new FixApiCopierPosition()
				{
					FixApiCopier = set.FixApiCopier,
					MasterPositionId = arbPos.LongTicket.Value,
					OpenPosition = arbPos.ShortPosition
				});
			}
		}
		private void RemoveCopierPosition(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (!arbPos.HasBothSides) return;
			RemoveCopierPositionMt4(set, arbPos);
			RemoveCopierPositionFix(set, arbPos);
		}
		private void RemoveCopierPositionMt4(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.Copier == null) return;
			if (!arbPos.LongTicket.HasValue) return;
			if (!arbPos.ShortTicket.HasValue) return;
			if (set.Copier.Slave.Account != set.LongAccount &&
			    set.Copier.Slave.Account != set.ShortAccount) return;
			if (set.Copier.Slave.Master.Account != set.LongAccount &&
			    set.Copier.Slave.Master.Account != set.ShortAccount) return;

			if (set.Copier.Slave.Master.Account == set.ShortAccount)
				set.Copier.CopierPositions.RemoveAll(cp =>
					cp.SlaveTicket == arbPos.LongTicket && cp.MasterTicket == arbPos.ShortTicket);
			else if (set.Copier.Slave.Master.Account == set.LongAccount)
				set.Copier.CopierPositions.RemoveAll(cp =>
					cp.SlaveTicket == arbPos.ShortTicket && cp.MasterTicket == arbPos.LongTicket);
		}
		private void RemoveCopierPositionFix(LatencyArb set, LatencyArbPosition arbPos)
		{
			if (set.FixApiCopier == null) return;
			if (set.FixApiCopier.Slave.Account != set.LongAccount &&
			    set.FixApiCopier.Slave.Account != set.ShortAccount) return;
			if (set.FixApiCopier.Slave.Master.Account != set.LongAccount &&
			    set.FixApiCopier.Slave.Master.Account != set.ShortAccount) return;

			// Short MT4 master
			if (set.FixApiCopier.Slave.Master.Account == set.ShortAccount &&
			    arbPos.ShortTicket.HasValue && arbPos.LongPosition != null)
				set.FixApiCopier.FixApiCopierPositions.RemoveAll(cp =>
					cp.MasterPositionId == arbPos.ShortTicket && cp.OpenPosition == arbPos.LongPosition);
			// Long MT4 master
			else if (set.FixApiCopier.Slave.Master.Account == set.LongAccount &&
			         arbPos.LongTicket.HasValue && arbPos.ShortPosition != null)
				set.FixApiCopier.FixApiCopierPositions.RemoveAll(cp =>
					cp.MasterPositionId == arbPos.LongTicket && cp.OpenPosition == arbPos.ShortPosition);
		}

		private bool IsTime(TimeSpan current, TimeSpan? start, TimeSpan? end)
		{
			if (!start.HasValue || !end.HasValue) return false;
			var startOk = current >= start;
			var endOk = current < end;

			if (end < start) return startOk || endOk;
			return startOk && endOk;
		}

		private void CheckUnfinished(LatencyArb set, OrderResponse response)
		{
			if (response?.IsUnfinished != true) return;
			set.State = LatencyArb.LatencyArbStates.Error;
			Logger.Warn($"{set} latency arb. - unfinished ERROR");
		}
		private void CheckUnfinished(LatencyArb set, PositionResponse response)
		{
			if (response?.IsUnfinished != true) return;
			set.State = LatencyArb.LatencyArbStates.Error;
			Logger.Warn($"{set} latency arb. - unfinished ERROR");
		}
	}
}

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
	public interface INewsArbService
	{
		void Start(List<NewsArb> sets);
		void Stop();
	}

	public class NewsArbService : INewsArbService
	{
		private class OpenResult
		{
			public Account Account { get; set; }
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

		private List<NewsArb> _sets;
		private readonly ConcurrentDictionary<int, FastBlockingCollection<Action>> _queues =
			new ConcurrentDictionary<int, FastBlockingCollection<Action>>();

		public void Start(List<NewsArb> sets)
		{
			_cancellation?.Dispose();

			_sets = sets;
			_cancellation = new CancellationTokenSource();

			foreach (var set in _sets)
			{
				if (!set.Run) continue;
				if (!set.IsConnected) continue;
				new Thread(() => SetLoop(set, _cancellation.Token)) { Name = $"NewsArb_{set.Id}", IsBackground = true }
					.Start();
			}

			Logger.Info("News arbs are started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("News arbs are stopped");
		}

		private void SetLoop(NewsArb set, CancellationToken token)
		{
			var queue = _queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>());

			set.NewTick -= Set_NewTick;
			set.SnwAccount.Connector.Subscribe(set.SnwSymbol);
			set.FirstAccount.Connector.Subscribe(set.FirstSymbol);
			set.HedgeAccount.Connector.Subscribe(set.HedgeSymbol);
			set.NewTick += Set_NewTick;

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (set.State == NewsArb.NewsArbStates.None)
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
					Logger.Error("NewsArbService.Loop exception", e);
				}
			}

			//set.State = NewsArb.NewsArbStates.None;
			_queues.TryRemove(set.Id, out queue);
		}

		private void Set_NewTick(object sender, NewTick e)
		{
			if (_cancellation.IsCancellationRequested) return;
			var set = (NewsArb)sender;
			if (!set.Run) return;
			if (set.State == NewsArb.NewsArbStates.None) return;

			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => Check(set));
		}

		private void Check(NewsArb set)
		{
			if (set.State == NewsArb.NewsArbStates.None) return;
			if (set.State == NewsArb.NewsArbStates.Reset)
			{
				foreach (var arbPos in set.NewsArbPositions)
				{
					arbPos.Archived = true;
				}
				set.State = NewsArb.NewsArbStates.None;
				return;
			}

			if (!set.HasPrices) return;
			if (HiResDatetime.UtcNow - set.LastFirstTick.Time > TimeSpan.FromSeconds(60)) return;
			if (HiResDatetime.UtcNow - set.LastHedgeTick.Time > TimeSpan.FromSeconds(60)) return;

			CheckOpening(set);
		}

		private void CheckOpening(NewsArb set)
		{
			if (set.State != NewsArb.NewsArbStates.Opening) return;

			var last = set.LivePositions
				.FirstOrDefault(p => !p.HasLong || !p.HasShort);

			// Check for new signal
			if (last == null)
			{
				if (set.State != NewsArb.NewsArbStates.Opening) return;
				if (!set.ShortSpreadCheck) return;
				if (!set.LongSpreadCheck) return;
				if (set.LastSnwTick == null) return;
				if (set.LastSnwTick.Ask == 0 && set.LastSnwTick.Bid == 0) return;
				if (HiResDatetime.UtcNow - set.LastSnwTick.Time > TimeSpan.FromMilliseconds(set.SnwTimeWindowInMs)) return;
				if (set.LastSnwTick.Ask > 0) Logger.Debug($"{set} News arb buy signal {set.LastSnwTick.Ask}");
				if (set.LastSnwTick.Bid > 0) Logger.Debug($"{set} News arb sell signal {set.LastSnwTick.Bid}");
				// Long signal
				if (set.LastSnwTick.Ask == set.SnwSignal)
				{
					var pos = SendLongOrder(set, true);
					if (pos == null)
					{
						Logger.Warn($"{set} News arb long first side open error");
						return;
					}
					set.NewsArbPositions.Add(new NewsArbPosition()
					{
						NewsArb = set,
						LongTicket = pos.Ticket,
						LongPosition = pos.StratPosition,
						LongOpenPrice = pos.OpenPrice
					});
					Logger.Info($"{set} News arb long first side opened at {pos.OpenPrice}" +
					            $"{Environment.NewLine}\tExecution time is {pos.ExecutionTime} ms with {pos.Slippage / set.PipSize:F2} pip slippage");
				}
				// Short signal
				else if (set.LastSnwTick.Bid == set.SnwSignal)
				{
					var pos = SendShortOrder(set, true);
					if (pos == null)
					{
						Logger.Warn($"{set} News arb short first side open error");
						return;
					}
					set.NewsArbPositions.Add(new NewsArbPosition()
					{
						NewsArb = set,
						ShortTicket = pos.Ticket,
						ShortPosition = pos.StratPosition,
						ShortOpenPrice = pos.OpenPrice
					});
					Logger.Info($"{set} News arb short first side opened at {pos.OpenPrice}" +
					            $"{Environment.NewLine}\tExecution time is {pos.ExecutionTime} ms with {pos.Slippage / set.PipSize:F2} pip slippage");
				}
			}
			// Long side opened
			else if (last.HasLong)
			{
				var hedge = false;
				var price = set.LastHedgeTick.Bid;

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
					Logger.Warn($"{set} News arb short hedge side open error");
					return;
				}
				last.ShortTicket = pos.Ticket;
				last.ShortPosition = pos.StratPosition;
				last.ShortOpenPrice = pos.OpenPrice;
				set.State = NewsArb.NewsArbStates.Closing;
				Logger.Info($"{set} News arb short hedge side opened at {pos.OpenPrice} with {(pos.OpenPrice - last.LongOpenPrice)/set.PipSize} pips" +
				            $"{Environment.NewLine}\tExecution time is {pos.ExecutionTime} ms with {pos.Slippage / set.PipSize:F2} pip slippage");
			}
			// Short side opened
			else if (last.HasShort)
			{
				var hedge = false;
				var price = set.LastHedgeTick.Ask;

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
					Logger.Warn($"{set} News arb long hedge side open error");
					return;
				}
				last.LongTicket = pos.Ticket;
				last.LongPosition = pos.StratPosition;
				last.LongOpenPrice = pos.OpenPrice;
				set.State = NewsArb.NewsArbStates.Closing;
				Logger.Info($"{set} News arb long hedge side opened at {pos.OpenPrice} with {(last.ShortOpenPrice - pos.OpenPrice) / set.PipSize} pips" +
				            $"{Environment.NewLine}\tExecution time is {pos.ExecutionTime} ms with {pos.Slippage/set.PipSize:F2} pip slippage");
			}
		}

		private OpenResult SendLongOrder(NewsArb set, bool isFirst)
		{
			var account = isFirst ? set.FirstAccount : set.HedgeAccount;
			var symbol = isFirst ? set.FirstSymbol : set.HedgeSymbol;
			var size = isFirst ? set.FirstSize : set.HedgeSize;
			var signalPrice = isFirst ? set.LastFirstTick.Ask : set.LastHedgeTick.Ask;
			set.Stopwatch.Restart();
			try
			{
				if (account.Connector is Mt4Integration.Connector connector)
				{
					var pos = connector.SendMarketOrderRequest(symbol, Sides.Buy, (double)size, 0, set.Comment);
					if (pos == null) return null;
					return new OpenResult
					{
						Account = account,
						Slippage = signalPrice - pos.OpenPrice,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						Ticket = pos.Id,
						OpenPrice = pos.OpenPrice
					};
				}

				if (account.Connector is FixApiIntegration.Connector fixConnector)
				{
					OrderResponse result = null;
					if (!isFirst || set.FirstOrderType == NewsArb.NewsArbOrderTypes.Market)
						result = fixConnector.SendMarketOrderRequest(symbol, Sides.Buy, size).Result;
					else if (set.FirstOrderType == NewsArb.NewsArbOrderTypes.Aggressive)
						result = fixConnector.SendAggressiveOrderRequest(symbol, Sides.Buy, size, signalPrice,
							set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;

					if (result?.IsFilled != true) return null;
					return new OpenResult
					{
						Account = account,
						Slippage = signalPrice - result.AveragePrice.Value,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						OpenPrice = result.AveragePrice.Value,
						StratPosition = new StratPosition()
						{
							Account = account,
							AvgPrice = result.AveragePrice.Value,
							OpenTime = HiResDatetime.UtcNow,
							Side = result.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell,
							Symbol = symbol,
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

		private OpenResult SendShortOrder(NewsArb set, bool isFirst)
		{
			var account = isFirst ? set.FirstAccount : set.HedgeAccount;
			var symbol = isFirst ? set.FirstSymbol : set.HedgeSymbol;
			var size = isFirst ? set.FirstSize : set.HedgeSize;
			var signalPrice = isFirst ? set.LastFirstTick.Bid : set.LastHedgeTick.Bid;
			set.Stopwatch.Restart();
			try
			{
				if (account.Connector is Mt4Integration.Connector connector)
				{
					var pos = connector.SendMarketOrderRequest(symbol, Sides.Sell, (double)size, 0, set.Comment);
					if (pos == null) return null;
					return new OpenResult
					{
						Account = account,
						Slippage = pos.OpenPrice - signalPrice,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						Ticket = pos.Id,
						OpenPrice = pos.OpenPrice
					};
				}

				if (account.Connector is FixApiIntegration.Connector fixConnector)
				{
					OrderResponse result = null;
					if (!isFirst || set.FirstOrderType == NewsArb.NewsArbOrderTypes.Market)
						result = fixConnector.SendMarketOrderRequest(symbol, Sides.Sell, size).Result;
					else if (set.FirstOrderType == NewsArb.NewsArbOrderTypes.Aggressive)
						result = fixConnector.SendAggressiveOrderRequest(symbol, Sides.Sell, size, signalPrice,
							set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;

					if (result?.IsFilled != true) return null;
					return new OpenResult
					{
						Account = account,
						Slippage = result.AveragePrice.Value - signalPrice,
						ExecutionTime = set.Stopwatch.ElapsedMilliseconds,
						OpenPrice = result.AveragePrice.Value,
						StratPosition = new StratPosition()
						{
							Account = account,
							AvgPrice = result.AveragePrice.Value,
							OpenTime = HiResDatetime.UtcNow,
							Side = result.Side == Sides.Buy ? StratPosition.Sides.Buy : StratPosition.Sides.Sell,
							Symbol = symbol,
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

		#region close
		/*
		private void CheckClosing(NewsArb set)
		{
			if (set.State != NewsArb.NewsArbStates.Closing) return;

			var first = set.LivePositions.OrderBy(p => p.Level).FirstOrDefault(p => p.HasBothSides);
			if (first == null) return;
			if (!first.HasBothSides) return;

			// No closed side
			if (!first.LongClosed && !first.ShortClosed)
			{
				if (!set.ShortSpreadCheck) return;
				if (!set.LongSpreadCheck) return;
				// Long close signal
				if (set.FirstSide != NewsArb.NewsArbFirstSides.Short && set.NormFeedBid <= set.NormLongBid - set.LongSignalDiffInPip * set.PipSize)
				{
					if (set.Copier != null) set.Copier.Run = false;
					var closePos = CloseLong(set, first, true);
					if (closePos != null) RemoveCopierPosition(set, first);
					if (set.Copier != null) set.Copier.Run = true;
					if (closePos == null) return;

					first.LongClosePrice = closePos.ClosePrice;
					first.Trailing = null;
					Logger.Info($"{set} News arb - {first.Level}. long first side closed at {closePos.ClosePrice}" +
					            $"{Environment.NewLine}\tExecution time is {closePos.ExecutionTime} ms with {closePos.Slippage / set.PipSize:F2} pip slippage");
				}
				// Short close signal
				else if (set.FirstSide != NewsArb.NewsArbFirstSides.Long && set.NormFeedAsk >= set.NormShortAsk + set.ShortSignalDiffInPip * set.PipSize)
				{
					if (set.Copier != null) set.Copier.Run = false;
					var closePos = CloseShort(set, first, true);
					if (closePos != null) RemoveCopierPosition(set, first);
					if (set.Copier != null) set.Copier.Run = true;
					if (closePos == null) return;

					first.ShortClosePrice = closePos.ClosePrice;
					first.Trailing = null;
					Logger.Info($"{set} News arb - {first.Level}. short first side closed at {closePos.ClosePrice}" +
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
				if (!hedge) return;

				var closePos = CloseShort(set, first, false);
				if (closePos == null) return;
				first.ShortClosePrice = closePos.ClosePrice;
				first.Archived = true;
				Logger.Info($"{set} News arb - {first.Level}. short hedge side closed at {closePos.ClosePrice} with {(first.LongClosePrice - closePos.ClosePrice) /set.PipSize} pips" +
				            $"{Environment.NewLine}\tExecution time is {closePos.ExecutionTime} ms with {closePos.Slippage / set.PipSize:F2} pip slippage");
				// Switch state if rotating
				if (set.Rotating && set.State == NewsArb.NewsArbStates.Closing &&
				    set.LivePositions.Count == 0)
					set.State = NewsArb.NewsArbStates.Opening;
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

				var closePos = CloseLong(set, first, false);
				if (closePos == null) return;
				first.LongClosePrice = closePos.ClosePrice;
				first.Archived = true;
				Logger.Info($"{set} News arb - {first.Level}. long hedge side closed at {closePos.ClosePrice} with {(closePos.ClosePrice - first.ShortClosePrice) / set.PipSize} pips" +
				            $"{Environment.NewLine}\tExecution time is {closePos.ExecutionTime} ms with {closePos.Slippage / set.PipSize:F2} pip slippage");
				// Switch state if rotating
				if (set.Rotating && set.State == NewsArb.NewsArbStates.Closing &&
				    set.LivePositions.Count == 0)
					set.State = NewsArb.NewsArbStates.Opening;
			}
		}

		private CloseResult CloseLong(NewsArb set, NewsArbPosition arbPos, bool isFirst)
		{
			var signalPrice = set.LastLongTick.Bid;
			set.Stopwatch.Restart();
			try
			{
				if (set.LongAccount.Connector is Mt4Integration.Connector connector)
				{
					if (!arbPos.LongTicket.HasValue) return null;
					connector.Positions.TryGetValue(arbPos.LongTicket.Value, out var pos);
					if (pos == null) return null;
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
					if (!isFirst || set.FirstOrderType == NewsArb.NewsArbOrderTypes.Market)
						result = fixConnector.SendMarketOrderRequest(set.LongSymbol, Sides.Sell, arbPos.LongPosition.Size).Result;
					else if (set.FirstOrderType == NewsArb.NewsArbOrderTypes.Aggressive)
						result = fixConnector.SendAggressiveOrderRequest(set.LongSymbol, Sides.Sell, arbPos.LongPosition.Size,
							set.LastLongTick.Bid,
							set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;

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

		private CloseResult CloseShort(NewsArb set, NewsArbPosition arbPos, bool isFirst)
		{
			var signalPrice = set.LastShortTick.Ask;
			set.Stopwatch.Restart();
			try
			{
				if (set.ShortAccount.Connector is Mt4Integration.Connector connector)
				{
					if (!arbPos.ShortTicket.HasValue) return null;
					connector.Positions.TryGetValue(arbPos.ShortTicket.Value, out var pos);
					if (pos == null) return null;
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
					if (!isFirst || set.FirstOrderType == NewsArb.NewsArbOrderTypes.Market)
						result = fixConnector.SendMarketOrderRequest(set.ShortSymbol, Sides.Buy, arbPos.ShortPosition.Size).Result;
					else if (set.FirstOrderType == NewsArb.NewsArbOrderTypes.Aggressive)
						result = fixConnector.SendAggressiveOrderRequest(set.ShortSymbol, Sides.Buy, arbPos.ShortPosition.Size,
							set.LastShortTick.Ask,
							set.Deviation, 0, set.TimeWindowInMs, set.MaxRetryCount, set.RetryPeriodInMs).Result;

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
		*/
		#endregion 
	}
}

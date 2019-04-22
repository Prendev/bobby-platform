using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

		public void Start(List<MarketMaker> sets)
		{
			_cancellation?.Dispose();

			_sets = sets;
			_cancellation = new CancellationTokenSource();

			foreach (var set in _sets)
			{
				if (!set.Run) continue;
				new Thread(() => SetLoop(set, _cancellation.Token)) { Name = $"AntiMarketMaker_{set.Id}", IsBackground = true }
					.Start();
			}

			Logger.Info("Anti market makers are started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("Anti market makers are stopped");
		}

		private void SetLoop(MarketMaker set, CancellationToken token)
		{
			set.NewTick -= Set_NewTick;
			set.NewTick += Set_NewTick;

			var queue = _queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>());
			set.Account.Connector.Subscribe(set.Symbol);

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (set.State == MarketMaker.MarketMakerStates.None)
					{
						Thread.Sleep(1);
						continue;
					}
					if (set.State == MarketMaker.MarketMakerStates.Init) InitBases(set);

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

			set.NewTick -= Set_NewTick;
			_queues.TryRemove(set.Id, out queue);

			set.State = MarketMaker.MarketMakerStates.None;
			set.NextBottomDepth = 0;
			set.NextTopDepth = 0;
		}

		private void InitBases(MarketMaker set)
		{
			var lastTick = set.Account.GetLastTick(set.Symbol);
			if (lastTick?.HasValue != true)
			{
				Thread.Sleep(1);
				return;
			}

			var bid = set.InitBidPrice.HasValue ? set.InitBidPrice.Value : lastTick.Bid;
			var ask = set.InitBidPrice.HasValue ? (set.InitBidPrice.Value + set.TickSize) : lastTick.Ask;
			set.BottomBase = bid - set.InitialDistanceInTick * set.TickSize;
			set.TopBase = ask + set.InitialDistanceInTick * set.TickSize;

			set.State = MarketMaker.MarketMakerStates.Trade;
		}

		private void Set_NewTick(object sender, NewTick newTick)
		{
			if (_cancellation.IsCancellationRequested) return;
			var set = (MarketMaker)sender;

			if (!set.Run) return;
			if (set.State != MarketMaker.MarketMakerStates.Trade) return;

			_queues.GetOrAdd(set.Id, new FastBlockingCollection<Action>()).Add(() => Check(set));
		}

		private void Check(MarketMaker set)
		{
			lock (set)
			{
				CheckClose(set);
				CheckOpen(set);
			}
		}

		private void CheckClose(MarketMaker set)
		{
			CheckLongClose(set);
			CheckShortClose(set);
		}
		private void CheckLongClose(MarketMaker set)
		{
			if (_cancellation.IsCancellationRequested) return;
			if (!set.TopBase.HasValue) return;
			if (set.NextTopDepth <= 0) return;
			var stop = set.TopBase.Value + (set.NextTopDepth - 1) * set.LimitGapsInTick * set.TickSize - set.TpOrSlInTick * set.TickSize;
			var lastTick = set.Account.GetLastTick(set.Symbol);
			if (lastTick.Bid > stop - set.TpOrSlInTick * set.TickSize) return;
			if (lastTick.Bid == stop && set.DomTrigger > 0 && lastTick.BidVolume > set.DomTrigger) return;
			LongClose(set, set.NextTopDepth - 1);
		}
		private async void LongClose(MarketMaker set, int d)
		{
			try
			{
				var stop = set.TopBase.Value + d * set.LimitGapsInTick * set.TickSize - set.TpOrSlInTick * set.TickSize;
				var lastTick = set.Account.GetLastTick(set.Symbol);
				var connector = (FixApiConnectorBase)set.Account.Connector;
				var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());

				if (lastTick.Bid > stop - set.MarketThresholdInTick * set.TickSize)
				{
					if (level.LongCloseLimitResponse != null)
					{
						if (level.LongCloseLimitResponse.RemainingQuantity != 0) return;
						set.NextTopDepth--;
						level.LongCloseLimitResponse = null;
					}
					else level.LongCloseLimitResponse = await connector.PutNewOrderRequest(set.Symbol, Sides.Sell, set.ContractSize, stop);
				}
				// Switch to market
				else
				{
					if (level.LongCloseLimitResponse != null)
					{
						if (level.LongCloseLimitResponse.RemainingQuantity == 0)
						{
							set.NextTopDepth--;
							level.LongCloseLimitResponse = null;
							return;
						}

						if (!(await connector.CancelLimit(level.LongCloseLimitResponse))) return;
						if (level.LongCloseLimitResponse.FilledQuantity != 0)
						{
							set.NextTopDepth--;
							level.LongCloseLimitResponse = null;
							return;
						}
						level.LongCloseLimitResponse = null;
					}
					var orderResponse = await connector.SendMarketOrderRequest(set.Symbol, Sides.Sell, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					set.NextTopDepth--;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.LongClose exception", e);
			}
		}
		private void CheckShortClose(MarketMaker set)
		{
			if (_cancellation.IsCancellationRequested) return;
			if (!set.BottomBase.HasValue) return;
			if (set.NextBottomDepth <= 0) return;
			var stop = set.BottomBase.Value - (set.NextBottomDepth - 1) * set.LimitGapsInTick * set.TickSize + set.TpOrSlInTick * set.TickSize;
			var lastTick = set.Account.GetLastTick(set.Symbol);
			if (lastTick.Ask < stop - set.TpOrSlInTick * set.TickSize) return;
			if (lastTick.Ask == stop && set.DomTrigger > 0 && lastTick.AskVolume > set.DomTrigger) return;
			ShortClose(set, set.NextBottomDepth - 1);
		}
		private async void ShortClose(MarketMaker set, int d)
		{
			try
			{
				var stop = set.BottomBase.Value - d * set.LimitGapsInTick * set.TickSize + set.TpOrSlInTick * set.TickSize;
				var lastTick = set.Account.GetLastTick(set.Symbol);
				var connector = (FixApiConnectorBase)set.Account.Connector;
				var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());

				if (lastTick.Ask < stop + set.MarketThresholdInTick * set.TickSize)
				{
					if (level.ShortCloseLimitResponse != null)
					{
						if (level.ShortCloseLimitResponse.RemainingQuantity != 0) return;
						set.NextBottomDepth--;
						level.ShortCloseLimitResponse = null;
					}
					else level.ShortCloseLimitResponse = await connector.PutNewOrderRequest(set.Symbol, Sides.Buy, set.ContractSize, stop);
				}
				// Switch to market
				else
				{
					if (level.ShortCloseLimitResponse != null)
					{
						if (level.ShortCloseLimitResponse.RemainingQuantity == 0)
						{
							set.NextBottomDepth--;
							level.ShortCloseLimitResponse = null;
							return;
						}

						if (!(await connector.CancelLimit(level.ShortCloseLimitResponse))) return;
						if (level.ShortCloseLimitResponse.FilledQuantity != 0)
						{
							set.NextBottomDepth--;
							level.ShortCloseLimitResponse = null;
							return;
						}
						level.ShortCloseLimitResponse = null;
					}
					var orderResponse = await connector.SendMarketOrderRequest(set.Symbol, Sides.Buy, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					set.NextBottomDepth--;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.ShortClose exception", e);
			}
		}

		private void CheckOpen(MarketMaker set)
		{
			CheckLongOpen(set);
			CheckShortOpen(set);
		}
		private void CheckLongOpen(MarketMaker set)
		{
			if (_cancellation.IsCancellationRequested) return;
			if (!set.TopBase.HasValue) return;
			if (set.MaxDepth > 0 && set.NextTopDepth >= set.MaxDepth) return;
			var limit = set.TopBase.Value + set.NextTopDepth * set.LimitGapsInTick * set.TickSize;
			var lastTick = set.Account.GetLastTick(set.Symbol);
			if (lastTick.Ask < limit) return;
			if (lastTick.Ask == limit && set.DomTrigger > 0 && lastTick.AskVolume > set.DomTrigger) return;

			LongOpen(set, set.NextTopDepth).Wait();
		}
		private async Task LongOpen(MarketMaker set, int d)
		{
			try
			{
				var limit = set.TopBase.Value + d * set.LimitGapsInTick * set.TickSize;
				var lastTick = set.Account.GetLastTick(set.Symbol);
				var connector = (FixApiConnectorBase) set.Account.Connector;
				var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());

				if (lastTick.Ask < limit + set.MarketThresholdInTick * set.TickSize)
				{
					if (level.LongOpenLimitResponse != null)
					{
						if (level.LongOpenLimitResponse.RemainingQuantity != 0) return;
						set.NextTopDepth++;
						level.LongOpenLimitResponse = null;
					}
					else level.LongOpenLimitResponse = await connector.PutNewOrderRequest(set.Symbol, Sides.Buy, set.ContractSize, limit);
				}
				// Switch to market
				else
				{
					if (level.LongOpenLimitResponse != null)
					{
						if (level.LongOpenLimitResponse.RemainingQuantity == 0)
						{
							set.NextTopDepth++;
							level.LongOpenLimitResponse = null;
							return;
						}

						if (!(await connector.CancelLimit(level.LongOpenLimitResponse))) return;
						if (level.LongOpenLimitResponse.FilledQuantity != 0)
						{
							set.NextTopDepth++;
							level.LongOpenLimitResponse = null;
							return;
						}
						level.LongOpenLimitResponse = null;
					}
					var orderResponse = await connector.SendMarketOrderRequest(set.Symbol, Sides.Buy, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					set.NextTopDepth++;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.LongOpen exception", e);
			}
		}
		private void CheckShortOpen(MarketMaker set)
		{
			if (_cancellation.IsCancellationRequested) return;
			if (!set.BottomBase.HasValue) return;
			if (set.MaxDepth > 0 && set.NextBottomDepth >= set.MaxDepth) return;
			var limit = set.BottomBase.Value - set.NextBottomDepth * set.LimitGapsInTick * set.TickSize;
			var lastTick = set.Account.GetLastTick(set.Symbol);
			if (lastTick.Bid > limit) return;
			if (lastTick.Bid == limit && set.DomTrigger > 0 && lastTick.BidVolume > set.DomTrigger) return;

			ShortOpen(set, set.NextBottomDepth).Wait();
		}
		private async Task ShortOpen(MarketMaker set, int d)
		{
			try
			{
				var limit = set.BottomBase.Value - d * set.LimitGapsInTick * set.TickSize;
				var lastTick = set.Account.GetLastTick(set.Symbol);
				var connector = (FixApiConnectorBase)set.Account.Connector;
				var level = set.AntiLevels.GetOrAdd(d, new MarketMaker.Level());

				if (lastTick.Bid > limit - set.MarketThresholdInTick * set.TickSize)
				{
					if (level.ShortOpenLimitResponse != null)
					{
						if (level.ShortOpenLimitResponse.RemainingQuantity != 0) return;
						set.NextBottomDepth++;
						level.ShortOpenLimitResponse = null;
					}
					else level.ShortOpenLimitResponse = await connector.PutNewOrderRequest(set.Symbol, Sides.Sell, set.ContractSize, limit);
				}
				// Switch to market
				else
				{
					if (level.ShortOpenLimitResponse != null)
					{
						if (level.ShortOpenLimitResponse.RemainingQuantity == 0)
						{
							set.NextBottomDepth++;
							level.ShortOpenLimitResponse = null;
							return;
						}

						if (!(await connector.CancelLimit(level.ShortOpenLimitResponse))) return;
						if (level.ShortOpenLimitResponse.FilledQuantity != 0)
						{
							set.NextBottomDepth++;
							level.ShortOpenLimitResponse = null;
							return;
						}
						level.ShortOpenLimitResponse = null;
					}
					var orderResponse = await connector.SendMarketOrderRequest(set.Symbol, Sides.Sell, set.ContractSize);
					if (!orderResponse.IsFilled) return;
					set.NextBottomDepth++;
				}
			}
			catch (Exception e)
			{
				Logger.Error("AntiMarketMakerService.ShortOpen exception", e);
			}
		}
	}
}

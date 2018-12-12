using System;
using System.Threading;
using QvaDev.Common.Integration;
using QvaDev.Data;

namespace QvaDev.Orchestration.Services
{
	public interface ISpoofingService
	{
		SpoofingState Spoofing(Spoof spoof, Sides side);
	}

	public class SpoofingService : ISpoofingService
	{
		public SpoofingState Spoofing(Spoof spoof, Sides side)
		{
			if (!(spoof.TradeAccount?.Connector is IFixConnector)) return null;
			if (spoof.FeedAccount == null) return null;
			if (string.IsNullOrWhiteSpace(spoof.TradeSymbol)) return null;
			if (string.IsNullOrWhiteSpace(spoof.FeedSymbol)) return null;
			if (side == Sides.None) return null;
			if (spoof.Size <= 0) return null;

			var state = new SpoofingState();
			new Thread(() => Loop(spoof, side, state)) { IsBackground = true }.Start();
			return state;
		}

		private void Loop(Spoof spoof, Sides side, SpoofingState state)
		{
			var tradeConnector = (IFixConnector)spoof.TradeAccount.Connector;

			var lastTick = spoof.FeedAccount.GetLastTick(spoof.FeedSymbol);
			var waitHandle = new AutoResetEvent(false);

			void NewTick(object s, NewTick e)
			{
				if (state.IsCancellationRequested) return;
				if (e.Tick.Symbol != spoof.FeedSymbol) return;
				lastTick = e.Tick;
				waitHandle.Set();
			}

			var changed = false;
			spoof.FeedAccount.Connector.NewTick += NewTick;
			if (lastTick?.HasValue == true) waitHandle.Set();

			while (!state.IsCancellationRequested)
			{
				try
				{
					if (!waitHandle.WaitOne(10)) continue;
					if (state.IsCancellationRequested) break;
					if (HiResDatetime.UtcNow - lastTick.Time > TimeSpan.FromSeconds(10)) continue;

					var price = GetPrice(spoof, side, lastTick);
					if (state.LimitResponse == null)
						state.LimitResponse = tradeConnector.SendSpoofOrderRequest(spoof.TradeSymbol, side, spoof.Size, price).Result;
					else changed = tradeConnector.ChangeLimitPrice(state.LimitResponse, price).Result;
				}
				catch (Exception e)
				{
					Logger.Error("SpoofingService.Loop exception", e);
				}
			}
			spoof.FeedAccount.Connector.NewTick -= NewTick;

			try
			{
				var canceled = tradeConnector.CancelLimit(state.LimitResponse).Result;
				waitHandle.Dispose();
			}
			catch (Exception e)
			{
				Logger.Error("SpoofingService.Loop exception", e);
			}
		}

		private static decimal GetPrice(Spoof spoof, Sides side, Tick tick)
		{
			return side == Sides.Buy ? (tick.Bid - spoof.Distance) : (tick.Ask + spoof.Distance);
		}
	}
}

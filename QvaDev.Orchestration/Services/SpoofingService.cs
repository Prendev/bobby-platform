using System;
using System.Threading;
using QvaDev.Common.Integration;
using QvaDev.Data;

namespace QvaDev.Orchestration.Services
{
	public interface ISpoofingService
	{
		CancellationTokenSource Spoofing(Spoof spoof, Sides side);
	}

	public class SpoofingService : ISpoofingService
	{
		public CancellationTokenSource Spoofing(Spoof spoof, Sides side)
		{
			if (!(spoof.TradeAccount?.Connector is IFixConnector)) return null;
			if (spoof.FeedAccount == null) return null;
			if (string.IsNullOrWhiteSpace(spoof.TradeSymbol)) return null;
			if (string.IsNullOrWhiteSpace(spoof.FeedSymbol)) return null;
			if (side == Sides.None) return null;
			if (spoof.Size <= 0) return null;

			var cancel = new CancellationTokenSource();
			new Thread(() => Loop(spoof, side, cancel.Token)) { IsBackground = true }.Start();
			return cancel;
		}

		private void Loop(Spoof spoof, Sides side, CancellationToken token)
		{
			var tradeConnector = (IFixConnector)spoof.TradeAccount.Connector;
			spoof.FeedAccount.Connector.Subscribe(spoof.FeedSymbol);
			LimitResponse limitResponse = null;

			NewTick lastTick = null;
			var waitHandle = new AutoResetEvent(false);

			void NewTick(object s, NewTick e)
			{
				if (token.IsCancellationRequested) return;
				if (e.Tick.Symbol != spoof.FeedSymbol) return;
				lastTick = e;
				waitHandle.Set();
			}

			var changed = false;
			spoof.FeedAccount.Connector.NewTick += NewTick;
			while (!token.IsCancellationRequested)
			{
				try
				{
					if (!waitHandle.WaitOne(10)) continue;
					var price = GetPrice(spoof, side, lastTick);
					if (limitResponse == null)
						limitResponse = tradeConnector.SendSpoofOrderRequest(spoof.TradeSymbol, side, spoof.Size, price).Result;
					else changed = tradeConnector.ChangeLimitPrice(limitResponse, price).Result;
				}
				catch (Exception e)
				{
					Logger.Error("SpoofingService.Loop exception", e);
				}
			}
			spoof.FeedAccount.Connector.NewTick -= NewTick;

			try
			{
				var canceled = tradeConnector.CancelLimit(limitResponse).Result;
				waitHandle.Dispose();
			}
			catch (Exception e)
			{
				Logger.Error("SpoofingService.Loop exception", e);
			}
		}

		private static decimal GetPrice(Spoof spoof, Sides side, NewTick e)
		{
			return side == Sides.Buy ? (e.Tick.Bid - spoof.Distance) : (e.Tick.Ask + spoof.Distance);
		}
	}
}

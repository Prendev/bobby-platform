using System.Threading;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration.Services
{
	public class Spoof
	{
		public Account FeedAccount { get; }
		public string FeedSymbol { get; }
		public Account TradeAccount { get; }
		public string TradeSymbol { get; }
		public decimal Size { get; }
		public decimal Distance { get; }

		public Spoof(
			Account feedAccount,
			string feedSymbol,
			Account tradeAccount,
			string tradeSymbol,
			decimal size,
			decimal distance)
		{
			Size = size;
			Distance = distance;
			TradeSymbol = tradeSymbol;
			TradeAccount = tradeAccount;
			FeedSymbol = feedSymbol;
			FeedAccount = feedAccount;
		}
	}

	public class SpoofingService
	{
		public void Spoofing(Spoof spoof, Sides side, CancellationToken token)
		{
			if (!(spoof.TradeAccount.Connector is IFixConnector)) return;
			if (side == Sides.None) return;

			new Thread(() => Loop(spoof, side, token)) { IsBackground = true }.Start();
		}

		private void Loop(Spoof spoof, Sides side, CancellationToken token)
		{
			var tradeConnector = (IFixConnector)spoof.TradeAccount.Connector;
			spoof.FeedAccount.Connector.Subscribe(spoof.FeedSymbol);
			LimitResponse orderResponse = null;

			NewTick lastTick = null;
			var waitHandle = new AutoResetEvent(false);

			void NewTick(object s, NewTick e)
			{
				if (e.Tick.Symbol != spoof.FeedSymbol) return;
				lastTick = e;
				waitHandle.Set();
			}
			spoof.FeedAccount.Connector.NewTick += NewTick;

			var changed = false;
			while (!token.IsCancellationRequested)
			{
				if (!waitHandle.WaitOne(100)) continue;
				var price = GetPrice(spoof, side, lastTick);

				if (orderResponse == null)
					orderResponse = tradeConnector.SendSpoofOrderRequest(spoof.TradeSymbol, side, spoof.Size, price).Result;
				else changed = tradeConnector.ChangeLimitPrice(orderResponse, price).Result;
			}
			var canceled = tradeConnector.CancelLimit(orderResponse).Result;

			spoof.FeedAccount.Connector.NewTick -= NewTick;
		}

		private static decimal GetPrice(Spoof spoof, Sides side, NewTick e)
		{
			return side == Sides.Buy ? (e.Tick.Bid - spoof.Distance) : (e.Tick.Ask + spoof.Distance);
		}
	}
}

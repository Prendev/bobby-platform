using System;
using System.Collections.Concurrent;
using QvaDev.Common.Integration;

namespace QvaDev.FixApiIntegration
{
	public class Connector : IConnector
	{
		public string Description { get; }
		public bool IsConnected => true;
		public ConcurrentDictionary<long, Position> Positions { get; }
		public event PositionEventHandler OnPosition;
		public event BarHistoryEventHandler OnBarHistory;
		public event TickEventHandler OnTick;

		public void Disconnect()
		{
		}

		public long GetOpenContracts(string symbol)
		{
			throw new NotImplementedException();
		}

		public double GetBalance()
		{
			throw new NotImplementedException();
		}

		public double GetFloatingProfit()
		{
			throw new NotImplementedException();
		}

		public double GetPnl(DateTime @from, DateTime to)
		{
			throw new NotImplementedException();
		}

		public string GetCurrency()
		{
			throw new NotImplementedException();
		}

		public int GetDigits(string symbol)
		{
			throw new NotImplementedException();
		}

		public double GetPoint(string symbol)
		{
			throw new NotImplementedException();
		}

		public Tick GetLastTick(string symbol)
		{
			throw new NotImplementedException();
		}

		public void Connect()
		{
		}
	}
}

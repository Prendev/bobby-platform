using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication.FixApi;

namespace QvaDev.FixApiIntegration
{
	public class Connector : IConnector
	{
		private readonly ILog _log;
		public string Description { get; }
		public bool IsConnected { get; private set; }
		public ConcurrentDictionary<long, Position> Positions { get; }
		public event PositionEventHandler OnPosition;
		public event BarHistoryEventHandler OnBarHistory;
		public event TickEventHandler OnTick;

		public Connector(ILog log)
		{
			_log = log;
		}

		public bool Connect(string configPath)
		{
			var doc = new XmlDocument();
			doc.Load(configPath);
			
			var confType = ConnectorHelper.GetConfigurationType(doc.DocumentElement.Name);
			var configurationTpye = ConnectorHelper.GetConnectorType(confType);
			var conf = new XmlSerializer(confType).Deserialize(File.OpenRead(configPath));

			var connector = (FixConnectorBase)Activator.CreateInstance(configurationTpye, conf);

			IsConnected = true;
			return IsConnected;
		}

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
	}
}

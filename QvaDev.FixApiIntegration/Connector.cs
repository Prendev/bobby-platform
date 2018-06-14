using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
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
		private FixConnectorBase _fixConnector;

		public string Description { get; }
		public bool IsConnected => _fixConnector?.IsPricingConnected == true && _fixConnector?.IsTradingConnected == true;
		public ConcurrentDictionary<long, Position> Positions { get; }
		public event PositionEventHandler OnPosition;
		public event BarHistoryEventHandler OnBarHistory;
		public event TickEventHandler OnTick;

		public Connector(string configPath, ILog log)
		{
			_log = log;

			var doc = new XmlDocument();
			doc.Load(configPath);

			var confType = ConnectorHelper.GetConfigurationType(doc.DocumentElement.Name);
			var configurationTpye = ConnectorHelper.GetConnectorType(confType);
			var conf = new XmlSerializer(confType).Deserialize(File.OpenRead(configPath));

			_fixConnector = (FixConnectorBase)Activator.CreateInstance(configurationTpye, conf);
		}

		public async Task<bool> Connect()
		{
			await _fixConnector.ConnectPricingAsync();
			await _fixConnector.ConnectTradingAsync();
			return IsConnected;
		}

		public void Disconnect()
		{
			_fixConnector.PricingSocket.Close();
			_fixConnector.TradingSocket.Close();
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

		public void SendMarketOrderRequest(string symbol, Sides side, decimal quantity)
		{
			_fixConnector.NewOrderAsync(new NewOrderRequest()
			{
				Side = side == Sides.Buy ? Side.Buy : Side.Sell,
				Symbol = Symbol.Parse(symbol),
				Type = OrdType.Market,
				Quantity = quantity
			}).Wait();
		}
	}
}

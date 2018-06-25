using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication;
using QvaDev.Communication.FixApi;

namespace QvaDev.FixApiIntegration
{
	public class Connector : IFixConnector
	{
		private readonly ILog _log;
		private readonly TaskCompletionManager  _taskCompletionManager;
		private FixConnectorBase _fixConnector;

		public string Description { get; }
		public bool IsConnected => _fixConnector?.IsPricingConnected == true && _fixConnector?.IsTradingConnected == true;
		public ConcurrentDictionary<long, Position> Positions { get; }
		public event PositionEventHandler OnPosition;
		public event BarHistoryEventHandler OnBarHistory;
		public event TickEventHandler OnTick;

		public ConcurrentDictionary<string, SymbolData> SymbolInfos { get; set; } =
			new ConcurrentDictionary<string, SymbolData>();

		public Connector(string configPath, ILog log)
		{
			_log = log;
			_taskCompletionManager = new TaskCompletionManager(1000, 5000);

			var doc = new XmlDocument();
			doc.Load(configPath);

			var confType = ConnectorHelper.GetConfigurationType(doc.DocumentElement.Name);
			var configurationTpye = ConnectorHelper.GetConnectorType(confType);
			var conf = new XmlSerializer(confType).Deserialize(File.OpenRead(configPath));

			_fixConnector = (FixConnectorBase)Activator.CreateInstance(configurationTpye, conf);

			_fixConnector.PricingSocketClosed += _fixConnector_SocketClosed;
			_fixConnector.TradingSocketClosed += _fixConnector_SocketClosed;
			_fixConnector.Quote += _fixConnector_Quote;
			_fixConnector.ExecutionReport += _fixConnector_ExecutionReport;
		}

		private void _fixConnector_ExecutionReport(object sender, ExecutionReportEventArgs e)
		{
			if (!e.ExecutionReport.FulfilledQuantity.HasValue) return;
			if (e.ExecutionReport.Side != Side.Buy && e.ExecutionReport.Side != Side.Sell) return;
			if (new[] {ExecType.Fill, ExecType.PartialFill, ExecType.Trade}
				    .Contains(e.ExecutionReport.ExecutionType) == false) return;

			var quantity = e.ExecutionReport.FulfilledQuantity.Value;
			if (e.ExecutionReport.Side == Side.Sell) quantity *= -1;

			SymbolInfos.AddOrUpdate(e.ExecutionReport.Symbol.ToString(),
				new SymbolData { SumContracts = quantity },
				(key, oldValue) =>
				{
					oldValue.SumContracts += quantity;
					return oldValue;
				});

			_taskCompletionManager.SetResult(e.ExecutionReport.OrderId, e.ExecutionReport);
		}

		// TODO go to nullable?
		private void _fixConnector_Quote(object sender, QuoteEventArgs e)
		{
			var ask = e.QuoteSet.Entries.First().Ask;
			var bid = e.QuoteSet.Entries.First().Bid;
			SymbolInfos.AddOrUpdate(e.QuoteSet.Symbol.ToString(),
				new SymbolData {Bid = bid ?? 0, Ask = ask ?? 0 },
				(key, oldValue) =>
				{
					oldValue.Bid = bid ?? oldValue.Bid;
					oldValue.Ask = ask ?? oldValue.Ask;
					return oldValue;
				});
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

		public decimal SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null)
		{
			var newResult = _fixConnector.NewOrderAsync(new NewOrderRequest()
			{
				Side = side == Sides.Buy ? Side.Buy : Side.Sell,
				Symbol = Symbol.Parse(symbol),
				Type = OrdType.Market,
				Quantity = quantity
			}).Result;
			var result = _taskCompletionManager.CreateCompletableTask<ExecutionReport>(newResult.OrderId).Result;
			return result.FulfilledQuantity ?? 0;
		}

		public void OrderMultipleCloseBy(string symbol)
		{
			var symbolInfo = GetSymbolInfo(symbol);
			if (symbolInfo.SumContracts == 0) return;
			var side = symbolInfo.SumContracts > 0 ? Sides.Sell : Sides.Buy;
			SendMarketOrderRequest(symbol, side, (decimal) Math.Abs(symbolInfo.SumContracts));
		}

		public SymbolData GetSymbolInfo(string symbol)
		{
			return SymbolInfos.GetOrAdd(symbol, new SymbolData());
		}

		private async void _fixConnector_SocketClosed(object sender, ClosedEventArgs e)
		{
			if (e.Error == null) return;
			await _fixConnector.ConnectPricingAsync();
			await _fixConnector.ConnectTradingAsync();
		}

		private Sides InvSide(Sides side)
		{
			return side == Sides.Buy ? Sides.Sell : Sides.Buy;
		}
	}
}

using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;

namespace TradeSystem.Backtester
{
	public class Connector : FixApiConnectorBase
	{
		public class Reader
		{
			public TextReader TextReader { get; set; }
			public CsvHelper.CsvReader CsvReader { get; set; }
			public object LastRecord { get; set; }

			public Reader(string file, string delimeter)
			{
				TextReader = new StreamReader(file, true);
				CsvReader = new CsvHelper.CsvReader(TextReader);
				CsvReader.Configuration.Delimiter = delimeter;
			}

			public void Disconnect()
			{
				CsvReader.Dispose();
			}
		}

		private const string FolderPath = "Backtester";
		private readonly ConcurrentDictionary<BacktesterInstrumentConfig, Reader> _csvReaders =
			new ConcurrentDictionary<BacktesterInstrumentConfig, Reader>();

		private readonly BacktesterAccount _account;

		public override int Id => _account?.Id ?? 0;
		public override string Description => _account?.Description;
		private bool _isConnected = false;
		public override bool IsConnected => _isConnected;

		public Connector(BacktesterAccount account)
		{
			_account = account;
			Directory.CreateDirectory(FolderPath);
		}

		public void Connect()
		{
			lock (this)
			{
				if (_isConnected) return;
				_isConnected = true;
			}

			foreach (var ic in _account.InstrumentConfigs)
			{
				var file = $"{FolderPath}/{ic.FileName}";
				_csvReaders.GetOrAdd(ic, key => new Reader(file, ic.GetDelimeter()));
			}

			OnConnectionChanged(ConnectionStates.Connected);
		}
		public override void Disconnect()
		{
			foreach (var reader in _csvReaders)
				reader.Value.Disconnect();
			_csvReaders.Clear();
			_isConnected = false;
			OnConnectionChanged(ConnectionStates.Disconnected);
		} 

		public override void Subscribe(string symbol) => Logger.Debug($"{Description} Connector.Subscribe({symbol})");

		public override Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity)
		{
			var price = GetLastTick(symbol).GetPrice(side);
			var response = new OrderResponse()
			{
				Side = side,
				OrderPrice = price,
				OrderedQuantity = quantity,
				AveragePrice = price,
				FilledQuantity = quantity
			};
			LogOrderResponse(symbol, response);
			return Task.FromResult(response);
		}
		public override Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, int timeout,
			int retryCount, int retryPeriod) => SendMarketOrderRequest(symbol, side, quantity);
		public override Task<OrderResponse> SendAggressiveOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice,
			decimal deviation, decimal priceDiff, int timeout, int retryCount, int retryPeriod) =>
			SendMarketOrderRequest(symbol, side, quantity);
		public override Task<OrderResponse> SendDelayedAggressiveOrderRequest(string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff, decimal correction, int timeout, int retryCount,
			int retryPeriod) => SendMarketOrderRequest(symbol, side, quantity);
		public override Task<OrderResponse> SendGtcLimitOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice,
			decimal deviation, decimal priceDiff, int timeout, int retryCount, int retryPeriod) =>
			SendMarketOrderRequest(symbol, side, quantity);

		public void Start()
		{
		}

		public void Pause()
		{
		}

		public void Stop()
		{
		}

		private void LogOrderResponse(string symbol, OrderResponse response)
		{
			Logger.Debug($"\t{Description}" +
			             $"\t{_account.UtcNow:yyyy-MM-dd HH:mm:ss.ffff}" +
						 $"\t{symbol}" +
			             $"\t{response.Side}" +
			             $"\t{response.FilledQuantity}");
		}
	}
}

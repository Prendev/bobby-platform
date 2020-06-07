using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
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
		private readonly EventWaitHandle _waitHandle = new AutoResetEvent(false);
		private readonly EventWaitHandle _pauseHandle = new ManualResetEvent(true);
		private volatile CancellationTokenSource _cancellation;
		private bool _isConnected;
		private bool _isStarted;
		private int _index;

		private readonly BacktesterAccount _account;

		public override int Id => _account?.Id ?? 0;
		public override string Description => _account?.Description;
		public override bool IsConnected => _isConnected;


		public Connector(BacktesterAccount account)
		{
			_account = account;
			Directory.CreateDirectory(FolderPath);
		}

		public void Connect()
		{
			_isConnected = true;
			OnConnectionChanged(ConnectionStates.Connected);
		}
		public override void Disconnect()
		{
			_isStarted = false;
			_cancellation?.Cancel(false);
			_cancellation?.Dispose();
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

		public override void OnTickProcessed() => _waitHandle.Set();

		public void Start()
		{
			_pauseHandle.Set();
			_waitHandle.Set();
			lock (this)
			{
				if (_isStarted) return;
				_isStarted = true;
			}

			_cancellation = new CancellationTokenSource();
			new Thread(() => Loop(_cancellation.Token))
			{
				Name = "Backtester",
				IsBackground = true,
				Priority = ThreadPriority.BelowNormal
			}.Start();
		}

		private SortedDictionary<DateTime, List<Tick>> ReadTicks()
		{
			try
			{
				var ticks = new SortedDictionary<DateTime, List<Tick>>();
				var csvReaders = new ConcurrentDictionary<BacktesterInstrumentConfig, Reader>();
				foreach (var ic in _account.InstrumentConfigs)
				{
					var folder = $"{FolderPath}/{ic.Folder}";
					var files = Directory.GetFiles(folder).OrderBy(f => f).ToList();
					if (files.Count <= _index) continue;
					csvReaders.GetOrAdd(ic, key => new Reader(files.ElementAt(_index), ic.GetDelimeter()));
				}

				_index++;

				foreach (var reader in csvReaders)
				{
					var csvReader = reader.Value.CsvReader;
					var config = reader.Key;
					while (csvReader.Read())
					{
						var dt = csvReader.GetField<string>(config.DateTimeColumn);
						var a = csvReader.GetField<string>(config.AskColumn);
						var b = csvReader.GetField<string>(config.BidColumn);

						var dateTime = DateTime.ParseExact(dt, config.DateTimeFormat, CultureInfo.InvariantCulture);
						DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
						var ask = decimal.Parse(a);
						var bid = decimal.Parse(b);

						var tick = new Tick()
						{
							Time = dateTime,
							Symbol = config.Symbol,
							Ask = ask,
							Bid = bid,
							AskVolume = 1,
							BidVolume = 1
						};
						if (ticks.ContainsKey(tick.Time)) ticks[tick.Time].Add(tick);
						else ticks[tick.Time] = new List<Tick> {tick};
					}

					reader.Value.Disconnect();
				}

				return ticks;
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.ReadTicks error", e);
				return new SortedDictionary<DateTime, List<Tick>>();
			}
		}

		private void Loop(CancellationToken token)
		{
			_index = 0;
			LastTicks.Clear();
			while (true)
			{
				var ticks = ReadTicks();
				if (!ticks.Any()) break;
				foreach (var subTicks in ticks)
				{
					foreach (var tick in subTicks.Value)
					{
						_pauseHandle.WaitOne();
						_waitHandle.WaitOne();
						if (token.IsCancellationRequested) break;

						LastTicks.AddOrUpdate(tick.Symbol, key => tick, (key, old) => tick);
						OnNewTick(new NewTick { Tick = tick });
						if (_account.SleepInMs > 0) Thread.Sleep(_account.SleepInMs);
						if (token.IsCancellationRequested) break;
					}

					if (token.IsCancellationRequested) break;
				}
			}
			_isStarted = false;
		}

		public void Pause() => _pauseHandle.Reset();

		public void Stop()
		{
			_index = 0;
			_cancellation?.Cancel(false);
			_cancellation?.Dispose();
			_waitHandle.Set();
			_pauseHandle.Set();
			LastTicks.Clear();
		}

		private void LogOrderResponse(string symbol, OrderResponse response)
		{
			Logger.Debug($"\t{Description}" +
			             $"\t{_account.UtcNow:yyyy-MM-dd HH:mm:ss.ffff}" +
						 $"\t{symbol}" +
			             $"\t{response.Side}" +
			             $"\t{response.FilledQuantity}" +
						 $"\t{response.AveragePrice}");
		}
	}
}

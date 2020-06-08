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

		public BacktesterAccount Account { get; }

		private const string FolderPath = "Backtester";
		private readonly Random _rnd = new Random();
		private readonly EventWaitHandle _waitHandle = new AutoResetEvent(false);
		private readonly EventWaitHandle _slippageHandle = new AutoResetEvent(false);
		private readonly EventWaitHandle _pauseHandle = new ManualResetEvent(true);
		private volatile CancellationTokenSource _cancellation;
		private bool _isConnected;
		private bool _isStarted;
		private int _index;
		private volatile int _instanceCount;
		private volatile bool _slippage;

		public override int Id => Account?.Id ?? 0;
		public override string Description => Account?.Description;
		public override bool IsConnected => _isConnected;


		public Connector(BacktesterAccount account)
		{
			Account = account;
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
		public override Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity)
		{
			var instrumentConfig = Account.InstrumentConfigs.First(ic => ic.Symbol == symbol);
			var orderPrice = GetLastTick(symbol).GetPrice(side);
			Slippage(instrumentConfig);
			var fillPrice = GetLastTick(symbol).GetPrice(side);

			var response = new OrderResponse()
			{
				Side = side,
				OrderPrice = orderPrice,
				OrderedQuantity = quantity,
				AveragePrice = fillPrice,
				FilledQuantity = quantity
			};
			BacktesterLogger.Log(this, symbol, response);
			return Task.FromResult(response);
		}

		private void Slippage(BacktesterInstrumentConfig instrumentConfig)
		{
			if (Account.Instances > 1) return;
			if (!LastTicks.TryGetValue(instrumentConfig.Symbol, out var tick)) return;

			var min = instrumentConfig.MinSlippageInMs;
			var max = Math.Max(min, instrumentConfig.MaxSlippageInMs);
			var slippage = min < 0 || max <= 0 ? 0 : _rnd.Next(min, max);
			if (slippage <= 0) return;

			_slippage = true;
			var slippageTime = tick.Time.AddMilliseconds(slippage);
			while (_slippage && _isStarted && LastTicks.Max(t => t.Value.Time) < slippageTime)
			{
				OnTickProcessed();
				_slippageHandle.WaitOne();
			}

			_slippage = false;
		}

		public override void OnTickProcessed()
		{
			lock (this)
			{
				_instanceCount++;
				if (_instanceCount < Account.Instances) return;
				_instanceCount = 0;
				_waitHandle.Set();
			}
		}

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
				foreach (var ic in Account.InstrumentConfigs)
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
						if (Account.TickSleepInMs > 0) Thread.Sleep(Account.TickSleepInMs);

						if (_slippage) _slippageHandle.Set();
						else OnNewTick(new NewTick { Tick = tick });

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
			_slippage = false;
			_cancellation?.Cancel(false);
			_cancellation?.Dispose();
			_slippageHandle.Set();
			_waitHandle.Set();
			_pauseHandle.Set();
			LastTicks.Clear();
		}
	}
}

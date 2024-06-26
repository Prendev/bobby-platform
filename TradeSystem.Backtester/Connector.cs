﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
			public FileStream FileStream { get; set; }
			public StreamReader StreamReader { get; set; }

			public Reader(string file)
			{
				FileStream = File.OpenRead(file);
				StreamReader = new StreamReader(FileStream, Encoding.UTF8, true, 128);
			}

			public void Disconnect()
			{
				try
				{
					StreamReader.Dispose();
					FileStream.Dispose();
				}
				catch { }
			}
		}

		public BacktesterAccount Account { get; }

		private const string FolderPath = "Backtester";
		private readonly Random _rnd = new Random();
		private readonly EventWaitHandle _waitHandle = new AutoResetEvent(false);
		private readonly EventWaitHandle _pauseHandle = new ManualResetEvent(true);
		private readonly EventWaitHandle _slippageHandle = new AutoResetEvent(false);
		private volatile CancellationTokenSource _cancellation;
		private bool _isConnected;
		private bool _isStarted;
		private int _index;
		private int _instanceCount;
		private int _slippageCount;

		private readonly ConcurrentDictionary<LimitResponse, object> _limits =
			new ConcurrentDictionary<LimitResponse, object>();

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
			if (_cancellation?.IsCancellationRequested == false)
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
			};
			if (!Reject(instrumentConfig))
			{
				response.AveragePrice = fillPrice;
				response.FilledQuantity = quantity;
			}

			BacktesterLogger.Log(this, symbol, response);
			return Task.FromResult(response);
		}

		public override Task<LimitResponse> PutNewOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice)
		{
			var limit = new LimitResponse()
			{
				Symbol = symbol,
				Side = side,
				OrderedQuantity = quantity,
				OrderPrice = limitPrice,
			};

			lock (this)
			{
				_limits.AddOrUpdate(limit, _ => null, (_, __) => null);
				CheckLimit(limit);
			}
			return Task.FromResult(limit);
		}

		public override Task<bool> ChangeLimitPrice(LimitResponse response, decimal limitPrice)
		{
			response.OrderPrice = limitPrice;
			return Task.FromResult(true);
		}

		public override Task<bool> CancelLimit(LimitResponse response)
		{
			_limits.TryRemove(response, out var _);
			return Task.FromResult(true);
		}

		private void CheckLimit(LimitResponse response)
		{
			if (!LastTicks.TryGetValue(response.Symbol, out var tick)) return;

			if (response.Side == Sides.Buy && tick.Ask >= response.OrderPrice)
			{
				response.FilledQuantity = response.OrderedQuantity;
				_limits.TryRemove(response, out var _);
				OnLimitFill(new LimitFill()
				{
					LimitResponse = response,
					Price = tick.Ask,
					Quantity = response.FilledQuantity
				});
				BacktesterLogger.Log(this, response);
			}

			else if (response.Side == Sides.Sell && tick.Bid <= response.OrderPrice)
			{
				response.FilledQuantity = response.OrderedQuantity;
				_limits.TryRemove(response, out var _);
				OnLimitFill(new LimitFill()
				{
					LimitResponse = response,
					Price = tick.Bid,
					Quantity = response.FilledQuantity
				});
				BacktesterLogger.Log(this, response);
			}
		}

		private void Slippage(BacktesterInstrumentConfig instrumentConfig)
		{
			if (!LastTicks.TryGetValue(instrumentConfig.Symbol, out var tick)) return;

			var min = instrumentConfig.MinSlippageInMs;
			var max = Math.Max(min, instrumentConfig.MaxSlippageInMs);
			var slippage = min < 0 || max <= 0 ? 0 : _rnd.Next(min, max);
			if (slippage <= 0) return;

			Interlocked.Increment(ref _slippageCount);
			var slippageTime = tick.Time.AddMilliseconds(slippage);
			while (_isStarted && LastTicks.Max(t => t.Value.Time) < slippageTime)
			{
				OnTickProcessed();
				_slippageHandle.WaitOne();
			}
			Interlocked.Decrement(ref _slippageCount);
		}

		private bool Reject(BacktesterInstrumentConfig instrumentConfig)
		{
			var reject = instrumentConfig.RejectPercentage;
			reject = Math.Max(0, reject);
			reject = Math.Min(100, reject);
			if (reject <= 0) return false;
			return _rnd.Next(0, 100) <= reject;
		}

		public override void OnTickProcessed()
		{
			lock (this)
			{
				Interlocked.Increment(ref _instanceCount);
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
				var readers = new ConcurrentDictionary<BacktesterInstrumentConfig, Reader>();
				foreach (var ic in Account.InstrumentConfigs)
				{
					var folder = $"{FolderPath}/{ic.Folder}";
					var files = Directory.GetFiles(folder).OrderBy(f => f).ToList();
					if (files.Count <= _index) continue;
					readers.GetOrAdd(ic, key => new Reader(files.ElementAt(_index)));
					if (_index != 0) continue;
					Logger.Debug($"{Description} backtester read {ic.Symbol} from files: {files.Count}");
				}

				if(!readers.Any()) return ticks;
				_index++;

				var lastCount = 0;
				foreach (var reader in readers)
				{
					string line;
					while ((line = reader.Value.StreamReader.ReadLine()) != null)
					{
						var tick = ReadTick(reader.Key, line);
						if (!tick.HasValue) continue;
						if (tick == null) continue;
						if (ticks.ContainsKey(tick.Time)) ticks[tick.Time].Add(tick);
						else ticks[tick.Time] = new List<Tick> { tick };

						if (ticks.Count % 10000 == 0 && ticks.Count != lastCount)
						{
							lastCount = ticks.Count;
							Logger.Debug($"{Description} backtester read {_index}. files with count: {ticks.Count}");
						}
					}

					reader.Value.Disconnect();
				}

				Logger.Debug($"{Description} backtester read {_index}. files DONE with count: {ticks.Count}");

				return ticks;
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.ReadTicks error", e);
				return new SortedDictionary<DateTime, List<Tick>>();
			}
		}
		private Tick ReadTick(
			BacktesterInstrumentConfig config,
			string line)
		{
			try
			{
				var columns = line.Split(new[] { config.GetDelimeter() }, StringSplitOptions.None);
				var dt = columns[config.DateTimeColumn];
				var a = columns[config.AskColumn];
				var b = columns[config.BidColumn];

				var dateTime = DateTime.ParseExact(dt, config.GetDateTimeFormat(), CultureInfo.InvariantCulture);
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
				return tick;
			}
			catch
			{
				return null;
			}
		}

		private void Loop(CancellationToken token)
		{
			_index = 0;
			LastTicks.Clear();
			while (true)
			{
				if (token.IsCancellationRequested) break;
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

						foreach (var response in _limits.Keys.ToList())
							CheckLimit(response);

						OnNewTick(new NewTick { Tick = tick });
						Enumerable.Range(0, _slippageCount).ToList().ForEach(i => _slippageHandle.Set());
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
			_slippageCount = 0;
			if (_cancellation?.IsCancellationRequested == false)
				_cancellation?.Cancel(false);
			_cancellation?.Dispose();
			_waitHandle.Set();
			_pauseHandle.Set();
			_slippageHandle.Reset();
			LastTicks.Clear();
		}
	}
}

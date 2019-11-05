using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.CTraderIntegration.Dto;
using TradeSystem.CTraderIntegration.Services;

namespace TradeSystem.CTraderIntegration
{
    public class Connector : ConnectorBase
	{
		private readonly TaskCompletionManager<string> _taskCompletionManager;
		private readonly ITradingAccountsService _tradingAccountsService;
        private readonly CTraderClientWrapper _cTraderClientWrapper;
        private readonly AccountInfo _accountInfo;
        private long AccountId => _accountInfo?.AccountId ?? 0;

        /// <summary>
        /// The key is access token
        /// </summary>
        public static ConcurrentDictionary<string, Lazy<List<AccountData>>> BalanceAccounts =
            new ConcurrentDictionary<string, Lazy<List<AccountData>>>();
		private readonly List<string> _symbols = new List<string>();
		private readonly ConcurrentDictionary<string, Tick> _lastTicks =
			new ConcurrentDictionary<string, Tick>();

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description;
		public override bool IsConnected => _cTraderClientWrapper?.IsConnected == true && AccountId > 0;
        public ConcurrentDictionary<long, Position> Positions { get; }

		public Connector(
            AccountInfo accountInfo,
            CTraderClientWrapper cTraderClientWrapper,
            ITradingAccountsService tradingAccountsService)
        {
            _tradingAccountsService = tradingAccountsService;
            _accountInfo = accountInfo;
            _cTraderClientWrapper = cTraderClientWrapper;
	        _taskCompletionManager = new TaskCompletionManager<string>(100, 30000);
			Positions = new ConcurrentDictionary<long, Position>();
        }

		public override void Disconnect()
		{
			_cTraderClientWrapper.CTraderClient.OnTick -= CTraderClient_OnTick;
			_cTraderClientWrapper.CTraderClient.OnPosition -= CTraderClient_OnPosition;
            _cTraderClientWrapper.CTraderClient.OnError -= CTraderClient_OnError;
            _cTraderClientWrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(AccountId);
			OnConnectionChanged(ConnectionStates.Disconnected);
		}

		public override Tick GetLastTick(string symbol)
	    {
		    try
			{
				var lastTick = _lastTicks.GetOrAdd(symbol, (Tick)null);
				if (lastTick != null) return lastTick;

				return SubscribeAsync(symbol).Result;
			}
		    catch (Exception e)
			{
				Logger.Error($"{Description} Connector.GetLastTick({symbol})", e);
				return null;
			}
		}

		public override void Subscribe(string symbol)
		{
			lock (_symbols)
				if (!_symbols.Contains(symbol))
					_symbols.Add(symbol);
			lock (_cTraderClientWrapper.CTraderClient)
				_cTraderClientWrapper.CTraderClient.SendSubscribeForSpotsRequest(_accountInfo.AccessToken, AccountId, symbol);
			Logger.Debug($"{Description} Connector.Subscribe({symbol})");
		}
		private async Task<Tick> SubscribeAsync(string symbol)
		{
			lock (_symbols)
				if (!_symbols.Contains(symbol))
					_symbols.Add(symbol);

			var task = _taskCompletionManager.CreateCompletableTask<Tick>(symbol);
			lock (_cTraderClientWrapper.CTraderClient)
				_cTraderClientWrapper.CTraderClient.SendSubscribeForSpotsRequest(_accountInfo.AccessToken, AccountId, symbol);
			var lastTick = await task;
			Logger.Debug($"{Description} Connector.SubscribeAsync({symbol})");
			return lastTick;
		}

		public void Connect()
        {
            if (!IsConnected)
            {
                Logger.Error($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) FAILED to connect");
				OnConnectionChanged(ConnectionStates.Error);
                return;
            }
            Logger.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) connected");

	        lock (_cTraderClientWrapper.CTraderClient)
	        {
		        _cTraderClientWrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(AccountId);
		        _cTraderClientWrapper.CTraderClient.SendSubscribeForTradingEventsRequest(_accountInfo.AccessToken, AccountId);
	        }

	        _cTraderClientWrapper.CTraderClient.OnTick -= CTraderClient_OnTick;
			_cTraderClientWrapper.CTraderClient.OnPosition -= CTraderClient_OnPosition;
            _cTraderClientWrapper.CTraderClient.OnError -= CTraderClient_OnError;
	        _cTraderClientWrapper.CTraderClient.OnTick += CTraderClient_OnTick;
			_cTraderClientWrapper.CTraderClient.OnPosition += CTraderClient_OnPosition;
            _cTraderClientWrapper.CTraderClient.OnError += CTraderClient_OnError;

            var positions = _tradingAccountsService.GetPositionsAsync(new AccountRequest
            {
                AccessToken = _accountInfo.AccessToken,
                BaseUrl = _cTraderClientWrapper.PlatformInfo.AccountsApi,
                AccountId = AccountId
            }).Result;
	        lock (_symbols)
	        {
		        foreach (var symbol in _symbols)
			        Subscribe(symbol);
	        }
			Logger.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) positions acquired");

            foreach (var p in positions)
            {
                Positions.GetOrAdd(p.positionId,
                    new Position
                    {
                        Id = p.positionId,
                        Volume = p.volume,
                        RealVolume = p.volume / 100 * (p.tradeSide == "BUY" ? 1 : -1),
                        Comment = p.comment,
                        Symbol = p.symbolName,
                        Side = p.tradeSide == "BUY" ? Sides.Buy : Sides.Sell
                    });
            }

	        OnConnectionChanged(ConnectionStates.Connected);
		}

		public PositionResponse SendMarketOrderRequest(string symbol, Sides side, long volume, int maxRetryCount, int retryPeriodInMs)
		{
			var type = side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;
			return SendOrderRequestAsync(symbol, type, volume, null, 0, maxRetryCount, retryPeriodInMs).Result;
		}
		public PositionResponse SendMarketRangeOrderRequest(string symbol, Sides side, long volume, decimal price, int slippageInPips, int maxRetryCount, int retryPeriodInMs)
		{
			var type = side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;
			return SendOrderRequestAsync(symbol, type, volume, price, slippageInPips, maxRetryCount, retryPeriodInMs).Result;
		}
		private async Task<PositionResponse> SendOrderRequestAsync(string symbol, ProtoTradeSide type, long volume, decimal? price, int slippageInPips, int maxRetryCount, int retryPeriodInMs)
		{
			var clientMsgId = Guid.NewGuid().ToString();
			var retValue = new PositionResponse()
	        {
		        Pos = new Position
		        {
			        Volume = 0,
			        Comment = clientMsgId,
			        Symbol = symbol,
			        Side = type == ProtoTradeSide.BUY ? Sides.Buy : Sides.Sell
		        }
	        };

			// ReTry mechanism
			while (maxRetryCount-- >= 0)
			{
				var startTime = HiResDatetime.UtcNow;
				try
				{
				var task = _taskCompletionManager.CreateCompletableTask<Position>(clientMsgId);

					lock (_cTraderClientWrapper.CTraderClient)
					{
						if (price.HasValue)
							_cTraderClientWrapper.CTraderClient.SendMarketRangeOrderRequest(_accountInfo.AccessToken, AccountId,
								symbol, type, volume - retValue.Pos.Volume, (double)price, slippageInPips, clientMsgId);
						else
							_cTraderClientWrapper.CTraderClient.SendMarketOrderRequest(_accountInfo.AccessToken, AccountId, symbol,
								type, volume - retValue.Pos.Volume, clientMsgId);
					}

					var pos = await task;
					// Return, unexpected null
					if (pos == null) throw new TimeoutException("Unexpected null for position");
					retValue.Pos.Volume += pos.Volume;
					if (pos.Volume > 0) retValue.Pos.Ids.Add(pos.Id);

					if (price.HasValue)
						Logger.Debug(
							$"{Description} Connector.SendMarketRangeOrderRequest({symbol}, {type}, {volume}, {price}, {slippageInPips})" +
							$" is successful with id {pos.Id} and {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time");
					else
						Logger.Debug($"{Description} Connector.SendMarketOrderRequest({symbol}, {type}, {volume})" +
						             $" is successful with id {pos.Id} and {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time");

					if (retValue.Pos.Volume == volume) return retValue;
				}
				catch (TimeoutException e)
				{
					if (price.HasValue)
						Logger.Error(
							$"{Description} Connector.SendMarketRangeOrderRequest({symbol}, {type}, {volume}, {price}, {slippageInPips})" +
							$" exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);
					else
						Logger.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {type}, {volume})" +
						             $" exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);

					retValue.IsUnfinished = true;
					return retValue;
				}
				catch (Exception e)
				{
					if (price.HasValue)
						Logger.Error(
							$"{Description} Connector.SendMarketRangeOrderRequest({symbol}, {type}, {volume}, {price}, {slippageInPips})" +
							$" exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);
					else
						Logger.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {type}, {volume})" +
						             $" exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);
				}

				// If no more try return or sleep
				if (maxRetryCount < 0) return retValue;
				Thread.Sleep(retryPeriodInMs);
			}

			return retValue;
		}

		public PositionResponse SendClosePositionRequest(long id, int maxRetryCount, int retryPeriodInMs)
		{
			if (!Positions.TryGetValue(id, out var position)) return new PositionResponse();
			if (position.IsClosed) return new PositionResponse { Pos = position };
			return SendClosePositionRequestAsync(position, maxRetryCount, retryPeriodInMs).Result;
		}
		private async Task<PositionResponse> SendClosePositionRequestAsync(Position position, int maxRetryCount, int retryPeriodInMs)
		{
			if (position == null)
			{
				Logger.Error($"{_accountInfo.Description} Connector.SendClosePositionRequests position is NULL");
				return new PositionResponse();
			}

			var startTime = HiResDatetime.UtcNow;
			try
			{
				var task = _taskCompletionManager.CreateCompletableTask<Position>(position.Id.ToString());
				lock (_cTraderClientWrapper.CTraderClient)
					_cTraderClientWrapper.CTraderClient.SendClosePositionRequest(_accountInfo.AccessToken, AccountId,
						position.Id, position.Volume, position.Id.ToString());
				position = await task;
				Logger.Debug(
					$"{_accountInfo.Description} Connector.SendClosePositionRequests({position.Id}, {position.Comment})" +
					$" is successful with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time");
				if (position.IsClosed || maxRetryCount <= 0) return new PositionResponse() {Pos = position};
				Thread.Sleep(retryPeriodInMs);
				return await SendClosePositionRequestAsync(position, --maxRetryCount, retryPeriodInMs);
			}
			catch (TimeoutException e)
			{
				Logger.Error(
					$"{_accountInfo.Description} Connector.SendClosePositionRequests({position.Id}, {position.Comment})" +
					$" exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);

				if (maxRetryCount <= 0) return new PositionResponse() { Pos = position, IsUnfinished = true };
				Thread.Sleep(retryPeriodInMs);
				return await SendClosePositionRequestAsync(position, --maxRetryCount, retryPeriodInMs);
			}
			catch (Exception e)
			{
				Logger.Error(
					$"{_accountInfo.Description} Connector.SendClosePositionRequests({position.Id}, {position.Comment})" +
					$" exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);
				if (maxRetryCount <= 0) return new PositionResponse() { Pos = position };
				Thread.Sleep(retryPeriodInMs);
				return await SendClosePositionRequestAsync(position, --maxRetryCount, retryPeriodInMs);
			}
		}

		private void CTraderClient_OnTick(ProtoOASpotEvent tick)
		{
			var t = new Tick
			{
				Symbol = tick.SymbolName,
				Ask = (decimal)tick.AskPrice,
				Bid = (decimal)tick.BidPrice,
				Time = HiResDatetime.UtcNow
			};
			_lastTicks.AddOrUpdate(t.Symbol, key => t, (key, old) => t);
			_taskCompletionManager.SetResult(t.Symbol, t, true);
			OnNewTick(new NewTick { Tick = t });
		}

		private void CTraderClient_OnPosition(ProtoOAPosition p)
        {
            if (p.AccountId != AccountId) return;
            if (p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN &&
                p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_CLOSED) return;

	        var position = UpdatePosition(p);
	        if (p.PositionStatus == ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN)
				_taskCompletionManager.SetResult(p.Comment, position);
			else if (p.PositionStatus == ProtoOAPositionStatus.OA_POSITION_STATUS_CLOSED)
		        _taskCompletionManager.SetResult(p.PositionId.ToString(), position);

			OnNewPosition(new NewPosition
            {
                AccountType = AccountTypes.Ct,
                Position = position,
                Action = p.PositionStatus == ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN ? NewPositionActions.Open : NewPositionActions.Close,
            });
		}
		private Position UpdatePosition(ProtoOAPosition order)
		{
			var position = new Position
			{
				Id = order.PositionId,
				Volume = order.Volume,
				RealVolume = order.Volume * (order.TradeSide == ProtoTradeSide.BUY ? 1 : -1) / 100,
				Symbol = order.SymbolName,
				Side = order.TradeSide == ProtoTradeSide.BUY ? Sides.Buy : Sides.Sell,
				//OpenTime = order.OpenTimestamp,
				OpenPrice = (decimal)order.EntryPrice,
				//CloseTime = order.CloseTime,
				//ClosePrice = 0,
				IsClosed = order.PositionStatus == ProtoOAPositionStatus.OA_POSITION_STATUS_CLOSED,
				Commission = order.Commission,
				Swap = order.Swap,
				Comment = order.Comment,
			};
			return Positions.AddOrUpdate(order.PositionId, t => position, (t, old) =>
			{
				old.Volume = order.Volume;
				old.IsClosed = order.HasCloseTimestamp;
				return old;
			});
		}

        private void CTraderClient_OnError(ProtoErrorRes error, string clientMsgId) => _taskCompletionManager.SetError(clientMsgId, new Exception(error.Description));

        public static DateTime CTraderTimestampToDatetime(long timestamp)
        {
            // Java timestamp is milliseconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(timestamp).ToLocalTime();
            return dtDateTime;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Dto;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public class Connector : ConnectorBase, IConnector
    {
        private readonly ITradingAccountsService _tradingAccountsService;
        private readonly ConcurrentDictionary<string, MarketOrder> _marketOrders = new ConcurrentDictionary<string, MarketOrder>();
        private readonly CTraderClientWrapper _cTraderClientWrapper;
        private readonly AccountInfo _accountInfo;
        private long AccountId => _accountInfo?.AccountId ?? 0;

        /// <summary>
        /// The key is access token
        /// </summary>
        public static ConcurrentDictionary<string, Lazy<List<AccountData>>> BalanceAccounts =
            new ConcurrentDictionary<string, Lazy<List<AccountData>>>();

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
            Positions = new ConcurrentDictionary<long, Position>();
        }

		public override void Disconnect()
        {
            _cTraderClientWrapper.CTraderClient.OnPosition -= CTraderClient_OnPosition;
            _cTraderClientWrapper.CTraderClient.OnError -= OnError;
            lock (_cTraderClientWrapper.CTraderClient)
            {
                _cTraderClientWrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(AccountId);
            }
            Logger.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) disconnected");
        }

		public override Tick GetLastTick(string symbol)
	    {
		    throw new NotImplementedException();
	    }

		public override void Subscribe(string symbol)
	    {
		    throw new NotImplementedException();
	    }

	    public bool Connect()
        {
            if (!IsConnected)
            {
                Logger.Error($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) FAILED to connect");
                return false;
            }
            Logger.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) connected");

            _cTraderClientWrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(AccountId);
            _cTraderClientWrapper.CTraderClient.SendSubscribeForTradingEventsRequest(_accountInfo.AccessToken, AccountId);

            _cTraderClientWrapper.CTraderClient.OnPosition -= CTraderClient_OnPosition;
            _cTraderClientWrapper.CTraderClient.OnError -= OnError;
            _cTraderClientWrapper.CTraderClient.OnPosition += CTraderClient_OnPosition;
            _cTraderClientWrapper.CTraderClient.OnError += OnError;

            var positions = _tradingAccountsService.GetPositions(new AccountRequest
            {
                AccessToken = _accountInfo.AccessToken,
                BaseUrl = _cTraderClientWrapper.PlatformInfo.AccountsApi,
                AccountId = AccountId
            });
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

            return IsConnected;
        }

        public long GetOpenContracts(string symbol)
        {
            return Positions.Where(p => p.Value.Symbol == symbol && !p.Value.IsClosed).Sum(p => p.Value.RealVolume);
        }

        public double GetBalance()
        {
            if (!IsConnected) return 0;

            var accounts = BalanceAccounts.GetOrAdd(_accountInfo.AccessToken,
                accessToken => new Lazy<List<AccountData>>(() =>
                {
                    var accs = _tradingAccountsService
                        .GetAccounts(new BaseRequest
                        {
                            AccessToken = accessToken,
                            BaseUrl = _cTraderClientWrapper.PlatformInfo.AccountsApi
                        });

                    Logger.Debug($"Accounts acquired for access token: {accessToken}");
                    return accs;
                }, true));

            return (double)(accounts.Value.FirstOrDefault(a => a.accountId == AccountId)?.balance ?? 0);
        }

        public double GetPnl(DateTime from, DateTime to)
        {
            if (!IsConnected) return 0;
            Thread.Sleep(2000);

            var deals = _tradingAccountsService.GetDeals(new DealsRequest
            {
                AccessToken = _accountInfo.AccessToken,
                BaseUrl = _cTraderClientWrapper.PlatformInfo.AccountsApi,
                AccountId = AccountId,
                From = from,
                To = to
            });

            return (double) deals.Sum(deal => deal.GetNetProfit());
        }

        public string GetCurrency()
        {
            if (!IsConnected) return "";

            var accounts = BalanceAccounts.GetOrAdd(_accountInfo.AccessToken,
                accessToken => new Lazy<List<AccountData>>(() =>
                {
                    var accs = _tradingAccountsService
                        .GetAccounts(new BaseRequest
                        {
                            AccessToken = accessToken,
                            BaseUrl = _cTraderClientWrapper.PlatformInfo.AccountsApi
                        });

                    Logger.Debug($"Accounts acquired for access token: {accessToken}");
                    return accs;
                }, true));

            return accounts.Value.FirstOrDefault(a => a.accountId == AccountId)?.depositCurrency ?? "";
        }

        public void SendMarketOrderRequest(string symbol, ProtoTradeSide type, long volume, string clientOrderId, int maxRetryCount = 5, int retryPeriodInMs = 3000)
        {
            var clientMsgId = $"{AccountId}|{clientOrderId}";
            _marketOrders.GetOrAdd(clientMsgId, new MarketOrder
            {
                Symbol = symbol,
                Side = type ==  ProtoTradeSide.BUY ? Sides.Buy : Sides.Sell,
                Volume = volume,
                MaxRetryCount = maxRetryCount,
                RetryPeriodInMs = retryPeriodInMs
            });

            lock (_cTraderClientWrapper.CTraderClient)
            {
                _cTraderClientWrapper.CTraderClient.SendMarketOrderRequest(_accountInfo.AccessToken, AccountId, symbol,
                    type, volume, clientMsgId);
            }
        }

        public void SendMarketRangeOrderRequest(string symbol, ProtoTradeSide type, long volume, decimal price, int slippageInPips, string clientOrderId, int maxRetryCount = 5, int retryPeriodInMs = 3000)
        {
            var clientMsgId = $"{AccountId}|{clientOrderId}";
            _marketOrders.GetOrAdd(clientMsgId, new MarketOrder
            {
                Symbol = symbol,
                Side = type == ProtoTradeSide.BUY ? Sides.Buy : Sides.Sell,
                Volume = volume,
                Price = price,
                SlippageInPips = slippageInPips,
                MaxRetryCount = maxRetryCount,
                RetryPeriodInMs = retryPeriodInMs
            });

            lock (_cTraderClientWrapper.CTraderClient)
            {
                _cTraderClientWrapper.CTraderClient.SendMarketRangeOrderRequest(_accountInfo.AccessToken, AccountId,
                    symbol, type, volume, (double)price, slippageInPips, clientMsgId);
            }
        }

        public void SendClosePositionRequests(string clientOrderId, int maxRetryCount = 5, int retryPeriodInMs = 3000)
        {
            var clientMsgId = $"{AccountId}|{clientOrderId}";
            foreach (var pos in Positions.Where(p => p.Value.Comment == clientMsgId && !p.Value.IsClosed))
                SendClosePositionRequest(pos, Math.Abs(pos.Value.Volume), maxRetryCount, retryPeriodInMs);
        }

        private void SendClosePositionRequest(KeyValuePair<long, Position> pos, long volume, int maxRetryCount = 5, int retryPeriodInMs = 3000)
        {
            var clientMsgId = $"{AccountId}|{pos.Key}";
            pos.Value.CloseOrder = new RetryOrder { MaxRetryCount = maxRetryCount, RetryPeriodInMs = retryPeriodInMs };
            lock (_cTraderClientWrapper.CTraderClient)
            {
                _cTraderClientWrapper.CTraderClient.SendClosePositionRequest(_accountInfo.AccessToken, AccountId,
                    pos.Key, volume, clientMsgId);
            }
        }

        private void CTraderClient_OnPosition(ProtoOAPosition p)
        {
            if (p.AccountId != AccountId) return;
            if (p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN &&
                p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_CLOSED) return;

            var position = new Position
            {
                Id = p.PositionId,
                Volume = p.Volume,
                RealVolume = p.Volume * (p.TradeSide == ProtoTradeSide.BUY ? 1 : -1) / 100,
                Comment = p.Comment,
                Symbol = p.SymbolName,
                Side = p.TradeSide == ProtoTradeSide.BUY ? Sides.Buy : Sides.Sell
            };
            position = Positions.AddOrUpdate(p.PositionId, id => position, (id, old) => position);

            CheckMarketOrder(p);
            CheckCloseOrder(p, position);

            OnNewPosition(new NewPosition
            {
                AccountType = AccountTypes.Ct,
                Position = position,
                Action = p.PositionStatus == ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN ? NewPositionActions.Open : NewPositionActions.Close,
            });
        }

        private void CheckMarketOrder(ProtoOAPosition position)
        {
            var clientMsgId = position.Comment;
            if (string.IsNullOrWhiteSpace(clientMsgId)) return;
	        if (!_marketOrders.TryGetValue(clientMsgId, out var order)) return;
            if (order.Volume == position.Volume)
            {
                _marketOrders.TryRemove(clientMsgId, out order);
                return;
            }

            order.Volume -= position.Volume;
            RetryMarketOrder(order, clientMsgId);
        }

        private void CheckCloseOrder(ProtoOAPosition protoPos, Position ctPos)
        {
            if (ctPos.Volume == 0)
            {
                Positions.TryRemove(protoPos.PositionId, out ctPos);
                return;
            }
            ctPos.Volume = protoPos.Volume;
            RetryClose(ctPos);
        }

        private void OnError(ProtoErrorRes error, string clientMsgId)
        {
            if (string.IsNullOrWhiteSpace(clientMsgId)) return;

            MarketOrder marketOrder;
            if (_marketOrders.TryGetValue(clientMsgId, out marketOrder))
            {
                Logger.Info($"cTrader error: {error.Description}");
                RetryMarketOrder(marketOrder, clientMsgId);
                return;
            }

            long positionId;
            Position ctPos;
            var parts = clientMsgId.Split('|');
            if (long.TryParse(parts[1], out positionId) && Positions.TryGetValue(positionId, out ctPos))
                RetryClose(ctPos);
        }

        private void RetryMarketOrder(MarketOrder order, string clientMsgId)
        {
            order.RetryCount++;
            if (order.RetryCount > order.MaxRetryCount) return;
            if (DateTime.UtcNow - order.Time > new TimeSpan(0, 0, 0, 0, order.RetryPeriodInMs)) return;

            lock (_cTraderClientWrapper.CTraderClient)
            {
                if (order.Price > 0)
                    _cTraderClientWrapper.CTraderClient.SendMarketRangeOrderRequest(_accountInfo.AccessToken, AccountId, order.Symbol,
                        order.Side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL, order.Volume, (double)order.Price, order.SlippageInPips, clientMsgId);
                else _cTraderClientWrapper.CTraderClient.SendMarketOrderRequest(_accountInfo.AccessToken, AccountId,
                    order.Symbol, order.Side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL, order.Volume, clientMsgId);
            }
        }

        private void RetryClose(Position ctPos)
        {
            if (ctPos.CloseOrder == null) return;
            ctPos.CloseOrder.RetryCount++;
            if (ctPos.CloseOrder.RetryCount > ctPos.CloseOrder.MaxRetryCount) return;
            if (DateTime.UtcNow - ctPos.CloseOrder.Time > new TimeSpan(0, 0, 0, 0, ctPos.CloseOrder.RetryPeriodInMs)) return;

            lock (_cTraderClientWrapper.CTraderClient)
            {
                _cTraderClientWrapper.CTraderClient.SendClosePositionRequest(_accountInfo.AccessToken, AccountId,
                    ctPos.Id, ctPos.Volume, $"{AccountId}|{ctPos.Id}");
            }
        }

        public static DateTime CTraderTimestampToDatetime(long timestamp)
        {
            // Java timestamp is milliseconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(timestamp).ToLocalTime();
            return dtDateTime;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Dto;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public class Connector : IConnector
    {
        private readonly ILog _log;
        private readonly ITradingAccountsService _tradingAccountsService;
        private readonly ConcurrentDictionary<string, MarketOrder> _marketOrders = new ConcurrentDictionary<string, MarketOrder>();
        private readonly CTraderClientWrapper _cTraderClientWrapper;
        private readonly AccountInfo _accountInfo;
        private long AccountId => _accountInfo?.AccountId ?? 0;

        public string Description => _accountInfo?.Description;
        public bool IsConnected => _cTraderClientWrapper?.IsConnected == true && AccountId > 0;
        public ConcurrentDictionary<long, Position> Positions { get; }
        public event PositionEventHandler OnPosition;

        public Connector(
            AccountInfo accountInfo,
            CTraderClientWrapper cTraderClientWrapper,
            ITradingAccountsService tradingAccountsService,
            ILog log)
        {
            _tradingAccountsService = tradingAccountsService;
            Positions = new ConcurrentDictionary<long, Position>();
            _accountInfo = accountInfo;
            _cTraderClientWrapper = cTraderClientWrapper;
            _log = log;
        }

        public void Disconnect()
        {
            _cTraderClientWrapper.CTraderClient.OnPosition -= CTraderClient_OnPosition;
            _cTraderClientWrapper.CTraderClient.OnError -= OnError;
            lock (_cTraderClientWrapper.CTraderClient)
                _cTraderClientWrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(AccountId);
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) disconnected");
        }

        public bool Connect(AccountInfo accountInfo)
        {
            if (!IsConnected)
            {
                _log.Error($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) FAILED to connect");
                return false;
            }
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) connected");

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
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) positions acquired");

            foreach (var p in positions)
            {
                Positions.GetOrAdd(p.positionId,
                    new Position
                    {
                        AccountId = _accountInfo.Id,
                        AccountType = AccountTypes.Ct,
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
            return Positions.Where(p => p.Value.Symbol == symbol).Sum(p => p.Value.RealVolume);
        }

        public void SendMarketOrderRequest(string symbol, ProtoTradeSide type, long volume, string clientOrderId, int maxRetryCount = 5, int retryPeriodInMilliseconds = 3000)
        {
            var clientMsgId = $"{AccountId}|{clientOrderId}";
            _marketOrders.GetOrAdd(clientMsgId, new MarketOrder
            {
                Symbol = symbol,
                Side = type ==  ProtoTradeSide.BUY ? Sides.Buy : Sides.Sell,
                Volume = volume,
                MaxRetryCount = maxRetryCount,
                RetryPeriodInMilliseconds = retryPeriodInMilliseconds
            });

            lock (_cTraderClientWrapper.CTraderClient)
                _cTraderClientWrapper.CTraderClient.SendMarketOrderRequest(_accountInfo.AccessToken, AccountId, symbol, type, volume, clientMsgId);
        }


        public void SendMarketRangeOrderRequest(string symbol, ProtoTradeSide type, long volume, double price, int slippageInPips, string clientOrderId, int maxRetryCount = 5, int retryPeriodInMilliseconds = 3000)
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
                RetryPeriodInMilliseconds = retryPeriodInMilliseconds
            });

            lock (_cTraderClientWrapper.CTraderClient)
                _cTraderClientWrapper.CTraderClient.SendMarketRangeOrderRequest(_accountInfo.AccessToken, AccountId, symbol, type, volume,
                    price, slippageInPips, clientMsgId);
        }

        public void SendClosePositionRequests(string clientOrderId, int maxRetryCount = 5, int retryPeriodInMilliseconds = 3000)
        {
            var clientMsgId = $"{AccountId}|{clientOrderId}";
            foreach (var pos in Positions.Where(p => p.Value.Comment == clientMsgId))
                SendClosePositionRequest(pos.Key, Math.Abs(pos.Value.Volume), maxRetryCount, retryPeriodInMilliseconds);
        }

        private void SendClosePositionRequest(long positionId, long volume, int maxRetryCount = 5, int retryPeriodInMilliseconds = 3000)
        {
            var clientMsgId = $"{AccountId}|{positionId}";

            Position position;
            if(Positions.TryGetValue(positionId, out position))
                position.CloseOrder = new RetryOrder { MaxRetryCount = maxRetryCount, RetryPeriodInMilliseconds = retryPeriodInMilliseconds };

            lock (_cTraderClientWrapper.CTraderClient)
                _cTraderClientWrapper.CTraderClient.SendClosePositionRequest(_accountInfo.AccessToken, AccountId, positionId, volume, clientMsgId);
        }

        private void CTraderClient_OnPosition(ProtoOAPosition p)
        {
            if (p.AccountId != AccountId) return;
            if (p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN &&
                p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_CLOSED) return;

            var position = new Position
            {
                AccountId = _accountInfo.Id,
                AccountType = AccountTypes.Ct,
                Id = p.PositionId,
                Volume = p.Volume,
                RealVolume = p.Volume * (p.TradeSide == ProtoTradeSide.BUY ? 1 : -1) / 100,
                Comment = p.Comment,
                Symbol = p.SymbolName,
                Side = p.TradeSide == ProtoTradeSide.BUY ? Sides.Buy : Sides.Sell
            };
            var pos = Positions.AddOrUpdate(p.PositionId, id => position, (id, old) => position);

            OnPosition?.Invoke(this, new PositionEventArgs
            {
                Position = position,
                Action = p.PositionStatus == ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN ? PositionEventArgs.Actions.Open : PositionEventArgs.Actions.Close,
            });

            CheckMarketOrder(p);
            CheckCloseOrder(p, pos);
        }

        private void CheckMarketOrder(ProtoOAPosition position)
        {
            var clientMsgId = position.Comment;
            if (string.IsNullOrWhiteSpace(clientMsgId)) return;

            MarketOrder order;
            if (!_marketOrders.TryGetValue(clientMsgId, out order)) return;

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
                _log.Info($"cTrader error: {error.Description}");
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
            if (DateTime.UtcNow - order.Time > new TimeSpan(0, 0, 0, 0, order.RetryPeriodInMilliseconds)) return;

            if (order.Price > 0)
                _cTraderClientWrapper.CTraderClient.SendMarketRangeOrderRequest(_accountInfo.AccessToken, AccountId, order.Symbol,
                    order.Side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL, order.Volume, order.Price, order.SlippageInPips, clientMsgId);
            else _cTraderClientWrapper.CTraderClient.SendMarketOrderRequest(_accountInfo.AccessToken, AccountId,
                order.Symbol, order.Side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL, order.Volume, clientMsgId);
        }

        private void RetryClose(Position ctPos)
        {
            ctPos.CloseOrder.RetryCount++;
            if (ctPos.CloseOrder.RetryCount > ctPos.CloseOrder.MaxRetryCount) return;
            if (DateTime.UtcNow - ctPos.CloseOrder.Time > new TimeSpan(0, 0, 0, 0, ctPos.CloseOrder.RetryPeriodInMilliseconds)) return;

            _cTraderClientWrapper.CTraderClient.SendClosePositionRequest(_accountInfo.AccessToken, AccountId,
                ctPos.Id, ctPos.Volume, $"{AccountId}|{ctPos.Id}");
        }
    }
}

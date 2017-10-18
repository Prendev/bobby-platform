using System;
using System.Collections.Concurrent;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public class ConnectorRetryDecorator : IConnector
    {
        private readonly ILog _log;
        private readonly ConcurrentDictionary<string, MarketOrder> _marketOrders = new ConcurrentDictionary<string, MarketOrder>();
        private readonly CTraderClientWrapper _cTraderClientWrapper;
        private readonly Connector _connector;
        private readonly AccountInfo _accountInfo;
        private long AccountId => _accountInfo?.AccountId ?? 0;

        public string Description => _connector?.Description;
        public bool IsConnected => _connector?.IsConnected == true;
        public readonly ConcurrentDictionary<long, Position> Positions = new ConcurrentDictionary<long, Position>();
        public event PositionEventHandler OnOrder;

        public ConnectorRetryDecorator(
            AccountInfo accountInfo,
            CTraderClientWrapper cTraderClientWrapper,
            ITradingAccountsService tradingAccountsService,
            ILog log)
        {
            _accountInfo = accountInfo;
            _cTraderClientWrapper = cTraderClientWrapper;
            _log = log;

            _connector = new Connector(_accountInfo, _cTraderClientWrapper, tradingAccountsService, log);
        }

        public void Disconnect()
        {
            _connector.Disconnect();
        }

        public bool Connect(AccountInfo accountInfo)
        {
            if (!_connector.Connect()) return false;

            _cTraderClientWrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(AccountId);
            _cTraderClientWrapper.CTraderClient.SendSubscribeForTradingEventsRequest(_accountInfo.AccessToken, AccountId);

            _cTraderClientWrapper.CTraderClient.OnPosition -= OnPosition;
            _cTraderClientWrapper.CTraderClient.OnError -= OnError;
            _cTraderClientWrapper.CTraderClient.OnPosition += OnPosition;
            _cTraderClientWrapper.CTraderClient.OnError += OnError;

            foreach (var p in _connector.GetPositions())
            {
                Positions.GetOrAdd(p.positionId,
                    new Position
                    {
                        AccountDescription = _accountInfo.Description,
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
            _connector.SendMarketOrderRequest(symbol, type, volume, clientMsgId);
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
            _connector.SendMarketRangeOrderRequest(symbol, type, volume, price, slippageInPips, clientMsgId);
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

            _connector.SendClosePositionRequest(positionId, volume, clientMsgId);
        }

        private void OnPosition(ProtoOAPosition p)
        {
            if (p.AccountId != AccountId) return;
            if (p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN &&
                p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_CLOSED) return;

            var value = new Position
            {
                AccountDescription = _accountInfo.Description,
                Id = p.PositionId,
                Volume = p.Volume,
                RealVolume = p.Volume * (p.TradeSide == ProtoTradeSide.BUY ? 1 : -1) / 100,
                Comment = p.Comment,
                Symbol = p.SymbolName,
                Side = p.TradeSide == ProtoTradeSide.BUY ? Sides.Buy : Sides.Sell
            };
            var pos = Positions.AddOrUpdate(p.PositionId, id => value, (id, old) => value);

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

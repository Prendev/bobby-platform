using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Dto;
using QvaDev.CTraderIntegration.Model;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public class ConnectorRetryDecorator : IConnector
    {
        public class CtPosition
        {
            public long PositionId { get; set; }
            public long Volume { get; set; }
            public string ClientMsgId { get; set; }
            public RetryOrder CloseOrder { get; set; }
            public string Symbol { get; set; }
        }

        private readonly ILog _log;
        private readonly ConcurrentDictionary<string, MarketOrder> _marketOrders = new ConcurrentDictionary<string, MarketOrder>();
        private readonly CTraderClientWrapper _cTraderClientWrapper;
        private readonly Connector _connector;
        private readonly AccountInfo _accountInfo;

        public string Description => _connector?.Description;
        public long AccountId => _accountInfo?.AccountId ?? 0;
        public bool IsConnected => _connector?.IsConnected == true;
        public readonly ConcurrentDictionary<long, CtPosition> Positions = new ConcurrentDictionary<long, CtPosition>();
        public event OrderEventHandler OnOrder;

        public double VolumeMultiplier => 100;

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

            //_cTraderClient.OnOrder += OnOrder;
            //_cTraderClient.OnLogin += OnLogin;
            //_cTraderClient.OnTick += OnTick;

            foreach (var p in GetPositions())
                Positions.GetOrAdd(p.positionId,
                    new CtPosition
                    {
                        PositionId = p.positionId,
                        Volume = p.volume,
                        ClientMsgId = p.comment,
                        Symbol = p.symbolName
                    });

            return true;
        }

        public void SendMarketOrderRequest(string symbol, ProtoTradeSide type, long volume, string clientOrderId, int maxRetryCount = 5, int retryPeriodInMilliseconds = 3000)
        {
            var clientMsgId = $"{AccountId}|{clientOrderId}";
            _marketOrders.GetOrAdd(clientMsgId, new MarketOrder
            {
                Symbol = symbol,
                Type = type,
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
                Type = type,
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
            foreach (var pos in Positions.Where(p => p.Value.ClientMsgId == clientMsgId))
                SendClosePositionRequest(pos.Key, Math.Abs(pos.Value.Volume), maxRetryCount, retryPeriodInMilliseconds);
        }

        private void SendClosePositionRequest(long positionId, long volume, int maxRetryCount = 5, int retryPeriodInMilliseconds = 3000)
        {
            var clientMsgId = $"{AccountId}|{positionId}";

            CtPosition position;
            if(Positions.TryGetValue(positionId, out position))
                position.CloseOrder = new RetryOrder { MaxRetryCount = maxRetryCount, RetryPeriodInMilliseconds = retryPeriodInMilliseconds };

            _connector.SendClosePositionRequest(positionId, volume, clientMsgId);
        }

        public List<PositionData> GetPositions()
        {
            return _connector.GetPositions();
        }

        private void OnPosition(ProtoOAPosition p)
        {
            if (p.AccountId != AccountId) return;
            if (p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN &&
                p.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_CLOSED) return;

            var pos = Positions.AddOrUpdate(p.PositionId,
                id => new CtPosition {PositionId = p.PositionId, Volume = p.Volume, ClientMsgId = p.Comment, Symbol = p.SymbolName},
                (id, old) => new CtPosition {PositionId = p.PositionId, Volume = p.Volume, ClientMsgId = p.Comment});

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

        private void CheckCloseOrder(ProtoOAPosition protoPos, CtPosition ctPos)
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
            CtPosition ctPos;
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
                _cTraderClientWrapper.CTraderClient.SendMarketRangeOrderRequest(_accountInfo.AccessToken, AccountId, order.Symbol, order.Type, order.Volume,
                    order.Price, order.SlippageInPips, clientMsgId);
            else _cTraderClientWrapper.CTraderClient.SendMarketOrderRequest(_accountInfo.AccessToken, AccountId,
                order.Symbol, order.Type, order.Volume, clientMsgId);
        }

        private void RetryClose(CtPosition ctPos)
        {
            ctPos.CloseOrder.RetryCount++;
            if (ctPos.CloseOrder.RetryCount > ctPos.CloseOrder.MaxRetryCount) return;
            if (DateTime.UtcNow - ctPos.CloseOrder.Time > new TimeSpan(0, 0, 0, 0, ctPos.CloseOrder.RetryPeriodInMilliseconds)) return;

            _cTraderClientWrapper.CTraderClient.SendClosePositionRequest(_accountInfo.AccessToken, AccountId,
                ctPos.PositionId, ctPos.Volume, $"{AccountId}|{ctPos.PositionId}");
        }
    }
}

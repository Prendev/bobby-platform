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
        private class CtPosition
        {
            public long PositionId { get; set; }
            public long Volume { get; set; }
            public string ClientMsgId { get; set; }
            public RetryOrder CloseOrder { get; set; }
        }

        private readonly ILog _log;
        private readonly ConnectorConfig _connectorConfig;
        private readonly ConcurrentDictionary<long, CtPosition> _positions = new ConcurrentDictionary<long, CtPosition>();
        private readonly ConcurrentDictionary<string, MarketOrder> _marketOrders = new ConcurrentDictionary<string, MarketOrder>();
        private readonly CTraderClientWrapper _wrapper;
        private readonly Connector _connector;

        public string Description => _connector?.Description;
        public long AccountId => _connector?.AccountId ?? 0;
        public bool IsConnected => _connector?.IsConnected == true;
        public event OrderEventHandler OnOrder;

        public double VolumeMultiplier => 100;

        public ConnectorRetryDecorator(
            CTraderClientWrapper cTraderClientWrapper,
            ITradingAccountsService tradingAccountsService,
            ILog log)
        {
            _wrapper = cTraderClientWrapper;
            _connectorConfig = new ConnectorConfig { MaxRetryCount = 5, RetryPeriodInMilliseconds = 3000};
            _log = log;

            _connector = new Connector(cTraderClientWrapper, tradingAccountsService, log);
        }

        public void Disconnect()
        {
            _connector.Disconnect();
        }

        public bool Connect(AccountInfo accountInfo)
        {
            if (!_connector.Connect(accountInfo)) return false;

            _wrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(AccountId);
            _wrapper.CTraderClient.SendSubscribeForTradingEventsRequest(_wrapper.Platform.AccessToken, AccountId);

            _wrapper.CTraderClient.OnPosition -= OnPosition;
            _wrapper.CTraderClient.OnError -= OnError;
            _wrapper.CTraderClient.OnPosition += OnPosition;
            _wrapper.CTraderClient.OnError += OnError;

            //_cTraderClient.OnOrder += OnOrder;
            //_cTraderClient.OnLogin += OnLogin;
            //_cTraderClient.OnTick += OnTick;

            foreach (var p in GetPositions())
                _positions.GetOrAdd(p.positionId, new CtPosition {PositionId = p.positionId, Volume = p.volume, ClientMsgId = p.comment});

            return true;
        }

        public void SendMarketOrderRequest(string symbol, ProtoTradeSide type, long volume, string clientOrderId)
        {
            var clientMsgId = $"{AccountId}|{clientOrderId}";
            _marketOrders.GetOrAdd(clientMsgId, new MarketOrder { Symbol = symbol, Type = type, Volume = volume });
            _connector.SendMarketOrderRequest(symbol, type, volume, clientMsgId);
        }


        public void SendMarketRangeOrderRequest(string symbol, ProtoTradeSide type, long volume, double price, int slippageInPips, string clientOrderId)
        {
            var clientMsgId = $"{AccountId}|{clientOrderId}";
            _marketOrders.GetOrAdd(clientMsgId,
                new MarketOrder { Symbol = symbol, Type = type, Volume = volume, Price = price, SlippageInPips = slippageInPips });
            _connector.SendMarketRangeOrderRequest(symbol, type, volume, price, slippageInPips, clientMsgId);
        }

        public void SendClosePositionRequests(string clientOrderId)
        {
            var clientMsgId = $"{AccountId}|{clientOrderId}";
            foreach (var pos in _positions.Where(p => p.Value.ClientMsgId == clientMsgId))
                SendClosePositionRequest(pos.Key, Math.Abs(pos.Value.Volume));
        }

        private void SendClosePositionRequest(long positionId, long volume)
        {
            var clientMsgId = $"{AccountId}|{positionId}";

            CtPosition position;
            if(_positions.TryGetValue(positionId, out position))
                position.CloseOrder = new RetryOrder();

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

            var pos = _positions.AddOrUpdate(p.PositionId,
                id => new CtPosition {PositionId = p.PositionId, Volume = p.Volume, ClientMsgId = p.Comment},
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
                _positions.TryRemove(protoPos.PositionId, out ctPos);
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
            if (long.TryParse(parts[1], out positionId) && _positions.TryGetValue(positionId, out ctPos))
                RetryClose(ctPos);
        }

        private void RetryMarketOrder(MarketOrder order, string clientMsgId)
        {
            order.RetryCount++;
            if (order.RetryCount > _connectorConfig.MaxRetryCount) return;
            if (DateTime.UtcNow - order.Time > new TimeSpan(0, 0, 0, 0, _connectorConfig.RetryPeriodInMilliseconds)) return;

            if (order.Price > 0)
                _wrapper.CTraderClient.SendMarketRangeOrderRequest(_wrapper.Platform.AccessToken, AccountId, order.Symbol, order.Type, order.Volume,
                    order.Price, order.SlippageInPips, clientMsgId);
            else _wrapper.CTraderClient.SendMarketOrderRequest(_wrapper.Platform.AccessToken, AccountId,
                order.Symbol, order.Type, order.Volume, clientMsgId);
        }

        private void RetryClose(CtPosition ctPos)
        {
            ctPos.CloseOrder.RetryCount++;
            if (ctPos.CloseOrder.RetryCount > _connectorConfig.MaxRetryCount) return;
            if (DateTime.UtcNow - ctPos.CloseOrder.Time > new TimeSpan(0, 0, 0, 0, _connectorConfig.RetryPeriodInMilliseconds)) return;

            _wrapper.CTraderClient.SendClosePositionRequest(_wrapper.Platform.AccessToken, AccountId,
                ctPos.PositionId, ctPos.Volume, $"{AccountId}|{ctPos.PositionId}");
        }
    }
}

using System;
using System.Collections.Concurrent;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Model;

namespace QvaDev.CTraderIntegration
{
    public class Connector : IConnector
    {
        private readonly ILog _log;
        private readonly ConnectorConfig _connectorConfig;
        private readonly ConcurrentDictionary<string, MarketOrder> _marketOrders = new ConcurrentDictionary<string, MarketOrder>();
        private readonly ConcurrentDictionary<long, RetryOrder> _closeOrders = new ConcurrentDictionary<long, RetryOrder>();
        private readonly CTraderClientWrapper _wrapper;
        private long _accountId;

        public string Description { get; private set; }
        public bool IsConnected => _wrapper?.IsConnected == true && _accountId > 0;

        public double LotsMultiplier => 10000000;

        public Connector(
            CTraderClientWrapper cTraderClientWrapper,
            ILog log)
        {
            _wrapper = cTraderClientWrapper;
            _connectorConfig = new ConnectorConfig() { MaxRetryCount = 5, RetryPeriodInMilliseconds = 3000};
            _log = log;
        }

        public void Disconnect()
        {
            _wrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(_accountId);
            _accountId = 0;
        }

        public bool Connect(AccountInfo accountInfo)
        {
            Description = accountInfo.Description;

            if (!_wrapper.IsConnected) return false;
            _accountId = _wrapper.Accounts.Find(a => a.accountNumber == accountInfo.AccountNumber)?.accountId ?? 0;
            if (_accountId == 0) return false;

            _wrapper.CTraderClient.SendSubscribeForTradingEventsRequest(_wrapper.Platform.AccessToken, _accountId);

            _wrapper.CTraderClient.OnPosition -= OnPosition;
            _wrapper.CTraderClient.OnError -= OnError;
            _wrapper.CTraderClient.OnPosition += OnPosition;
            _wrapper.CTraderClient.OnError += OnError;

            //_cTraderClient.OnOrder += OnOrder;
            //_cTraderClient.OnLogin += OnLogin;
            //_cTraderClient.OnTick += OnTick;

            return true;
        }

        public void SendMarketOrderRequest(string symbol, ProtoTradeSide type, long volume, string clientOrderId = null)
        {
            if (!string.IsNullOrWhiteSpace(clientOrderId))
                _marketOrders.GetOrAdd(clientOrderId, new MarketOrder { Symbol = symbol, Type = type, Volume = volume });

            _wrapper.CTraderClient.SendMarketOrderRequest(_wrapper.Platform.AccessToken, _accountId, symbol, type, volume, $"{_accountId}|{clientOrderId}");
        }


        public void SendMarketRangeOrderRequest(string symbol, ProtoTradeSide type, long volume, double price, int slippageInPips, string clientOrderId = null)
        {
            if (!string.IsNullOrWhiteSpace(clientOrderId))
                _marketOrders.GetOrAdd(clientOrderId, new MarketOrder { Symbol = symbol, Type = type, Volume = volume, Price = price, SlippageInPips = slippageInPips });

            _wrapper.CTraderClient.SendMarketRangeOrderRequest(_wrapper.Platform.AccessToken, _accountId, symbol, type, volume,
                price, slippageInPips, $"{_accountId}|{clientOrderId}");
        }

        public void SendClosePositionRequest(long positionId, long volume)
        {
            _closeOrders.GetOrAdd(positionId, new RetryOrder { Volume = volume });
            _wrapper.CTraderClient.SendClosePositionRequest(_wrapper.Platform.AccessToken, _accountId, positionId, volume, $"{_accountId}|{positionId}");
        }

        private void OnPosition(ProtoOAPosition position)
        {
            if (position.AccountId != _accountId) return;
            if (position.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_OPEN &&
                position.PositionStatus != ProtoOAPositionStatus.OA_POSITION_STATUS_CLOSED) return;

            string clientOrderId = null;
            if(position.Comment?.Split('|').Length == 2) clientOrderId = position.Comment.Split('|')[1];

            CheckMarketOrder(position, clientOrderId);
            CheckCloseOrder(position);
        }

        private void CheckMarketOrder(ProtoOAPosition position, string clientOrderId)
        {
            MarketOrder order;
            if (string.IsNullOrWhiteSpace(clientOrderId) || !_marketOrders.TryGetValue(clientOrderId, out order)) return;
            if (order.Volume == position.Volume)
            {
                _marketOrders.TryRemove(clientOrderId, out order);
                return;
            }
            order.Volume -= position.Volume;
            RetryMarketOrder(order, clientOrderId);
        }

        private void CheckCloseOrder(ProtoOAPosition position)
        {
            RetryOrder order;
            if (!_closeOrders.TryGetValue(position.PositionId, out order)) return;
            if (position.Volume == 0)
            {
                _closeOrders.TryRemove(position.PositionId, out order);
                return;
            }
            order.Volume = position.Volume;
            RetryClose(order, position.PositionId);
        }

        private void OnError(ProtoErrorRes error, string clientMsgId)
        {
            if (string.IsNullOrWhiteSpace(clientMsgId)) return;

            var parts = clientMsgId.Split('|');
            long accountId;
            if (parts.Length != 2 || !long.TryParse(parts[0], out accountId) || accountId != _accountId) return;

            _log.Info($"cTrader error: {error.Description}");

            MarketOrder marketOrder;
            if (_marketOrders.TryGetValue(parts[1], out marketOrder)) RetryMarketOrder(marketOrder, parts[1]);

            long positionId;
            RetryOrder closeOrder;
            if (long.TryParse(parts[1], out positionId) && _closeOrders.TryGetValue(positionId, out closeOrder)) RetryClose(closeOrder, positionId);
        }

        private void RetryMarketOrder(MarketOrder order, string clientOrderId)
        {
            order.RetryCount++;
            if (order.RetryCount > _connectorConfig.MaxRetryCount) return;
            if (DateTime.UtcNow - order.Time > new TimeSpan(0, 0, 0, 0, _connectorConfig.RetryPeriodInMilliseconds)) return;

            if (order.Price > 0)
                _wrapper.CTraderClient.SendMarketRangeOrderRequest(_wrapper.Platform.AccessToken, _accountId, order.Symbol, order.Type, order.Volume,
                    order.Price, order.SlippageInPips, $"{_accountId}|{clientOrderId}");
            else _wrapper.CTraderClient.SendMarketOrderRequest(_wrapper.Platform.AccessToken, _accountId,
                order.Symbol, order.Type, order.Volume, $"{_accountId}|{clientOrderId}");
        }

        private void RetryClose(RetryOrder order, long positionId)
        {
            order.RetryCount++;
            if (order.RetryCount > _connectorConfig.MaxRetryCount) return;
            if (DateTime.UtcNow - order.Time > new TimeSpan(0, 0, 0, 0, _connectorConfig.RetryPeriodInMilliseconds)) return;

            _wrapper.CTraderClient.SendClosePositionRequest(_wrapper.Platform.AccessToken, _accountId,
                positionId, order.Volume, $"{_accountId}|{positionId}");
        }
    }
}

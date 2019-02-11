using System;
using System.Collections.Generic;
using Google.ProtocolBuffers;

namespace TradeSystem.CTraderApi
{
    public class MessagesFactory
    {
        uint _lastMessagePayloadType;
        ByteString _lastMessagePayload;

        #region Building Proto messages from Byte array methods
        public ProtoMessage GetMessage(byte[] msg)
        {
            var _msg = ProtoMessage.CreateBuilder().MergeFrom(msg).Build();
            _lastMessagePayloadType = _msg.PayloadType;
            _lastMessagePayload = _msg.Payload;
            return _msg;
        }
        public uint GetPayloadType(byte[] msg = null)
        {
            if (msg != null)
                GetMessage(msg);
            return _lastMessagePayloadType;
        }
        public ByteString GetPayload(byte[] msg = null)
        {
            if (msg != null)
                GetMessage(msg);
            return _lastMessagePayload;
        }
        public ProtoPingReq GetPingRequest(byte[] msg = null)
        {
            return ProtoPingReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoPingRes GetPingResponse(byte[] msg = null)
        {
            return ProtoPingRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoHeartbeatEvent GetHeartbeatEvent(byte[] msg = null)
        {
            return ProtoHeartbeatEvent.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoErrorRes GetErrorResponse(byte[] msg = null)
        {
            return ProtoErrorRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAAuthReq GetAuthorizationRequest(byte[] msg = null)
        {
            return ProtoOAAuthReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAAuthRes GetAuthorizationResponse(byte[] msg = null)
        {
            return ProtoOAAuthRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOASubscribeForTradingEventsReq GetSubscribeForTradingEventsRequest(byte[] msg = null)
        {
            return ProtoOASubscribeForTradingEventsReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOASubscribeForTradingEventsRes GetSubscribeForTradingEventsResponse(byte[] msg = null)
        {
            return ProtoOASubscribeForTradingEventsRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAUnsubscribeFromTradingEventsReq GetUnsubscribeForTradingEventsRequest(byte[] msg = null)
        {
            return ProtoOAUnsubscribeFromTradingEventsReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAUnsubscribeFromTradingEventsRes GetUnsubscribeForTradingEventsResponse(byte[] msg = null)
        {
            return ProtoOAUnsubscribeFromTradingEventsRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAGetSubscribedAccountsReq GetAllSubscriptionsForTradingEventsRequest(byte[] msg = null)
        {
            return ProtoOAGetSubscribedAccountsReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAGetSubscribedAccountsRes GetAllSubscriptionsForTradingEventsResponse(byte[] msg = null)
        {
            return ProtoOAGetSubscribedAccountsRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAExecutionEvent GetExecutionEvent(byte[] msg = null)
        {
            return ProtoOAExecutionEvent.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOACreateOrderReq GetCreateOrderRequest(byte[] msg = null)
        {
            return ProtoOACreateOrderReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOACancelOrderReq GetCancelOrderRequest(byte[] msg = null)
        {
            return ProtoOACancelOrderReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAClosePositionReq GetClosePositionRequest(byte[] msg = null)
        {
            return ProtoOAClosePositionReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAAmendPositionStopLossTakeProfitReq GetAmendPositionStopLossTakeProfitRequest(byte[] msg = null)
        {
            return ProtoOAAmendPositionStopLossTakeProfitReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAAmendOrderReq GetAmendOrderRequest(byte[] msg = null)
        {
            return ProtoOAAmendOrderReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOASubscribeForSpotsReq GetSubscribeForSpotsRequest(byte[] msg = null)
        {
            return ProtoOASubscribeForSpotsReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOASubscribeForSpotsRes GetSubscribeForSpotsResponse(byte[] msg = null)
        {
            return ProtoOASubscribeForSpotsRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAUnsubscribeFromSpotsReq GetUnsubscribeFromSpotsRequest(byte[] msg = null)
        {
            return ProtoOAUnsubscribeFromSpotsReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAUnsubscribeFromSpotsRes GetUnsubscribeFromSpotsResponse(byte[] msg = null)
        {
            return ProtoOAUnsubscribeFromSpotsRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAGetSpotSubscriptionReq GetGetSpotSubscriptionRequest(byte[] msg = null)
        {
            return ProtoOAGetSpotSubscriptionReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAGetSpotSubscriptionRes GetGetSpotSubscriptionResponse(byte[] msg = null)
        {
            return ProtoOAGetSpotSubscriptionRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAGetAllSpotSubscriptionsReq GetGetAllSpotSubscriptionsRequest(byte[] msg = null)
        {
            return ProtoOAGetAllSpotSubscriptionsReq.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOAGetAllSpotSubscriptionsRes GetGetAllSpotSubscriptionsResponse(byte[] msg = null)
        {
            return ProtoOAGetAllSpotSubscriptionsRes.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        public ProtoOASpotEvent GetSpotEvent(byte[] msg = null)
        {
            return ProtoOASpotEvent.CreateBuilder().MergeFrom(GetPayload(msg)).Build();
        }
        #endregion

        #region Creating new Proto messages with parameters specified
        public ProtoMessage CreateMessage(uint payloadType, ByteString payload = null,
            string clientMsgId = null)
        {
            var protoMsg = ProtoMessage.CreateBuilder();
            protoMsg.PayloadType = payloadType;
            if (payload != null)
                protoMsg.SetPayload(payload);
            if (clientMsgId != null)
                protoMsg.SetClientMsgId(clientMsgId);

            return protoMsg.Build();
        }
        public ProtoMessage CreateMessage(ProtoMessage.Builder messageBuilder, string clientMsgId = null)
        {
            return CreateMessage(messageBuilder.PayloadType, messageBuilder.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreatePingRequest(ulong timestamp, string clientMsgId = null)
        {
            return CreateMessage((uint)ProtoPayloadType.PING_REQ, ProtoPingReq.CreateBuilder().SetTimestamp(timestamp).Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreatePingResponse(ulong timestamp, string clientMsgId = null)
        {
            return CreateMessage((uint)ProtoPayloadType.PING_RES, ProtoPingRes.CreateBuilder().SetTimestamp(timestamp).Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateHeartbeatEvent(string clientMsgId = null)
        {
            return CreateMessage((uint)ProtoPayloadType.HEARTBEAT_EVENT, ProtoHeartbeatEvent.CreateBuilder().Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateAuthorizationRequest(string clientId, string clientSecret, string clientMsgId = null)
        {
            var msg = ProtoOAAuthReq.CreateBuilder();
            msg.SetClientId(clientId);
            msg.SetClientSecret(clientSecret);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateAuthorizationResponse(string clientMsgId = null)
        {
            return CreateMessage((uint)ProtoOAPayloadType.OA_AUTH_RES, ProtoOAAuthRes.CreateBuilder().Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateSubscribeForTradingEventsRequest(long accountId, string accessToken, string clientMsgId = null)
        {
            var msg = ProtoOASubscribeForTradingEventsReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateSubscribeForTradingEventsResponse(string clientMsgId = null)
        {
            return CreateMessage((uint)ProtoOAPayloadType.OA_SUBSCRIBE_FOR_TRADING_EVENTS_RES, ProtoOASubscribeForTradingEventsRes.CreateBuilder().Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateUnsubscribeForTradingEventsRequest(long accountId, string clientMsgId = null)
        {
            var msg = ProtoOAUnsubscribeFromTradingEventsReq.CreateBuilder();
            msg.SetAccountId(accountId);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateUnsubscribeForTradingEventsResponse(string clientMsgId = null)
        {
            return CreateMessage((uint)ProtoOAPayloadType.OA_UNSUBSCRIBE_FROM_TRADING_EVENTS_RES, ProtoOAUnsubscribeFromTradingEventsRes.CreateBuilder().Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateAllSubscriptionsForTradingEventsRequest(string clientMsgId = null)
        {
            return CreateMessage((uint)ProtoOAPayloadType.OA_GET_SUBSCRIBED_ACCOUNTS_REQ, ProtoOAGetSubscribedAccountsReq.CreateBuilder().Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateAllSubscriptionsForTradingEventsResponse(List<long> accountIdsList, string clientMsgId = null)
        {
            var msg = ProtoOAGetSubscribedAccountsRes.CreateBuilder();
            foreach (var accountId in accountIdsList)
                msg.AddAccountId(accountId);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateExecutionEvent(ProtoOAExecutionType executionType, ProtoOAOrder order, ProtoOAPosition position = null, string reasonCode = null, string clientMsgId = null)
        {
            var msg = ProtoOAExecutionEvent.CreateBuilder();
            msg.SetExecutionType(executionType);
            msg.SetOrder(order);
            if (position != null)
                msg.SetPosition(position);
            if (reasonCode != null)
                msg.SetReasonCode(reasonCode);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateExecutionEvent(ProtoOAExecutionType executionType, ProtoOAOrder.Builder order, ProtoOAPosition.Builder position = null, string reasonCode = null, string clientMsgId = null)
        {
            return CreateExecutionEvent(executionType, order.Build(), position?.Build(), reasonCode, clientMsgId);
        }
        public ProtoMessage CreateMarketOrderRequest(long accountId, string accessToken, string symbolName, ProtoTradeSide tradeSide, long volume, string clientMsgId = null)
        {
            var msg = ProtoOACreateOrderReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetSymbolName(symbolName);
            msg.SetOrderType(ProtoOAOrderType.OA_MARKET);
            msg.SetTradeSide(tradeSide);
            msg.SetVolume(volume);
            msg.SetComment(clientMsgId ?? "TradingApiTest.CreateMarketOrderRequest");
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateMarketRangeOrderRequest(long accountId, string accessToken,
            string symbolName, ProtoTradeSide tradeSide, long volume, double baseSlippagePrice, long slippageInPips, string clientMsgId = null)
        {
            var msg = ProtoOACreateOrderReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetSymbolName(symbolName);
            msg.SetOrderType(ProtoOAOrderType.OA_MARKET_RANGE);
            msg.SetTradeSide(tradeSide);
            msg.SetVolume(volume);
            msg.SetBaseSlippagePrice(baseSlippagePrice);
            msg.SetSlippageInPips(slippageInPips);
            msg.SetComment(clientMsgId ?? "TradingApiTest.CreateMarketRangeOrderRequest");
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateLimitOrderRequest(long accountId, string accessToken, string symbolName, ProtoTradeSide tradeSide, long volume, double limitPrice, string clientMsgId = null)
        {
            var msg = ProtoOACreateOrderReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetSymbolName(symbolName);
            msg.SetOrderType(ProtoOAOrderType.OA_LIMIT);
            msg.SetTradeSide(tradeSide);
            msg.SetVolume(volume);
            msg.SetLimitPrice(limitPrice);
            msg.SetComment("TradingApiTest.CreateLimitOrderRequest");
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            msg.SetExpirationTimestamp((long)(HiResDatetime.UtcNow.AddHours(1) - epoch).TotalMilliseconds);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateStopOrderRequest(long accountId, string accessToken, string symbolName, ProtoTradeSide tradeSide, long volume, double stopPrice, string clientMsgId = null)
        {
            var msg = ProtoOACreateOrderReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetSymbolName(symbolName);
            msg.SetOrderType(ProtoOAOrderType.OA_STOP);
            msg.SetTradeSide(tradeSide);
            msg.SetVolume(volume);
            msg.SetStopPrice(stopPrice);
            msg.SetComment("TradingApiTest.CreateStopOrderRequest");
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateCancelOrderRequest(long accountId, string accessToken, long orderId, string clientMsgId = null)
        {
            var msg = ProtoOACancelOrderReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetOrderId(orderId);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateClosePositionRequest(long accountId, string accessToken, long positionId, long volume, string clientMsgId = null)
        {
            var msg = ProtoOAClosePositionReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetPositionId(positionId);
            msg.SetVolume(volume);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateAmendPositionStopLossRequest(long accountId, string accessToken, long positionId, double stopLossPrice, string clientMsgId = null)
        {
            var msg = ProtoOAAmendPositionStopLossTakeProfitReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetPositionId(positionId);
            msg.SetStopLossPrice(stopLossPrice);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateAmendPositionTakeProfitRequest(long accountId, string accessToken, long positionId, double takeProfitPrice, string clientMsgId = null)
        {
            var msg = ProtoOAAmendPositionStopLossTakeProfitReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetPositionId(positionId);
            msg.SetTakeProfitPrice(takeProfitPrice);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateAmendPositionProtectionRequest(long accountId, string accessToken, long positionId, double stopLossPrice, double takeProfitPrice, string clientMsgId = null)
        {
            var msg = ProtoOAAmendPositionStopLossTakeProfitReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetPositionId(positionId);
            msg.SetStopLossPrice(stopLossPrice);
            msg.SetTakeProfitPrice(takeProfitPrice);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateAmendLimitOrderRequest(long accountId, string accessToken, long orderId, double limitPrice, string clientMsgId = null)
        {
            var msg = ProtoOAAmendOrderReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetOrderId(orderId);
            msg.SetLimitPrice(limitPrice);
            msg.SetTakeProfitPrice(limitPrice + 0.02);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateAmendStopOrderRequest(long accountId, string accessToken, long orderId, double stopPrice, string clientMsgId = null)
        {
            var msg = ProtoOAAmendOrderReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetOrderId(orderId);
            msg.SetStopPrice(stopPrice);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateSubscribeForSpotsRequest(long accountId, string accessToken, string symbolName, string clientMsgId = null)
        {
            var msg = ProtoOASubscribeForSpotsReq.CreateBuilder();
            msg.SetAccountId(accountId);
            msg.SetAccessToken(accessToken);
            msg.SetSymblolName(symbolName);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateSubscribeForSpotsResponse(uint subscriptionId, string clientMsgId = null)
        {
            var msg = ProtoOASubscribeForSpotsRes.CreateBuilder();
            msg.SetSubscriptionId(subscriptionId);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateUnsubscribeFromAllSpotsRequest(string clientMsgId = null)
        {
            var msg = ProtoOAUnsubscribeFromSpotsReq.CreateBuilder();
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateUnsubscribeAccountFromSpotsRequest(uint subscriptionId, string clientMsgId = null)
        {
            var msg = ProtoOAUnsubscribeFromSpotsReq.CreateBuilder();
            msg.SetSubscriptionId(subscriptionId);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateUnsubscribeFromSymbolSpotsRequest(string symbolName, string clientMsgId = null)
        {
            var msg = ProtoOAUnsubscribeFromSpotsReq.CreateBuilder();
            msg.SetSymblolName(symbolName);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateUnsubscribeAccountFromSymbolSpotsRequest(uint subscriptionId, string symbolName, string clientMsgId = null)
        {
            var msg = ProtoOAUnsubscribeFromSpotsReq.CreateBuilder();
            msg.SetSubscriptionId(subscriptionId);
            msg.SetSymblolName(symbolName);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateUnsubscribeFromSpotsResponse(string clientMsgId = null)
        {
            var msg = ProtoOAUnsubscribeFromSpotsRes.CreateBuilder();
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateGetSpotSubscriptionRequest(uint subscriptionId, string clientMsgId = null)
        {
            var msg = ProtoOAGetSpotSubscriptionReq.CreateBuilder();
            msg.SetSubscriptionId(subscriptionId);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateGetSpotSubscriptionResponse(ProtoOASpotSubscription spotSubscription, string clientMsgId = null)
        {
            var msg = ProtoOAGetSpotSubscriptionRes.CreateBuilder();
            msg.SetSpotSubscription(spotSubscription);
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        public ProtoMessage CreateGetAllSpotSubscriptionsRequest(string clientMsgId = null)
        {
            var msg = ProtoOAGetAllSpotSubscriptionsReq.CreateBuilder();
            return CreateMessage((uint)msg.PayloadType, msg.Build().ToByteString(), clientMsgId);
        }
        #endregion

        #region Creating new Proto messages Builders
        public ProtoOAGetAllSpotSubscriptionsRes.Builder CreateGetAllSpotSubscriptionsResponseBuilder(string clientMsgId = null)
        {
            return ProtoOAGetAllSpotSubscriptionsRes.CreateBuilder();
        }
        public ProtoOASpotEvent.Builder CreateSpotEventBuilder(uint subscriptionId, string symbolName, string clientMsgId = null)
        {
            return ProtoOASpotEvent.CreateBuilder();
        }
        #endregion
    }
}

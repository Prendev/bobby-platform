namespace QvaDev.CTraderApi
{
    public class ModelObjectsFactory
    {
        #region Building Proto Model objects from Byte array methods
        public ProtoOAOrder GetOrder(byte[] obj = null)
        {
            return ProtoOAOrder.CreateBuilder().MergeFrom(obj).Build();
        }
        public ProtoOAPosition GetPosition(byte[] obj = null)
        {
            return ProtoOAPosition.CreateBuilder().MergeFrom(obj).Build();
        }
        public ProtoOAClosePositionDetails GetClosePositionDetails(byte[] obj = null)
        {
            return ProtoOAClosePositionDetails.CreateBuilder().MergeFrom(obj).Build();
        }
        public ProtoOASpotSubscription GetSpotSubscription(byte[] obj = null)
        {
            return ProtoOASpotSubscription.CreateBuilder().MergeFrom(obj).Build();
        }
        #endregion

        #region Creating new Proto Model objects with parameters specified
        public ProtoOAOrder.Builder CreateOrderBuilder(long orderId, long accountId, ProtoOAOrderType orderType, ProtoTradeSide tradeSide, string symbolName, long requestedVolume, long executedVolume, bool closingOrder,
            string channel = null, string comment=null)
        {
            var obj = ProtoOAOrder.CreateBuilder();
            obj.SetOrderId(orderId);
            obj.SetAccountId(accountId);
            obj.SetOrderType(orderType);
            obj.SetTradeSide(tradeSide);
            obj.SetSymbolName(symbolName);
            obj.SetRequestedVolume(requestedVolume);
            obj.SetExecutedVolume(executedVolume);
            obj.SetClosingOrder(closingOrder);
            if (channel != null)
                obj.SetChannel(channel);
            if (comment != null)
                obj.SetComment(comment);
            return obj;
        }
        public ProtoOAPosition.Builder CreatePositionBuilder(long positionId, ProtoOAPositionStatus positionStatus, long accountId, ProtoTradeSide tradeSide, string symbolName, long volume, double entryPrice, long swap,
            long commission, long openTimestamp, string channel = null, string comment = null)
        {
            var obj = ProtoOAPosition.CreateBuilder();
            obj.SetPositionId(positionId);
            obj.SetPositionStatus(positionStatus);
            obj.SetAccountId(accountId);
            obj.SetTradeSide(tradeSide);
            obj.SetSymbolName(symbolName);
            obj.SetVolume(volume);
            obj.SetEntryPrice(entryPrice);
            obj.SetSwap(swap);
            obj.SetCommission(commission);
            obj.SetOpenTimestamp(openTimestamp);
            if (channel != null)
                obj.SetChannel(channel);
            if (comment != null)
                obj.SetComment(comment);
            return obj;
        }
        public ProtoOAClosePositionDetails.Builder CreateClosePositionDetailsBuilder(double entryPrice, long profit, long swap, long commission, long balance, long closedVolume, bool closedByStopOut, string comment = null)
        {
            var obj = ProtoOAClosePositionDetails.CreateBuilder();
            obj.SetEntryPrice(entryPrice);
            obj.SetProfit(profit);
            obj.SetSwap(swap);
            obj.SetCommission(commission);
            obj.SetBalance(balance);
            obj.SetClosedVolume(closedVolume);
            obj.SetClosedByStopOut(closedByStopOut);
            if (comment != null)
                obj.SetComment(comment);
            return obj;
        }
        public ProtoOASpotSubscription.Builder CreateSpotSubscriptionBuilder(long accountId, uint subscriptionId)
        {
            var obj = ProtoOASpotSubscription.CreateBuilder();
            obj.SetAccountId(accountId);
            obj.SetSubscriptionId(subscriptionId);
            return obj;
        }
        #endregion
    }
}

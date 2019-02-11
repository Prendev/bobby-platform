using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBApi;
using TradeSystem.Common.Integration;
using Bar = IBApi.Bar;

namespace TradeSystem.IbIntegration
{
	public partial class Connector : EWrapper
	{
		public void error(Exception e)
		{
			Logger.Error($"{_accountInfo.Description} account exeption", e);
		}

		public void error(string str)
		{
			Logger.Error($"{_accountInfo.Description} account exeption", new Exception(str));
		}

		public void error(int id, int errorCode, string errorMsg)
		{
			// A notification that connection to the market data server is ok. This is a notification and not a true error condition, and is expected on first establishing connection.
			if (errorCode == 2104) return;
			// A notification that connection to the market data server is ok. This is a notification and not a true error condition, and is expected on first establishing connection. 
			if (errorCode == 2106) return;
			// Note: -1 will indicate a notification and not true error condition.
			if (id == -1) return;

			if (errorCode < 0)
			{
				Logger.Error($"{_accountInfo.Description} account exeption with id {id} and errorCode {errorCode}", new Exception(errorMsg));
				OnConnectionChanged(ConnectionStates.Error);
			}
			else if (errorCode == 399)
				Logger.Warn($"{_accountInfo.Description} account exeption with id {id} and errorCode {errorCode}\n{errorMsg}");
			else
				Task.Run(() => _taskCompletionManager.SetError(id.ToString(), new Exception($"{id} | {errorCode} | {errorMsg}")));
		}

		public void currentTime(long time)
		{
			
		}

		public void tickPrice(int tickerId, int field, double price, TickAttrib attribs)
		{
			
		}

		public void tickSize(int tickerId, int field, int size)
		{
			
		}

		public void tickString(int tickerId, int field, string value)
		{
			
		}

		public void tickGeneric(int tickerId, int field, double value)
		{
			
		}

		public void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture,
			int holdDays, string futureLastTradeDate, double dividendImpact, double dividendsToLastTradeDate)
		{
			
		}

		public void deltaNeutralValidation(int reqId, DeltaNeutralContract deltaNeutralContract)
		{
			
		}

		public void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice,
			double pvDividend, double gamma, double vega, double theta, double undPrice)
		{
			
		}

		public void tickSnapshotEnd(int tickerId)
		{
			
		}

		public void nextValidId(int orderId)
		{
			_lastOrderId = orderId - 1;
			_taskCompletionManager.SetCompleted(_accountInfo.Description, true);
		}

		public void managedAccounts(string accountsList)
		{
			
		}

		public void connectionClosed()
		{
			OnConnectionChanged(GetStatus());
		}

		public void accountSummary(int reqId, string account, string tag, string value, string currency)
		{
			
		}

		public void accountSummaryEnd(int reqId)
		{
			
		}

		public void bondContractDetails(int reqId, ContractDetails contract)
		{
			
		}

		public void updateAccountValue(string key, string value, string currency, string accountName)
		{
			
		}

		public void updatePortfolio(Contract contract, double position, double marketPrice, double marketValue, double averageCost,
			double unrealizedPNL, double realizedPNL, string accountName)
		{
			
		}

		public void updateAccountTime(string timestamp)
		{
			
		}

		public void accountDownloadEnd(string account)
		{
			
		}

		public void orderStatus(int orderId, string status, double filled, double remaining, double avgFillPrice, int permId,
			int parentId, double lastFillPrice, int clientId, string whyHeld, double mktCapPrice)
		{
			
		}

		public void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
		{
			
		}

		public void openOrderEnd()
		{
			
		}

		public void contractDetails(int reqId, ContractDetails contractDetails)
		{
			
		}

		public void contractDetailsEnd(int reqId)
		{
			
		}

		public void execDetails(int reqId, Contract contract, Execution execution)
		{
			if (execution.ClientId != _accountInfo.ClientId) return;

			var response = new OrderResponse()
			{
				AveragePrice = (decimal)execution.AvgPrice,
				FilledQuantity = (decimal)Math.Abs(execution.CumQty)
			};

			Task.Run(() => _taskCompletionManager.SetResult(execution.OrderId.ToString(), response, true));
		}

		public void execDetailsEnd(int reqId)
		{
			
		}

		public void commissionReport(CommissionReport commissionReport)
		{
			
		}

		public void fundamentalData(int reqId, string data)
		{
			
		}

		public void historicalData(int reqId, Bar bar)
		{
			
		}

		public void historicalDataUpdate(int reqId, Bar bar)
		{
			
		}

		public void historicalDataEnd(int reqId, string start, string end)
		{
			
		}

		public void marketDataType(int reqId, int marketDataType)
		{
			
		}

		public void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
		{
			
		}

		public void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size,
			bool isSmartDepth)
		{
			
		}

		public void updateNewsBulletin(int msgId, int msgType, string message, string origExchange)
		{
			
		}

		public void position(string account, Contract contract, double pos, double avgCost)
		{
			
		}

		public void positionEnd()
		{
			
		}

		public void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP,
			int count)
		{
			
		}

		public void scannerParameters(string xml)
		{
			
		}

		public void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark,
			string projection, string legsStr)
		{
			
		}

		public void scannerDataEnd(int reqId)
		{
			
		}

		public void receiveFA(int faDataType, string faXmlData)
		{
			
		}

		public void verifyMessageAPI(string apiData)
		{
			
		}

		public void verifyCompleted(bool isSuccessful, string errorText)
		{
			
		}

		public void verifyAndAuthMessageAPI(string apiData, string xyzChallenge)
		{
			
		}

		public void verifyAndAuthCompleted(bool isSuccessful, string errorText)
		{
			
		}

		public void displayGroupList(int reqId, string groups)
		{
			
		}

		public void displayGroupUpdated(int reqId, string contractInfo)
		{
			
		}

		public void connectAck()
		{
			
		}

		public void positionMulti(int requestId, string account, string modelCode, Contract contract, double pos, double avgCost)
		{
			
		}

		public void positionMultiEnd(int requestId)
		{
			
		}

		public void accountUpdateMulti(int requestId, string account, string modelCode, string key, string value, string currency)
		{
			
		}

		public void accountUpdateMultiEnd(int requestId)
		{
			
		}

		public void securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass,
			string multiplier, HashSet<string> expirations, HashSet<double> strikes)
		{
			
		}

		public void securityDefinitionOptionParameterEnd(int reqId)
		{
			
		}

		public void softDollarTiers(int reqId, SoftDollarTier[] tiers)
		{
			
		}

		public void familyCodes(FamilyCode[] familyCodes)
		{
			
		}

		public void symbolSamples(int reqId, ContractDescription[] contractDescriptions)
		{
			
		}

		public void mktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
		{
			
		}

		public void tickNews(int tickerId, long timeStamp, string providerCode, string articleId, string headline, string extraData)
		{
			
		}

		public void smartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
		{
			
		}

		public void tickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
		{
			
		}

		public void newsProviders(NewsProvider[] newsProviders)
		{
			
		}

		public void newsArticle(int requestId, int articleType, string articleText)
		{
			
		}

		public void historicalNews(int requestId, string time, string providerCode, string articleId, string headline)
		{
			
		}

		public void historicalNewsEnd(int requestId, bool hasMore)
		{
			
		}

		public void headTimestamp(int reqId, string headTimestamp)
		{
			
		}

		public void histogramData(int reqId, HistogramEntry[] data)
		{
			
		}

		public void rerouteMktDataReq(int reqId, int conId, string exchange)
		{
			
		}

		public void rerouteMktDepthReq(int reqId, int conId, string exchange)
		{
			
		}

		public void marketRule(int marketRuleId, PriceIncrement[] priceIncrements)
		{
			
		}

		public void pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
		{
			
		}

		public void pnlSingle(int reqId, int pos, double dailyPnL, double unrealizedPnL, double realizedPnL, double value)
		{
			
		}

		public void historicalTicks(int reqId, HistoricalTick[] ticks, bool done)
		{
			
		}

		public void historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
		{
			
		}

		public void historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
		{
			
		}

		public void tickByTickAllLast(int reqId, int tickType, long time, double price, int size, TickAttribLast tickAttriblast,
			string exchange, string specialConditions)
		{
			
		}

		public void tickByTickBidAsk(int reqId, long time, double bidPrice, double askPrice, int bidSize, int askSize,
			TickAttribBidAsk tickAttribBidAsk)
		{
			var symbol = _subscriptions[reqId];
			var tick = new Tick
			{
				Symbol = symbol,
				Ask = (decimal)askPrice,
				Bid = (decimal)bidPrice,
				Time = HiResDatetime.UtcNow
			};
			LastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);
			OnNewTick(new NewTick { Tick = tick });
		}

		public void tickByTickMidPoint(int reqId, long time, double midPoint)
		{
			
		}

		public void orderBound(long orderId, int apiClientId, int apiOrderId)
		{
			
		}
	}
}

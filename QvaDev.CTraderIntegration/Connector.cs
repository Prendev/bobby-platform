using System.Collections.Generic;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.CTraderIntegration.Dto;
using QvaDev.CTraderIntegration.Services;

namespace QvaDev.CTraderIntegration
{
    public class Connector : IConnector
    {
        private readonly ILog _log;
        private readonly CTraderClientWrapper _wrapper;
        private readonly ITradingAccountsService _tradingAccountsService;
        private readonly AccountInfo _accountInfo;

        public string Description => _accountInfo?.Description;
        public long AccountId => _accountInfo?.AccountId ?? 0;
        public bool IsConnected => _wrapper?.IsConnected == true && AccountId > 0;
        public event OrderEventHandler OnOrder;

        public double VolumeMultiplier => 100;

        public Connector(
            AccountInfo accountInfo,
            CTraderClientWrapper cTraderClientWrapper,
            ITradingAccountsService tradingAccountsService,
            ILog log)
        {
            _accountInfo = accountInfo;
            _tradingAccountsService = tradingAccountsService;
            _wrapper = cTraderClientWrapper;
            _log = log;
        }

        public void Disconnect()
        {
            lock (_wrapper.CTraderClient)
                _wrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(AccountId);
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) disconnected");
        }

        public bool Connect()
        {
            if (!IsConnected)
            {
                _log.Error($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) FAILED to connect");
                return false;
            }
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) connected");

            return true;
        }

        public void SendMarketOrderRequest(string symbol, ProtoTradeSide type, long volume, string clientMsgId = null)
        {
            lock (_wrapper.CTraderClient)
                _wrapper.CTraderClient.SendMarketOrderRequest(_accountInfo.AccessToken, AccountId, symbol, type, volume, clientMsgId);
        }


        public void SendMarketRangeOrderRequest(string symbol, ProtoTradeSide type, long volume, double price, int slippageInPips, string clientMsgId = null)
        {
            lock(_wrapper.CTraderClient)
                _wrapper.CTraderClient.SendMarketRangeOrderRequest(_accountInfo.AccessToken, AccountId, symbol, type, volume,
                    price, slippageInPips, clientMsgId);
        }

        public void SendClosePositionRequest(long positionId, long volume, string clientMsgId = null)
        {
            lock (_wrapper.CTraderClient)
                _wrapper.CTraderClient.SendClosePositionRequest(_accountInfo.AccessToken, AccountId, positionId, volume, clientMsgId);
        }

        public List<PositionData> GetPositions()
        {
            var positions = _tradingAccountsService.GetPositions(new AccountRequest
            {
                AccessToken = _accountInfo.AccessToken,
                BaseUrl = _wrapper.Platform.AccountsApi,
                AccountId = AccountId
            });

            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) positions acquired");
            return positions;
        }
    }
}

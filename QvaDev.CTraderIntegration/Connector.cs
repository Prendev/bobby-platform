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
        private AccountInfo _accountInfo;

        public string Description => _accountInfo?.Description;
        public long AccountId { get; private set; }
        public bool IsConnected => _wrapper?.IsConnected == true && AccountId > 0;
        public event OrderEventHandler OnOrder;

        public double VolumeMultiplier => 100;

        public Connector(
            CTraderClientWrapper cTraderClientWrapper,
            ITradingAccountsService tradingAccountsService,
            ILog log)
        {
            _tradingAccountsService = tradingAccountsService;
            _wrapper = cTraderClientWrapper;
            _log = log;
        }

        public void Disconnect()
        {
            _wrapper.CTraderClient.SendUnsubscribeForTradingEventsRequest(AccountId);
            AccountId = 0;
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) disconnected");
        }

        public bool Connect(AccountInfo accountInfo)
        {
            _accountInfo = accountInfo;
            AccountId = _wrapper.Accounts.Find(a => a.accountNumber == _accountInfo.AccountNumber)?.accountId ?? 0;

            if (!_wrapper.IsConnected || AccountId == 0)
            {
                _log.Error($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) FAILED to connect");
                return false;
            }
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) connected");

            return true;
        }

        public void SendMarketOrderRequest(string symbol, ProtoTradeSide type, long volume, string clientMsgId = null)
        {
            _wrapper.CTraderClient.SendMarketOrderRequest(_wrapper.Platform.AccessToken, AccountId, symbol, type, volume, clientMsgId);
        }


        public void SendMarketRangeOrderRequest(string symbol, ProtoTradeSide type, long volume, double price, int slippageInPips, string clientMsgId = null)
        {
            _wrapper.CTraderClient.SendMarketRangeOrderRequest(_wrapper.Platform.AccessToken, AccountId, symbol, type, volume,
                price, slippageInPips, clientMsgId);
        }

        public void SendClosePositionRequest(long positionId, long volume, string clientMsgId = null)
        {
            _wrapper.CTraderClient.SendClosePositionRequest(_wrapper.Platform.AccessToken, AccountId, positionId, volume, clientMsgId);
        }

        public List<PositionData> GetPositions()
        {
            var positions = _tradingAccountsService.GetPositions(new AccountRequest
            {
                AccessToken = _wrapper.Platform.AccessToken,
                BaseUrl = _wrapper.Platform.AccountsApi,
                AccountId = AccountId
            });

            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.AccountNumber}) positions acquired");
            return positions;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QvaDev.CTraderIntegration.Dto;

namespace QvaDev.CTraderIntegration.Services
{
    public interface ITradingAccountsService
    {
        List<AccountData> GetAccounts(BaseRequest request);

        List<PositionData> GetPositions(AccountRequest request);
        Task<List<PositionData>> GetPositionsAsync(AccountRequest request);

        List<DealData> GetDeals(DealsRequest request);
    }

    public class TradingAccountsService : ITradingAccountsService
    {
        private readonly IRestService _restService;

        public TradingAccountsService(IRestService restService)
        {
            _restService = restService;
        }

        public List<AccountData> GetAccounts(BaseRequest request)
        {
            return _restService.Get<ListResponse<AccountData>>("connect/tradingaccounts", request.AccessToken, request.BaseUrl).data;
        }

        public List<PositionData> GetPositions(AccountRequest request)
        {
            return _restService.Get<ListResponse<PositionData>>(
                $"connect/tradingaccounts/{request.AccountId}/positions", request.AccessToken, request.BaseUrl).data;
        }

        public async Task<List<PositionData>> GetPositionsAsync(AccountRequest request)
        {
            var response = await _restService.GetAsync<ListResponse<PositionData>>(
                $"connect/tradingaccounts/{request.AccountId}/positions", request.AccessToken, request.BaseUrl);
            return response.data;
        }

        public List<DealData> GetDeals(DealsRequest request)
        {
            return _restService.Get<ListResponse<DealData>>(
                $"connect/tradingaccounts/{request.AccountId}/deals",
                request.AccessToken, request.BaseUrl, ConvertToUnixTimestamp(request.From)).data;
        }

        private long ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = date.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalMilliseconds);
        }
    }
}

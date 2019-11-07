using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Collections;
using TradeSystem.CTraderIntegration.Dto;

namespace TradeSystem.CTraderIntegration.Services
{
    public interface ITradingAccountsService
    {
        Task<List<AccountData>> GetAccountsAsync(BaseRequest request);
        Task<List<PositionData>> GetPositionsAsync(AccountRequest request);
    }

    public class TradingAccountsService : ITradingAccountsService
	{
		private class Entry
		{
			public Guid Key { get; set; }
			public BaseRequest Request { get; set; }
			public IRestService RestService { get; set; }
		}

		private static readonly FastBlockingCollection<Entry> Queue =
			new FastBlockingCollection<Entry>();
		private static readonly TaskCompletionManager<Guid> TaskCompletionManager =
			new TaskCompletionManager<Guid>(100, 60000);

		private readonly IRestService _restService;

		static TradingAccountsService()
		{
			var thread = new Thread(Loop) { Name = "CTrader", IsBackground = true, Priority = ThreadPriority.BelowNormal };
			thread.Start();
		}

		private static void Loop()
		{
			while (true)
			{
				var entry = Queue.Take();
				try
				{
					if (entry.Request is AccountRequest request)
					{
						var response = entry.RestService.GetAsync<ListResponse<PositionData>>(
							$"connect/tradingaccounts/{request.AccountId}/positions", request.AccessToken, request.BaseUrl).Result;
						TaskCompletionManager.SetResult(entry.Key, response, true);
					}
					else
					{
						var response = entry.RestService.GetAsync<ListResponse<AccountData>>(
							"connect/tradingaccounts", entry.Request.AccessToken, entry.Request.BaseUrl).Result;
						TaskCompletionManager.SetResult(entry.Key, response, true);
					}
				}
				catch (Exception e)
				{
					TaskCompletionManager.SetError(entry.Key, e, true);
				}
				Thread.Sleep(TimeSpan.FromSeconds(3));
			}
		}

		public TradingAccountsService(IRestService restService)
        {
            _restService = restService;
        }

        public async Task<List<AccountData>> GetAccountsAsync(BaseRequest request)
        {
	        var key = Guid.NewGuid();
	        var task = TaskCompletionManager.CreateCompletableTask<ListResponse<AccountData>>(key);
	        Queue.Add(new Entry {Key = key, Request = request, RestService = _restService });
	        var response = await task;
	        return response.data;
		}

		public async Task<List<PositionData>> GetPositionsAsync(AccountRequest request)
		{
			var key = Guid.NewGuid();
			var task = TaskCompletionManager.CreateCompletableTask<ListResponse<PositionData>>(key);
			Queue.Add(new Entry { Key = key, Request = request, RestService = _restService});
			var response = await task;
			return response.data;
		}

        private long ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = date.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalMilliseconds);
        }
    }
}

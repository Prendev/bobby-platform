using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TradeSystem.Common;
using RestSharp;

namespace TradeSystem.CTraderIntegration
{
    public interface IRestService
    {
        Task<T> GetAsync<T>(string resource, string accessToken, string baseUrl) where T : new();
    }

    public class RestService : IRestService
    {
        private static readonly ConcurrentDictionary<string, RestClient> RestClients =
            new ConcurrentDictionary<string, RestClient>();

        public Task<T> GetAsync<T>(string resource, string accessToken, string baseUrl) where T : new()
        {
            var request = new RestRequest
            {
                Resource = resource
            };
            request.AddQueryParameter("oauth_token", accessToken);
            return ExecuteAsync<T>(request, baseUrl);
        }

        private async Task<T> ExecuteAsync<T>(RestRequest request, string baseUrl) where T : new()
        {
            var client = RestClients.GetOrAdd(baseUrl, CreateRestClient);
            var response = await client.ExecuteGetTaskAsync<T>(request);
			CtLogger.Log(request, response);

			if (response.ErrorException != null)
                throw new ApplicationException($"{response.StatusDescription} ({response.StatusCode}) {baseUrl}/{request?.Resource}.",
                    response.ErrorException);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new UnexpectedStatusCodeException(response.StatusCode, response.ResponseUri);

            return response.Data;
        }

        private RestClient CreateRestClient(string baseUrl)
        {
            return new RestClient(baseUrl);
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using QvaDev.Common;
using QvaDev.Common.Logging;
using RestSharp;

namespace QvaDev.CTraderIntegration
{
    public interface IRestService
    {
        T Get<T>(string resource, string accessToken, string baseUrl, long fromTimestamp = 0, long toTimestamp = 0) where T : new();
        Task<T> GetAsync<T>(string resource, string accessToken, string baseUrl) where T : new();
    }

    public class RestService : IRestService
    {
        private readonly ConcurrentDictionary<string, RestClient> _restClients =
            new ConcurrentDictionary<string, RestClient>();
        private readonly ILog _log;

        public RestService(ILog log)
        {
            _log = log;
        }

        public T Get<T>(string resource, string accessToken, string baseUrl,
            long fromTimestamp = 0, long toTimestamp = 0) where T : new()
        {
            var request = new RestRequest
            {
                Resource = resource
            };
            request.AddQueryParameter("oauth_token", accessToken);
            if (fromTimestamp > 0)
            {
                request.AddQueryParameter("fromTimestamp", fromTimestamp.ToString());
                request.AddQueryParameter("toTimestamp", toTimestamp.ToString());
                request.AddQueryParameter("limit ", 750.ToString());
            }
            return Execute<T>(request, baseUrl);
        }

        public Task<T> GetAsync<T>(string resource, string accessToken, string baseUrl) where T : new()
        {
            var request = new RestRequest
            {
                Resource = resource
            };
            request.AddQueryParameter("oauth_token", accessToken);
            return ExecuteAsync<T>(request, baseUrl);
        }

        private T Execute<T>(RestRequest request, string baseUrl) where T : new()
        {
            var client = new RestClient(baseUrl);

            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
                throw new ApplicationException("Error retrieving response.  Check inner details for more info.",
                    response.ErrorException);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new UnexpectedStatusCodeException(response.StatusCode, response.ResponseUri);

            return response.Data;
        }

        private async Task<T> ExecuteAsync<T>(RestRequest request, string baseUrl) where T : new()
        {
            var client = _restClients.GetOrAdd(baseUrl, CreateRestClient);
            var response = await client.ExecuteGetTaskAsync<T>(request);

            if (response.ErrorException != null)
                throw new ApplicationException("Error retrieving response.  Check inner details for more info.",
                    response.ErrorException);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new UnexpectedStatusCodeException(response.StatusCode, response.ResponseUri);

            return response.Data;
        }

        private RestClient CreateRestClient(string baseUrl)
        {
            return new RestClient(baseUrl);;
        }
    }
}

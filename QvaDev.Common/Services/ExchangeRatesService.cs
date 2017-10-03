using System;
using System.Collections.Generic;
using RestSharp;

namespace QvaDev.Common.Services
{
    public interface IExchangeRatesService
    {
        Dictionary<string, decimal> GetRates(string appId);
    }

    public class ExchangeRatesService : IExchangeRatesService
    {
        private class ExchangeRatesData
        {
            /// <summary>
            /// Exchange rates relative to USD
            /// </summary>
            public Dictionary<string, decimal> rates { get; set; }
        }

        public Dictionary<string, decimal> GetRates(string appId)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri("https://openexchangerates.org/api")
            };
            var request = new RestRequest
            {
                Resource = "latest.json"
            };
            request.AddQueryParameter("app_id", appId);

            var response = client.Execute<ExchangeRatesData>(request);

            if (response.ErrorException != null)
                throw new ApplicationException("Error retrieving response.  Check inner details for more info.",
                    response.ErrorException);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new UnexpectedStatusCodeException(response.StatusCode, response.ResponseUri);

            return response.Data.rates;
        }
    }
}

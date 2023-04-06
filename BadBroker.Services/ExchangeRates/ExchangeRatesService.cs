using BadBroker.Shared.Constants;
using BadBroker.Shared.ResponseModels;
using BadBroker.Shared.SettingModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BadBroker.Services.ExchangeRates
{
    public class ExchangeRatesService : IExchangeRatesService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExchangeRatesService> _logger;

        private readonly string _apiKey;
        private readonly string _apiUrl;

        public ExchangeRatesService(
            IHttpClientFactory httpClientFactory,
            ILogger<ExchangeRatesService> logger,
            IOptions<ExchangeRatesApi> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            _apiKey = options.Value.ApiKey;
            _apiUrl = options.Value.ApiUrl;
        }

        /// <inheritdoc />
        public async Task<TimeSeriesExchangeRate> GetTimeSeriesExchangeRate(DateTime startDate, DateTime endDate, string baseCurrency, string fromCurrencies)
        {
            using HttpClient httpClient = _httpClientFactory.CreateClient();

            httpClient.BaseAddress = new Uri(_apiUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apiKey", _apiKey);

            string startDateString = startDate.ToString(DateTimeFormatConstants.YYYYMMDD);
            string endDateString = endDate.ToString(DateTimeFormatConstants.YYYYMMDD);

            try
            {
                HttpResponseMessage response = await httpClient
                    .GetAsync($"timeseries?start_date={startDateString}&end_date={endDateString}&base={baseCurrency}&symbols={fromCurrencies}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    
                    return JsonConvert.DeserializeObject<TimeSeriesExchangeRate>(json);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"{nameof(GetTimeSeriesExchangeRate)}: {exception.Message}");
            }

            return null;
        }
    }
}

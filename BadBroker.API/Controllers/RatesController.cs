using BadBroker.Services.Broker;
using BadBroker.Services.ExchangeRates;
using BadBroker.Shared.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BadBroker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatesController : ControllerBase
    {
        private readonly ILogger<RatesController> _logger;
        private readonly IExchangeRatesService _exchangeRatesService;
        private readonly IBrokerService _brokerService;

        public RatesController(ILogger<RatesController> logger, IExchangeRatesService exchangeRatesService, IBrokerService brokerService)
        {
            _logger = logger;
            _exchangeRatesService = exchangeRatesService;
            _brokerService = brokerService;
        }

        [HttpGet("Best")]
        public async Task<IActionResult> Best([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] double moneyUsd)
        {
            const string baseCurrency = "USD";
            const string fromCurrencies = "RUB,EUR,GBP,JPY";
            const string buyCurrency = "RUB";

            if (startDate >= endDate)
            {
                throw new Exception("Start date must be earlier than End date.");
            }

            if (moneyUsd <= 0)
            {
                throw new Exception("Amount of money must be more than zero.");
            }

            TimeSeriesExchangeRate exchangeRate = await _exchangeRatesService.GetTimeSeriesExchangeRate(startDate, endDate, baseCurrency, fromCurrencies);
            BestRevenue bestRevenue = _brokerService.CalculateBestRevenue(exchangeRate, startDate, endDate, moneyUsd, buyCurrency);

            return Ok(bestRevenue);
        }
    }
}

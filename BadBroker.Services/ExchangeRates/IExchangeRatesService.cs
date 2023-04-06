using BadBroker.Shared.ResponseModels;
using System;
using System.Threading.Tasks;

namespace BadBroker.Services.ExchangeRates
{
    public interface IExchangeRatesService
    {
        /// <summary>
        /// Query the Exchange Rates API for daily historical rates between two dates, with a maximum time frame of 365 days.
        /// </summary>
        /// <param name="startDate">The start date of your preferred timeframe.</param>
        /// <param name="endDate">The end date of your preferred timeframe.</param>
        /// <param name="baseCurrency">Enter the three-letter currency code of your preferred base currency.</param>
        /// <param name="fromCurrencies">Enter a list of comma-separated currency codes to limit output currencies.</param>
        /// <returns>Response Object of type TimeSeriesExchangeRate</returns>
        Task<TimeSeriesExchangeRate> GetTimeSeriesExchangeRate(DateTime startDate, DateTime endDate, string baseCurrency, string fromCurrencies);
    }
}

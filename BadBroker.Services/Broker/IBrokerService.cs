using BadBroker.Shared.ResponseModels;
using System;

namespace BadBroker.Services.Broker
{
    public interface IBrokerService
    {
        /// <summary>
        /// Calculate the best revenue.
        /// </summary>
        /// <param name="exchangeRate">Exchange Rates Data from Exchange Rates API.</param>
        /// <param name="buyDate">The date you buy currency.</param>
        /// <param name="sellDate">The date you sell currency.</param>
        /// <param name="moneyUsd">The amount of mount you spend to buy.</param>
        /// <param name="buyCurrency">The currency you buy.</param>
        /// <returns></returns>
        BestRevenue CalculateBestRevenue(TimeSeriesExchangeRate exchangeRate, DateTime buyDate, DateTime sellDate, double moneyUsd, string buyCurrency);
    }
}

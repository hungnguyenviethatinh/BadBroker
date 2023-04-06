using BadBroker.Shared.Constants;
using BadBroker.Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BadBroker.Services.Broker
{
    public class BrokerService : IBrokerService
    {
        /// <inheritdoc />
        public BestRevenue CalculateBestRevenue(TimeSeriesExchangeRate exchangeRate, DateTime buyDate, DateTime sellDate, double moneyUsd, string buyCurrency)
        {
            const double fee = 1.0;

            string buyDateString = buyDate.ToString(DateTimeFormatConstants.YYYYMMDD);
            string sellDateString = sellDate.ToString(DateTimeFormatConstants.YYYYMMDD);

            double? exchangeRateAtBuyDate = exchangeRate.Rates.GetValueOrDefault(buyDateString)?.GetValueOrDefault(buyCurrency);
            double? exchangeRateAtSellDate = exchangeRate.Rates.GetValueOrDefault(sellDateString)?.GetValueOrDefault(buyCurrency);

            double revenue = (exchangeRateAtBuyDate.GetValueOrDefault() * moneyUsd / exchangeRateAtSellDate.GetValueOrDefault(defaultValue: 1.0)) - (int)(sellDate - buyDate).TotalDays * fee;

            return new BestRevenue
            {
                BuyDate = buyDate,
                SellDate = sellDate,
                Tool = buyCurrency,
                Revenue = revenue,
                Rates = exchangeRate.Rates.Select(kvp => new Rate
                {
                    Date = DateTime.Parse(kvp.Key),
                    RUB = kvp.Value.GetValueOrDefault("RUB"),
                    EUR = kvp.Value.GetValueOrDefault("EUR"),
                    GDP = kvp.Value.GetValueOrDefault("GBP"),
                    JPY = kvp.Value.GetValueOrDefault("JPY")
                })
            };
        }
    }
}

using BadBroker.Services.Broker;
using BadBroker.Shared.ResponseModels;
using System;
using System.Collections.Generic;
using Xunit;

namespace BadBroker.API.Tests.Services.Broker
{
    public class BrokerServiceTests
    {
        [Fact]
        public void When_CalculateBestRevenueReturnsBestRevenue_Expect_BestRevenueIsCorrect()
        {
            // Arrange
            var exchangeRate = new TimeSeriesExchangeRate
            {
                Success = true,
                TimeSeries = true,
                StartDate = "2023-04-02",
                EndDate = "2023-04-03",
                Base = "USD",
                Rates = new Dictionary<string, Dictionary<string, double>>
                {
                    { "2023-04-02", new Dictionary<string, double> { { "RUB", 77.99974 }, { "EUR", 0.92544 }, { "GBP", 0.813715 }, { "JPY", 133.296012 } } },
                    { "2023-04-03", new Dictionary<string, double> { { "RUB", 78.650577 }, { "EUR", 0.91655 }, {"GBP", 0.80499 }, {"JPY", 132.304501 } } }
                }
            };

            var buyDate = new DateTime(2023, 04, 02);
            var sellDate = new DateTime(2023, 04, 03);
            double fee = 1.0;
            double moneyUsd = 100.0;
            string buyCurrency = "RUB";

            var expectedBestRevenue = new BestRevenue
            {
                BuyDate = buyDate,
                SellDate = sellDate,
                Tool = "RUB",
                Revenue = (77.99974 * moneyUsd / 78.650577) - (int)(sellDate - buyDate).TotalDays * fee,
                Rates = new List<Rate>
                {
                    new Rate { Date = new DateTime(2023, 04, 02), RUB = 77.99974, EUR = 0.92544, GDP = 0.813715, JPY = 133.296012 },
                    new Rate { Date = new DateTime(2023, 04, 03), RUB = 78.650577, EUR = 0.91655, GDP = 0.80499, JPY = 132.304501 }
                }
            };

            var brokerService = new BrokerService();

            // Act
            BestRevenue bestRevenue = brokerService.CalculateBestRevenue(exchangeRate, buyDate, sellDate, moneyUsd, buyCurrency);

            // Assert
            Assert.Equivalent(expectedBestRevenue, bestRevenue);
        }
    }
}

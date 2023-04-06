using BadBroker.API.Controllers;
using BadBroker.Services.Broker;
using BadBroker.Services.ExchangeRates;
using BadBroker.Shared.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace BadBroker.API.Tests.Controllers
{
    public class RatesControllerTests
    {
        private RatesController CreateTestInstance()
        {
            return CreateTestInstance(out _, out _);
        }

        private RatesController CreateTestInstance(out Mock<IExchangeRatesService> exchangeRatesServiceMock, out Mock<IBrokerService> brokerServiceMock)
        {
            var loggerMock = new Mock<ILogger<RatesController>>();

            exchangeRatesServiceMock = new Mock<IExchangeRatesService>();
            brokerServiceMock = new Mock<IBrokerService>();

            return new RatesController(loggerMock.Object, exchangeRatesServiceMock.Object, brokerServiceMock.Object);
        }

        [Fact]
        public void When_StartDateLaterThanEndDate_Expect_ThrowsException()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(1);
            var endDate = DateTime.Now;
            double moneyUsd = 100.0;

            var rateController = CreateTestInstance();

            // Act and Assert
            Assert.ThrowsAsync<Exception>(() => rateController.Best(startDate, endDate, moneyUsd));
        }

        [Theory]
        [InlineData(-1.0)]
        [InlineData(0.0)]
        public void When_MoneyUsdIsNotGreaterThanZero_Expect_ThrowsException(double moneyUsd)
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            var rateController = CreateTestInstance();

            // Act and Assert
            Assert.ThrowsAsync<Exception>(() => rateController.Best(startDate, endDate, moneyUsd));
        }

        [Fact]
        public async void When_GetTimeSeriesExchangeRateThrowsException_Expect_ThrowsException()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);
            double moneyUsd = 100.0;

            var rateController = CreateTestInstance(out var exchangeRatesServiceMock, out _);

            const string exceptionMessage = "GetTimeSeriesExchangeRate throws exception.";
            exchangeRatesServiceMock.Setup(
                mock => mock
                .GetTimeSeriesExchangeRate(startDate, endDate, It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception(exceptionMessage));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => rateController.Best(startDate, endDate, moneyUsd));

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async void When_CalculateBestRevenueThrowsException_Expect_ThrowsException()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);
            double moneyUsd = 100.0;

            var rateController = CreateTestInstance(out _, out var brokerServiceMock);

            const string exceptionMessage = "CalculateBestRevenue throws exception.";
            brokerServiceMock.Setup(
                mock => mock
                .CalculateBestRevenue(It.IsAny<TimeSeriesExchangeRate>(), startDate, endDate, moneyUsd, It.IsAny<string>()))
                .Throws(new Exception(exceptionMessage));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => rateController.Best(startDate, endDate, moneyUsd));

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async void When_ReturnsResponse_Expect_ResponseStatusCodeIs200()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);
            double moneyUsd = 100.0;

            var rateController = CreateTestInstance(out var exchangeRatesServiceMock, out var brokerServiceMock);

            exchangeRatesServiceMock.Setup(
                mock => mock
                .GetTimeSeriesExchangeRate(startDate, endDate, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new TimeSeriesExchangeRate());

            brokerServiceMock.Setup(
                mock => mock
                .CalculateBestRevenue(It.IsAny<TimeSeriesExchangeRate>(), startDate, endDate, moneyUsd, It.IsAny<string>()))
                .Returns(new BestRevenue());

            // Act
            var response = await rateController.Best(startDate, endDate, moneyUsd) as OkObjectResult;

            // Assert
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void When_ReturnsResponse_Expect_ResponseWithBestValueData()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);
            double moneyUsd = 100.0;

            var rateController = CreateTestInstance(out var exchangeRatesServiceMock, out var brokerServiceMock);

            exchangeRatesServiceMock.Setup(
                mock => mock
                .GetTimeSeriesExchangeRate(startDate, endDate, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new TimeSeriesExchangeRate());

            var bestValue = new BestRevenue();

            brokerServiceMock.Setup(
                mock => mock
                .CalculateBestRevenue(It.IsAny<TimeSeriesExchangeRate>(), startDate, endDate, moneyUsd, It.IsAny<string>()))
                .Returns(bestValue);

            // Act
            var response = await rateController.Best(startDate, endDate, moneyUsd) as OkObjectResult;

            // Assert
            Assert.Equal(bestValue, response.Value);
        }
    }
}

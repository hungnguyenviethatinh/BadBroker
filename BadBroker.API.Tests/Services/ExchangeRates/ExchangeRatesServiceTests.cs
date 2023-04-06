using BadBroker.Services.ExchangeRates;
using BadBroker.Shared.ResponseModels;
using BadBroker.Shared.SettingModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BadBroker.API.Tests.Services.ExchangeRates
{
    public class ExchangeRatesServiceTests
    {
        const string ApiKey = "apikey";
        const string ApiUrl = "https://api.url/";

        private ExchangeRatesService CreateTestInstance(
            out Mock<IHttpClientFactory> httpClientFactoryMock,
            out Mock<ILogger<ExchangeRatesService>> loggerMock,
            out Mock<IOptions<ExchangeRatesApi>> optionsMock)
        {
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            loggerMock = new Mock<ILogger<ExchangeRatesService>>();
            optionsMock = new Mock<IOptions<ExchangeRatesApi>>();

            optionsMock.SetupGet(mock => mock.Value).Returns(new ExchangeRatesApi { ApiKey = ApiKey, ApiUrl = ApiUrl });

            return new ExchangeRatesService(httpClientFactoryMock.Object, loggerMock.Object, optionsMock.Object);
        }

        [Fact]
        public async void When_HttpClientGetAsyncThrowsException_Expect_CallsLogError()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            var service = CreateTestInstance(out var httpClientFactoryMock, out var loggerMock, out var optionsMock);

            const string exceptionMessage = "HttpClient GetAsync throws exception.";

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception(exceptionMessage));

            var httpClientMock = new HttpClient(httpMessageHandlerMock.Object);

            httpClientFactoryMock.Setup(mock => mock.CreateClient(It.IsAny<string>())).Returns(httpClientMock);
            
            // Act
            TimeSeriesExchangeRate exchangeRate = await service.GetTimeSeriesExchangeRate(startDate, endDate, It.IsAny<string>(), It.IsAny<string>());

            // Assert
            loggerMock.Verify(
                mock => mock.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(exceptionMessage)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async void When_HttpClientGetAsyncReturnsResponseWithStatusCodeIsNotOk_Expect_ReturnsNull()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            var service = CreateTestInstance(out var httpClientFactoryMock, out var loggerMock, out var optionsMock);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent });

            var httpClientMock = new HttpClient(httpMessageHandlerMock.Object);

            httpClientFactoryMock.Setup(mock => mock.CreateClient(It.IsAny<string>())).Returns(httpClientMock);

            // Act
            TimeSeriesExchangeRate exchangeRate = await service.GetTimeSeriesExchangeRate(startDate, endDate, It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.Null(exchangeRate);
        }

        [Fact]
        public async void When_HttpClientGetAsyncReturnsResponseWithStatusCodeIsOk_Expect_ReturnsExchangeRate()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            var service = CreateTestInstance(out var httpClientFactoryMock, out var loggerMock, out var optionsMock);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            HttpContent content = new StringContent(@"{
            'success': true,
            'timeseries': true,
            'start_date': '2023-04-02',
            'end_date': '2023-04-03',
            'base': 'USD',
            'rates': {
                '2023-04-02': {
                    'RUB': 77.99974,
                    'EUR': 0.92544,
                    'GBP': 0.813715,
                    'JPY': 133.296012
                },
                '2023-04-03': {
                    'RUB': 78.650577,
                    'EUR': 0.91655,
                    'GBP': 0.80499,
                    'JPY': 132.304501
                },
            }}");

            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = content });

            var httpClientMock = new HttpClient(httpMessageHandlerMock.Object);

            httpClientFactoryMock.Setup(mock => mock.CreateClient(It.IsAny<string>())).Returns(httpClientMock);

            var expectedExchangeRate = new TimeSeriesExchangeRate
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

            // Act
            TimeSeriesExchangeRate exchangeRate = await service.GetTimeSeriesExchangeRate(startDate, endDate, It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.Equivalent(expectedExchangeRate, exchangeRate);
        }
    }
}

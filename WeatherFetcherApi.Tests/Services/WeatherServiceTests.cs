using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using WeatherFetcherApi.Services;

namespace WeatherFetcherApi.Tests.Services;

public class WeatherServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly WeatherService _weatherService;

        public WeatherServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Mock<IConfiguration> configurationMock = new();
            configurationMock.Setup(c => c["OpenWeatherMap:ApiKey"]).Returns("fake-api-key");
            _weatherService = new WeatherService(httpClient, configurationMock.Object);
        }

        [Fact]
        public async Task FetchWeatherDescription_ReturnsWeatherDescription()
        {
            // Arrange
            var cityName = "London";
            var countryName = "UK";
            var responseContent = "{\"weather\":[{\"description\":\"clear sky\"}]}";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _weatherService.FetchWeatherDescription(cityName, countryName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("clear sky", result);
        }

        [Fact]
        public async Task FetchWeatherDescription_HandlesNonSuccessStatusCode()
        {
            // Arrange
            var cityName = "London";
            var countryName = "UK";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _weatherService.FetchWeatherDescription(cityName, countryName));
        }

        [Fact]
        public async Task FetchWeatherDescription_HandlesEmptyWeatherArray()
        {
            // Arrange
            var cityName = "London";
            var countryName = "UK";
            var responseContent = "{\"weather\":[]}";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _weatherService.FetchWeatherDescription(cityName, countryName);

            // Assert
            Assert.Equal("No weather information available", result);
        }
    }

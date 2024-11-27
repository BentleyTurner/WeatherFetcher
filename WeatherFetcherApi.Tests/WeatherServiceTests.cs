using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;
using WeatherFetcherApi.Services;
using WeatherFetcherApi.Models;

namespace WeatherFetcherApi.Tests;

public class WeatherServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly WeatherService _weatherService;

        public WeatherServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new System.Uri("http://localhost")
            };
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["OpenWeatherMap:ApiKey"]).Returns("fake-api-key");
            _weatherService = new WeatherService(_httpClient, _configurationMock.Object);
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

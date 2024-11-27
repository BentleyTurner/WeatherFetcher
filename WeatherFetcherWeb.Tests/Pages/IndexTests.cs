using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using WeatherFetcherWeb.Pages;
using Xunit;

namespace WeatherFetcherWeb.Tests;
public class IndexPageTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<IndexModel>> _loggerMock;
    private readonly IndexModel _pageModel;
    
    private readonly Mock<IConfiguration> _configurationMock;

    public IndexPageTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _loggerMock = new Mock<ILogger<IndexModel>>();
        _configurationMock = new Mock<IConfiguration>();

        // Setup configuration mock to return a list of API keys
        var apiKeys = new List<string> { "test-api-key" };
        var apiKeysSectionMock = new Mock<IConfigurationSection>();
        apiKeysSectionMock.Setup(x => x.Value).Returns(JsonSerializer.Serialize(apiKeys));

        _configurationMock.Setup(config => config.GetSection("ApiKeys")).Returns(apiKeysSectionMock.Object);

        _pageModel = new IndexModel(_httpClient, _loggerMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task OnPostFetchWeatherAsync_SuccessfulResponse_SetsWeatherDescription()
    {
        // Arrange
        _pageModel.CountryName = "TestCountry";
        _pageModel.SelectedApiKey = "test-api-key";
        _pageModel.CityName = "TestCity"; // Ensure CityName is set
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Sunny")
        };
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _pageModel.OnPostFetchWeatherAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Sunny", _pageModel.WeatherDescription);
    }

    [Fact]
    public async Task OnPostFetchWeatherAsync_UnsuccessfulResponse_SetsErrorInWeatherDescription()
    {
        // Arrange
        _pageModel.CountryName = "TestCountry";
        _pageModel.SelectedApiKey = "test-api-key";
        _pageModel.CityName = "TestCity"; // Ensure CityName is set
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _pageModel.OnPostFetchWeatherAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Error fetching weather description. Status code: BadRequest", _pageModel.WeatherDescription);
    }
}

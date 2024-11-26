using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

using Xunit;

namespace WeatherFetcherApi.Tests;

internal class TestWeatherService : IWeatherService
{
    public WeatherDescription FetchWeatherDescription(string cityName, string countryName, string apiKey) 
        => new("Sunny");
}

public abstract class TestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;

    protected TestBase(WebApplicationFactory<Program> factory)
        => Client = factory
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.AddScoped<IWeatherService, TestWeatherService>();
            }))
            .CreateClient();
}

public class WeatherDescriptionEndpointTest(WebApplicationFactory<Program> factory) : TestBase(factory)
{
    [Fact]
    public async Task CallingTheWeatherEndpointShouldReturnTheCorrectWeatherDescription()
    {
        // Arrange
        var cityName = "New York";
        var countryName = "USA";
        var apiKey = "test-api-key";
        var url = $"/weatherdescription?cityName={cityName}&countryName={countryName}&apiKey={apiKey}";

        // Act
        var result = await Client.GetAsync(url);

        // Assert
        result.EnsureSuccessStatusCode();
        var weatherDescription = await result.Content.ReadFromJsonAsync<WeatherDescription>();
        Assert.NotNull(weatherDescription);
        Assert.Equal("Sunny", weatherDescription.Description);
    }
    
}
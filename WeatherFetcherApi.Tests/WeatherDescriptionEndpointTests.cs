using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

using Xunit;

namespace WeatherFetcherApi.Tests;

internal class TestWeatherService : IWeatherService
{
    public Task<WeatherDescription> FetchWeatherDescription(string cityName, string countryName) 
        => Task.FromResult(new WeatherDescription("Clear sky"));
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
        var url = $"/weather-description?cityName={cityName}&countryName={countryName}";

        // Act
        var result = await Client.GetAsync(url);

        // Assert
        result.EnsureSuccessStatusCode();
        var weatherDescription = await result.Content.ReadFromJsonAsync<WeatherDescription>();
        Assert.NotNull(weatherDescription);
        Assert.Equal("Clear sky", weatherDescription.Description);
    }
    
}
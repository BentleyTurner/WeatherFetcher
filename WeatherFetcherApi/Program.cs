using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WeatherFetcherWeb.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OpenWeatherMapSettings>(builder.Configuration.GetSection("OpenWeatherMap"));
builder.Services.AddTransient<IWeatherService, WeatherService>();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();


builder.Services.AddMemoryCache();
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/");
});
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseClientRateLimiting();

app.MapGet("/weather-description", async (HttpContext context, string cityName, string countryName, [FromServices] IWeatherService weatherService) =>
{
    var weatherDescription = await weatherService.FetchWeatherDescription(cityName, countryName);
    await context.Response.WriteAsJsonAsync(weatherDescription);
}).RequireAuthorization();

app.Run();

public record WeatherDescription(string Description);

public interface IWeatherService
{
    Task<WeatherDescription> FetchWeatherDescription(string cityName, string countryName);
}

internal class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly OpenWeatherMapSettings _settings;

    public WeatherService(HttpClient httpClient, IOptions<OpenWeatherMapSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<WeatherDescription> FetchWeatherDescription(string cityName, string countryName)
    {
        var response = await _httpClient.GetAsync($"weather?q={cityName},{countryName}&appid={_settings.ApiKey}");
        response.EnsureSuccessStatusCode();

        var weatherData = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
        var weatherDescription = new WeatherDescription(weatherData.Weather[0].Description);

        return weatherDescription;
    }
}

public partial class Program { }


public class WeatherApiResponse
{
    public Weather[] Weather { get; set; }
}

public class Weather
{
    public string Description { get; set; }
}

public class OpenWeatherMapSettings
{
    public string ApiKey { get; set; }
}
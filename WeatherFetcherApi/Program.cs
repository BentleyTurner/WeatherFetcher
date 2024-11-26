using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IWeatherService, WeatherService>();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();


builder.Services.AddMemoryCache();
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseClientRateLimiting();

app.MapGet("/weatherdescription",
    (string cityName, string countryName, string apiKey, [FromServices] IWeatherService weatherService) =>
        weatherService.FetchWeatherDescription(cityName, countryName, apiKey));

app.Run();

public record WeatherDescription(string Description);

public interface IWeatherService
{
    WeatherDescription FetchWeatherDescription(string cityName, string countryName, string apiKey);
}

internal class WeatherService : IWeatherService
{
    public WeatherDescription FetchWeatherDescription(string cityName, string countryName, string apiKey)
    {
        var weatherDescription = new WeatherDescription(countryName);
        
        return weatherDescription;
    }
}

public partial class Program { }

using Microsoft.AspNetCore.Mvc;
using WeatherFetcherApi.Extensions;
using WeatherFetcherApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomServices(builder.Configuration);

var app = builder.Build();

app.UseCustomMiddleware();

app.MapGet("/weather-description", async (HttpContext context, string cityName, string countryName,
    [FromServices] IWeatherService weatherService) =>
{
    var weatherDescription = await weatherService.FetchWeatherDescription(cityName, countryName);
    await context.Response.WriteAsJsonAsync(weatherDescription);
});

app.Run();


public partial class Program { }

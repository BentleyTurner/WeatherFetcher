using Newtonsoft.Json;
using WeatherFetcherApi.Models;

namespace WeatherFetcherApi.Services;

public class WeatherService(HttpClient httpClient, IConfiguration configuration) : IWeatherService
{
    private readonly string? _apiKey = configuration["OpenWeatherMap:ApiKey"];

    public async Task<string> FetchWeatherDescription(string cityName, string countryName)
    {
        var response = await httpClient.GetStringAsync($"weather?q={cityName},{countryName}&apiKey={_apiKey}");
        var weatherApiResponse = JsonConvert.DeserializeObject<WeatherApiResponse>(response);

        return weatherApiResponse is { Weather.Length: > 0 } ? weatherApiResponse.Weather[0].Description : "No weather information available";
    }
}
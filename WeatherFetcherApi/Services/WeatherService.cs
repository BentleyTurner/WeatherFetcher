using Newtonsoft.Json;
using WeatherFetcherApi.Models;

namespace WeatherFetcherApi.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeatherMap:ApiKey"];
    }

    public async Task<string> FetchWeatherDescription(string cityName, string countryName)
    {
        var response = await _httpClient.GetStringAsync($"weather?q={cityName},{countryName}&apiKey={_apiKey}");
        var weatherApiResponse = JsonConvert.DeserializeObject<WeatherApiResponse>(response);

        if (weatherApiResponse.Weather != null && weatherApiResponse.Weather.Length > 0)
        {
            return weatherApiResponse.Weather[0].Description;
        }

        return "No weather information available";
    }
}
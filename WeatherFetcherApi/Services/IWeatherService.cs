namespace WeatherFetcherApi.Services;

public interface IWeatherService
{ 
    Task<string> FetchWeatherDescription(string cityName, string countryName);
}
namespace WeatherFetcherApi.Models;

public record WeatherDescription(string Description);

public class WeatherApiResponse
{
    public Weather[] Weather { get; set; }
}

public class Weather
{
    public string Description { get; set; }
}
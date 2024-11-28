namespace WeatherFetcherApi.Models;

public record WeatherDescription(string Description);

public class WeatherApiResponse
{
    public required Weather[] Weather { get; set; }
}

public class Weather
{
    public required string Description { get; set; }
}

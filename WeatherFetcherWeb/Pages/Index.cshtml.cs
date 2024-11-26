using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WeatherFetcherWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly HttpClient _httpClient;

    public IndexModel(HttpClient httpClient, ILogger<IndexModel> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    [BindProperty]
    public string CityName { get; set; }

    [BindProperty]
    public string CountryName { get; set; }

    [BindProperty]
    public string ApiKey { get; set; }

    public string WeatherDescription { get; set; }

    public async Task<IActionResult> OnPostFetchWeatherAsync()
    {
        var url = $"http://localhost:6008/weatherdescription?cityName={CityName}&countryName={CountryName}&apiKey={ApiKey}";
        _logger.LogInformation("Requesting weather data from URL: {Url}", url);

        var response = await _httpClient.GetAsync(url);

        _logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);

        if (response.IsSuccessStatusCode)
        {
            var weatherDescription = await response.Content.ReadFromJsonAsync<WeatherDescription>();
            WeatherDescription = weatherDescription.Description;
        }
        else
        {
            WeatherDescription = "Error fetching weather description.";
        }

        return Page();
    }
}

public class WeatherDescription
{
    public string Description { get; set; }
}


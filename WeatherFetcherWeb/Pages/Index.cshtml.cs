using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherFetcherWeb.Models;
using System.Net.Http.Headers;

namespace WeatherFetcherWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IndexModel(HttpClient httpClient, ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            ApiKeys = _configuration.GetSection("ApiKeys").Get<List<string>>();
        }


        [BindProperty]
        public string CityName { get; set; }

        [BindProperty]
        public string CountryName { get; set; }
        
        [BindProperty]
        public string SelectedApiKey { get; set; }
        
        public List<string> ApiKeys { get; set; }

        public string WeatherDescription { get; set; }

        public async Task<IActionResult> OnPostFetchWeatherAsync()
        {
            var url = $"http://localhost:6008/weather-description?cityName={CityName}&countryName={CountryName}";
            _logger.LogInformation("Requesting weather data from URL: {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("ClientIdHeader", SelectedApiKey);

            var response = await _httpClient.SendAsync(request);

            _logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                WeatherDescription = await response.Content.ReadAsStringAsync();
            }
            else
            {
                WeatherDescription = $"Error fetching weather description. Status code: {response.StatusCode}";
            }

            return Page();
        }
    }
}
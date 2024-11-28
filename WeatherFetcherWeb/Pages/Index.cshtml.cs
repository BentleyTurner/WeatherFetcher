using System.ComponentModel.DataAnnotations;
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
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "City name must contain only letters.")]
        public string CityName { get; set; }

        [BindProperty]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Country name must contain only letters.")]
        public string CountryName { get; set; }
        
        [BindProperty]
        public string SelectedApiKey { get; set; }
        
        public List<string> ApiKeys { get; set; }

        public string WeatherDescription { get; set; }

        public async Task<IActionResult> OnPostFetchWeatherAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var baseUrl = _configuration["WeatherApi:BaseUrl"];
            var url = $"{baseUrl}weather-description?cityName={CityName}&countryName={CountryName}";
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
                WeatherDescription = $"Oh no! we cant get the weather right now!";
            }

            return Page();
        }
    }
}
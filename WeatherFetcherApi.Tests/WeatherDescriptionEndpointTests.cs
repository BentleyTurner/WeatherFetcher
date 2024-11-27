using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WeatherFetcherApi.Tests
{
    public class WeatherDescriptionEndpointTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WeatherDescriptionEndpointTest(WebApplicationFactory<Program> factory)
        {
            var clientFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "ApiKeys:0", "your-api-key-here" }
                    });
                });

                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy("ApiKeyPolicy", policy =>
                        {
                            policy.RequireAuthenticatedUser();
                        });
                    });
                });
            });

            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task CallingTheWeatherEndpointShouldReturnTheCorrectWeatherDescription()
        {
            // Arrange
            var cityName = "New York";
            var countryName = "USA";
            var url = $"/weather-description?cityName={cityName}&countryName={countryName}";

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test");

            // Act
            var result = await _client.GetAsync(url);

            // Assert
            if (!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status code {result.StatusCode} and content: {content}");
            }

            result.EnsureSuccessStatusCode();
            var weatherDescription = await result.Content.ReadFromJsonAsync<WeatherDescription>();
            Assert.NotNull(weatherDescription);
            Assert.Equal("clear sky", weatherDescription.Description);
        }
    }

    public class WeatherDescription
    {
        public string Description { get; set; }
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
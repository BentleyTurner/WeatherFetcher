using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace WeatherFetcherWeb.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;
        private const string ApiKeyHeaderName = "ClientIdHeader";

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing API Key"));
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (string.IsNullOrEmpty(providedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
            }

            var apiKeys = _configuration.GetSection("ApiKeys").Get<List<string>>();

            if (!apiKeys.Contains(providedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var identities = new[] { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
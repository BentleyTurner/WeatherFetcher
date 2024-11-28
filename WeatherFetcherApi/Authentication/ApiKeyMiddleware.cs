using System.Diagnostics;

namespace WeatherFetcherApi.Authentication;
public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private const string ApiKeyHeaderName = "ClientIdHeader";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }

        var apiKeys = configuration.GetSection("ApiKeys").Get<List<string>>();
        if (apiKeys != null && !apiKeys.Contains(extractedApiKey!))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client");
            return;
        }

        await next(context);
    }
}
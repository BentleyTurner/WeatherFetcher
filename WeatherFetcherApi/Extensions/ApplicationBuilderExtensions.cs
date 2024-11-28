using AspNetCoreRateLimit;
using WeatherFetcherApi.Authentication;

namespace WeatherFetcherApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseClientRateLimiting();
            app.UseMiddleware<ApiKeyMiddleware>();

            return app;
        }
    }
}

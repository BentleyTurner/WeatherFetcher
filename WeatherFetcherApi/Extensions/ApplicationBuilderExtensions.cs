using AspNetCoreRateLimit;

namespace WeatherFetcherApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseClientRateLimiting();
            app.UseAuthorization();

            return app;
        }
    }
}
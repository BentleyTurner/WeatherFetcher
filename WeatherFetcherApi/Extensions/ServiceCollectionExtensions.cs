using AspNetCoreRateLimit;
using WeatherFetcherApi.Services;

namespace WeatherFetcherApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenWeatherMapSettings>(configuration.GetSection("OpenWeatherMap"));
            services.AddTransient<IWeatherService, WeatherService>();

            services.AddEndpointsApiExplorer()
                .AddSwaggerGen();

            services.AddMemoryCache();
            services.AddOptions();
            services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddDistributedMemoryCache();

            services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            services.AddInMemoryRateLimiting();

            services.AddHttpClient<IWeatherService, WeatherService>(client =>
            {
                var baseUrl = configuration["OpenWeatherMap:BaseUrl"];
                if (baseUrl != null) client.BaseAddress = new Uri(baseUrl);
            });

            return services;
        }
    }

    public class OpenWeatherMapSettings
    {
        public string? ApiKey { get; set; }
    }
}


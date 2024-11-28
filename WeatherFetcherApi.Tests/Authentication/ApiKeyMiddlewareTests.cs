using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using WeatherFetcherApi.Authentication;

namespace WeatherFetcherApi.Tests.Authentication;

public class ApiKeyMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ApiKeyMiddleware _middleware;
    private const string ClientIdHeaderName = "X-CLIENT-ID";

    public ApiKeyMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _configurationMock = new Mock<IConfiguration>();
        _middleware = new ApiKeyMiddleware(_nextMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_MissingApiKey_Returns401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = new StreamReader(context.Response.Body).ReadToEnd();
        Assert.Equal("API Key is missing", response);
    }

    [Fact]
    public async Task InvokeAsync_InvalidApiKey_Returns401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-KEY"] = "invalid-api-key";
        context.Request.Headers[ClientIdHeaderName] = "client-id";
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = new StreamReader(context.Response.Body).ReadToEnd();
        Assert.Equal("API Key is missing", response);
    }
}

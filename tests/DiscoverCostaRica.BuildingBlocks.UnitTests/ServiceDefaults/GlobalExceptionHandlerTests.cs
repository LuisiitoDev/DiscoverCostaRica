using DiscoverCostaRica.ServiceDefaults.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscoverCostaRica.BuildingBlocks.UnitTests.ServiceDefaults;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task UnhandledException_BecomesAGeneric500ProblemDetails_WithoutLeakingTheMessage()
    {
        var context = new DefaultHttpContext();
        await using var body = new MemoryStream();
        context.Response.Body = body;
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        var handled = await handler.TryHandleAsync(context, new InvalidOperationException("connection string secret=abc"), TestContext.Current.CancellationToken);

        var json = System.Text.Encoding.UTF8.GetString(body.ToArray());
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Contains("\"title\":\"An unexpected error occurred\"", json);
        Assert.DoesNotContain("secret", json);
    }
}

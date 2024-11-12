using System.Text.Json;
using DiscoverCostaRica.Api.Models;

namespace DiscoverCostaRica.Api.Middleware;
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandle exception has occurred.");
            await HandleExceptionAsync(context);
        }
    }

    private Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsync(JsonSerializer.Serialize(
            Result<object>.Failure("An error has occurred, try later")
        ));
    }
}
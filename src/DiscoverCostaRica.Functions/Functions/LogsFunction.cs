using DiscoverCostaRica.Functions.Interfaces;
using DiscoverCostaRica.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DiscoverCostaRica.Functions.Functions;

public class LogsFunction(IMongoService mongo, ILogger<LogsFunction> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Function("LogsFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            if (req.ContentLength == 0 || req.ContentLength == null)
            {
                logger.LogWarning("Request received with empty body");
                return new BadRequestObjectResult(new { error = "Request body cannot be empty" });
            }

            using var ms = new MemoryStream();
            await req.Body.CopyToAsync(ms);
            var body = ms.ToArray();

            if (body.Length == 0)
            {
                logger.LogWarning("Request body is empty after reading");
                return new BadRequestObjectResult(new { error = "Invalid request body" });
            }

            var log = JsonSerializer.Deserialize<LogModel>(body, JsonOptions);
            
            if (log == null)
            {
                logger.LogWarning("Failed to deserialize log model");
                return new BadRequestObjectResult(new { error = "Invalid log format" });
            }

            await mongo.Log(log);
            logger.LogInformation("Log entry saved successfully. Category: {Category}", log.Category);

            return new OkObjectResult(new { message = "Log entry saved successfully", id = log.Id });
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON deserialization failed");
            return new BadRequestObjectResult(new { error = "Invalid JSON format", details = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while processing log entry");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
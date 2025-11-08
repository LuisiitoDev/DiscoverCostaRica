using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;
namespace DiscoverCostaRica.Shared.Utils;

public static class SharedExtensions
{
    extension<T>(Result<T> result)
    {
        public IResult ToResult()
        {
            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Results.Ok(result),
                StatusCodes.Status404NotFound => Results.NotFound(result),
                StatusCodes.Status400BadRequest => Results.BadRequest(result),
                StatusCodes.Status500InternalServerError => Results.InternalServerError(),
                _ => Results.Problem(result.Message, statusCode: result.StatusCode)
            };
        }
    }
}

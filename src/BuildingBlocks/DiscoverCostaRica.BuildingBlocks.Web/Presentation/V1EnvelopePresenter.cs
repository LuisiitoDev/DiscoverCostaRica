using DiscoverCostaRica.BuildingBlocks.Application.Results;
using Microsoft.AspNetCore.Http;

namespace DiscoverCostaRica.BuildingBlocks.Web.Presentation;

/// <summary>
/// Presenter for API v1: turns a use-case <see cref="Result{T}"/> into the v1 HTTP response.
/// Use cases never see status codes; this is the one place that maps them.
/// </summary>
public static class V1EnvelopePresenter
{
    public static IResult ToV1Response<T>(this Result<T> result) => result.Match(
        value => Results.Ok(new V1Envelope<T>(value, StatusCodes.Status200OK, null)),
        Present<T>);

    private static IResult Present<T>(Error error) => error.Type switch
    {
        ErrorType.NotFound => Results.NotFound(new V1Envelope<T>(default, StatusCodes.Status404NotFound, error.Message)),
        ErrorType.Validation => Results.BadRequest(new V1Envelope<T>(default, StatusCodes.Status400BadRequest, error.Message)),
        ErrorType.Unavailable => Results.Problem(
            title: "A dependency is unavailable",
            detail: error.Message,
            statusCode: StatusCodes.Status503ServiceUnavailable),
        // As built, v1 answers unexpected failures with an empty 500.
        _ => Results.InternalServerError(),
    };
}

using System.Text.Json.Serialization;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Web.Presentation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DiscoverCostaRica.BuildingBlocks.UnitTests.Web;

/// <summary>The presenter must produce the frozen v1 bytes that the characterization suite pins for the legacy code.</summary>
public sealed class V1EnvelopePresenterTests
{
    private sealed record Province(int Id, string Name);

    [Fact]
    public async Task Success_WritesValueThenStatusCode()
    {
        Result<IReadOnlyList<Province>> result = new List<Province> { new(1, "San José"), new(2, "Alajuela") };

        var (status, body) = await ExecuteAsync(result.ToV1Response());

        Assert.Equal(StatusCodes.Status200OK, status);
        Assert.Equal("""{"value":[{"id":1,"name":"San José"},{"id":2,"name":"Alajuela"}],"statusCode":200}""", body);
    }

    [Fact]
    public async Task NotFound_WritesStatusCodeThenMessage()
    {
        Result<Province> result = Error.NotFound("No canton was found for 99");

        var (status, body) = await ExecuteAsync(result.ToV1Response());

        Assert.Equal(StatusCodes.Status404NotFound, status);
        Assert.Equal("""{"statusCode":404,"message":"No canton was found for 99"}""", body);
    }

    [Fact]
    public async Task Validation_Writes400Envelope()
    {
        Result<Province> result = Error.Validation("items must contain between 1 and 200 entries");

        var (status, body) = await ExecuteAsync(result.ToV1Response());

        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.Equal("""{"statusCode":400,"message":"items must contain between 1 and 200 entries"}""", body);
    }

    [Fact]
    public async Task Unavailable_WritesProblemDetails503()
    {
        Result<Province> result = Error.Unavailable("Geo did not answer");

        var (status, body) = await ExecuteAsync(result.ToV1Response());

        Assert.Equal(StatusCodes.Status503ServiceUnavailable, status);
        Assert.Contains("\"detail\":\"Geo did not answer\"", body);
    }

    [Fact]
    public async Task Unexpected_WritesEmpty500AsBuilt()
    {
        Result<Province> result = Error.Unexpected("boom");

        var (status, body) = await ExecuteAsync(result.ToV1Response());

        Assert.Equal(StatusCodes.Status500InternalServerError, status);
        Assert.Equal(string.Empty, body);
    }

    private static async Task<(int Status, string Body)> ExecuteAsync(IResult result)
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails()
            // Same JSON options as ServiceDefaults.AddJsonOptions.
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            })
            .BuildServiceProvider();

        var context = new DefaultHttpContext { RequestServices = services };
        await using var body = new MemoryStream();
        context.Response.Body = body;

        await result.ExecuteAsync(context);

        return (context.Response.StatusCode, System.Text.Encoding.UTF8.GetString(body.ToArray()));
    }
}

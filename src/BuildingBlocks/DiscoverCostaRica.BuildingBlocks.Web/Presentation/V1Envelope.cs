using System.Text.Json.Serialization;

namespace DiscoverCostaRica.BuildingBlocks.Web.Presentation;

/// <summary>
/// Wire shape of every v1 response body (frozen, see contracts/public-api-v1.md).
/// Property order and null omission are part of the contract.
/// </summary>
public sealed record V1Envelope<T>(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] T? Value,
    int StatusCode,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Message);

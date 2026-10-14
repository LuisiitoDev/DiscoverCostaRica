namespace DiscoverCostaRica.BuildingBlocks.Application.Results;

/// <summary>Why a use case did not succeed. Transport-neutral: presenters decide what each means on the wire.</summary>
public enum ErrorType
{
    NotFound,
    Validation,
    Unavailable,
    Unexpected,
}

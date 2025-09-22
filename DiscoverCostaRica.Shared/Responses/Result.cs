namespace DiscoverCostaRica.Shared.Responses;

public record Result(int StatusCode, string? Message = null);
public sealed record Success(object Value, int StatusCode = 200) : Result(StatusCode);
public sealed record Failure(string Message, int StatusCode = 500) : Result(StatusCode, Message);

public sealed record Result<TResult>(int StatusCode, string? Message) : Result(StatusCode, Message)
{
    public TResult? Value { get; set; }

    public static implicit operator Result<TResult>(Success success)
        => new(success.StatusCode, null) { Value = (TResult)success.Value };

    public static implicit operator Result<TResult>(Failure failure)
        => new(failure.StatusCode, failure.Message) { Value = default };
}
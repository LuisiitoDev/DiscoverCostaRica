using System.Diagnostics.CodeAnalysis;

namespace DiscoverCostaRica.BuildingBlocks.Application.Results;

/// <summary>
/// Outcome of a use case: either a value or an <see cref="Results.Error"/>, never both.
/// A class rather than a struct so that <c>default</c> cannot pose as a success.
/// </summary>
public sealed class Result<T>
{
    private Result(T value) => Value = value;

    private Result(Error error) => Error = error;

    public T? Value { get; }

    public Error? Error { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(value);
    }

    public static Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(Error error) => Failure(error);

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Error);

    /// <summary>Transforms the value of a success (e.g. into a wire contract); a failure passes through unchanged.</summary>
    public Result<TOut> Map<TOut>(Func<T, TOut> map) =>
        IsSuccess ? Result<TOut>.Success(map(Value)) : Result<TOut>.Failure(Error);
}

namespace DiscoverCostaRica.Api.Models;

public class Result<T>
{
    public bool IsNotFound { get; set; }
    public bool IsSuccess { get; set; }
    public T Value { get; set; }
    public string Error { get; set; }


    public static Result<T> Success(T value)
    {
        return new Result<T> { IsSuccess = true, Value = value };
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T> { IsSuccess = false, Error = error };
    }

    public static Result<T> NotFound(string error)
    {
        return new Result<T>() { IsNotFound = true, Error = error };
    }

    // Implicit operator from T to Result<T>
    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    // Implicit operator from string to Result<T> (for errors)
    public static implicit operator Result<T>(string error)
    {
        return Failure(error);
    }
}
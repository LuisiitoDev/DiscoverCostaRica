using DiscoverCostaRica.Shared.Responses;

namespace DiscoverCostaRica.Shared.Utils;

public static class SharedExtensions
{
    public static class Result
    {
        public static Success<TResult> Success<TResult>(TResult value, int statusCode = 200)
        {
            return new Success<TResult>(value, statusCode);
        }
    }

}

using System.Net;

namespace DiscoverCostaRica.CharacterizationTests.Harness;

internal static class ResponseAssert
{
    /// <summary>Asserts the status code and the exact body bytes (property order, casing, null omission).</summary>
    public static async Task Exactly(HttpResponseMessage response, HttpStatusCode status, string body)
    {
        var actual = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.True(status == response.StatusCode, $"Expected {(int)status} but got {(int)response.StatusCode}: {actual}");
        Assert.Equal(body, actual);
    }

    public static async Task<string> Status(HttpResponseMessage response, HttpStatusCode status)
    {
        var actual = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.True(status == response.StatusCode, $"Expected {(int)status} but got {(int)response.StatusCode}: {actual}");
        return actual;
    }
}

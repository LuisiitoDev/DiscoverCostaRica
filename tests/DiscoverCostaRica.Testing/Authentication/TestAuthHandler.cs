using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscoverCostaRica.Testing.Authentication;

/// <summary>
/// Replaces Entra ID in tests. A request authenticates with
/// <c>Authorization: Bearer test:Role.One,Role.Two</c> and gets those app roles,
/// emitted with the same claim type that JwtBearer maps the Entra <c>roles</c> claim to.
/// </summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    private const string TokenPrefix = "Bearer test:";

    public static string TokenFor(params string[] roles) => $"test:{string.Join(',', roles)}";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (!header.StartsWith(TokenPrefix, StringComparison.Ordinal))
            return Task.FromResult(AuthenticateResult.NoResult());

        var roles = header[TokenPrefix.Length..].Split(',', StringSplitOptions.RemoveEmptyEntries);
        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role))
            .Append(new Claim(ClaimTypes.NameIdentifier, "test-user"));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }
}

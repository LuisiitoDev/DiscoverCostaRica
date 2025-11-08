using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DiscoverCostaRica.Shared.Authentication;

/// <summary>
/// Implementation of ICurrentUserService using IHttpContextAccessor
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirst(AuthConstants.ClaimTypes.ObjectId)?.Value
                            ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value
                           ?? User?.FindFirst("preferred_username")?.Value;

    public string? Name => User?.FindFirst(ClaimTypes.Name)?.Value
                          ?? User?.FindFirst("name")?.Value;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Scopes
    {
        get
        {
            var scopeClaim = User?.FindFirst(AuthConstants.ClaimTypes.Scope)?.Value
                            ?? User?.FindFirst("scp")?.Value;

            if (string.IsNullOrWhiteSpace(scopeClaim))
            {
                return Enumerable.Empty<string>();
            }

            return scopeClaim.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public bool HasScope(string scope)
    {
        return Scopes.Contains(scope, StringComparer.OrdinalIgnoreCase);
    }
}

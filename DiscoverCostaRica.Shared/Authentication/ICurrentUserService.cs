namespace DiscoverCostaRica.Shared.Authentication;

/// <summary>
/// Service for accessing current authenticated user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's unique identifier (Object ID from Azure AD)
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current user's email address
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user's display name
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets whether the current request is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the scopes granted to the current user
    /// </summary>
    IEnumerable<string> Scopes { get; }

    /// <summary>
    /// Checks if the current user has a specific scope
    /// </summary>
    /// <param name="scope">The scope to check</param>
    /// <returns>True if the user has the scope, false otherwise</returns>
    bool HasScope(string scope);
}

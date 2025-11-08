using DiscoverCostaRica.Shared.Attributes;
using static DiscoverCostaRica.Shared.Authentication.AuthConstants;

namespace DiscoverCostaRica.Shared.Authentication;

[AuthorizationPolicy(Policies.BeachesRead, Scopes.BeachesRead)]
[AuthorizationPolicy(Policies.VolcanoRead, Scopes.VolcanoRead)]
[AuthorizationPolicy(Policies.CultureRead, Scopes.CultureRead)]
[AuthorizationPolicy(Policies.GeoRead, Scopes.GeoRead)]
public class DiscoverPolicies { }

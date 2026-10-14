using static DiscoverCostaRica.BuildingBlocks.Web.Authentication.AuthConstants;

namespace DiscoverCostaRica.BuildingBlocks.Web.Authentication;

[AuthorizationPolicy(Policies.BeachesRead, Scopes.BeachesRead)]
[AuthorizationPolicy(Policies.VolcanoRead, Scopes.VolcanoRead)]
[AuthorizationPolicy(Policies.CultureRead, Scopes.CultureRead)]
[AuthorizationPolicy(Policies.GeoRead, Scopes.GeoRead)]
public class DiscoverPolicies { }

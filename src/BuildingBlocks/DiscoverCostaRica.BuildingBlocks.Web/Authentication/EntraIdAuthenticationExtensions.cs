using DiscoverCostaRica.BuildingBlocks.Web.Authentication;
using DiscoverCostaRica.Generated;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace Microsoft.Extensions.Hosting;

public static class EntraIdAuthenticationExtensions
{
    /// <summary>Validates Entra ID bearer tokens and registers the app-role policies (generated from <see cref="DiscoverPolicies"/>).</summary>
    public static IHostApplicationBuilder AddEntraIdAuthentication(this IHostApplicationBuilder builder)
    {
        var entraIdOptions = builder.Configuration.GetSection(EntraIdOptions.SectionName).Get<EntraIdOptions>()
            ?? throw new InvalidOperationException($"Configuration section '{EntraIdOptions.SectionName}' is required.");

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(jwtOptions =>
            {
                jwtOptions.RequireHttpsMetadata = false;
                jwtOptions.Events = new DiscoverCostaRicaBearerEvents();
                jwtOptions.TokenValidationParameters.ValidAudiences = [entraIdOptions.Audience];
                jwtOptions.TokenValidationParameters.RoleClaimType = "roles";
            }, identityOptions =>
            {
                identityOptions.Instance = entraIdOptions.Instance;
                identityOptions.TenantId = entraIdOptions.TenantId;
                identityOptions.ClientId = entraIdOptions.ClientId;
            });

        builder.Services.AddPolicies();

        return builder;
    }
}

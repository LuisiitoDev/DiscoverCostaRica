using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Shared.Authentication;

public class DiscoverCostaRicaBearerEvents : JwtBearerEvents
{
    public DiscoverCostaRicaBearerEvents()
    {
        OnAuthenticationFailed = OnAuthenticationHasFailed;
        OnTokenValidated = OnTokenHasBeenValidated;
        OnChallenge = OnAuthenticationIsChalleging;
    }

    private Task OnAuthenticationHasFailed(AuthenticationFailedContext context)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<DiscoverCostaRicaBearerEvents>>();
        logger.LogError(context.Exception, "Authentication failed: {Message}", context.Exception.Message);
        return Task.CompletedTask;
    }

    private Task OnTokenHasBeenValidated(TokenValidatedContext context)
    {
        return Task.CompletedTask;
    }

    private Task OnAuthenticationIsChalleging(JwtBearerChallengeContext context)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<DiscoverCostaRicaBearerEvents>>();
        logger.LogWarning("Authentication challenge issued: {Error}, {ErrorDescription}", context.Error, context.ErrorDescription);
        return Task.CompletedTask;
    }
}

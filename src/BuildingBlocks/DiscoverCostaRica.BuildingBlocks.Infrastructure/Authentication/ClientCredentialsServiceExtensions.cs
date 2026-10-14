using DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class ClientCredentialsServiceExtensions
{
    /// <summary>Registers <see cref="IAccessTokenProvider"/> and <see cref="BearerTokenHandler"/> for outbound calls.</summary>
    public static IHostApplicationBuilder AddClientCredentialsTokenProvider(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<ClientCredentialsOptions>()
            .BindConfiguration(ClientCredentialsOptions.SectionName);
        builder.Services.AddSingleton<IAccessTokenProvider, ClientCredentialsTokenProvider>();
        builder.Services.AddTransient<BearerTokenHandler>();

        return builder;
    }
}

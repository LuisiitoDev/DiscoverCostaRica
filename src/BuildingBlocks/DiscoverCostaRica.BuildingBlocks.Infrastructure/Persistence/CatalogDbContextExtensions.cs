using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class CatalogDbContextExtensions
{
    public const string DefaultConnectionName = "DiscoverCostaRica";

    /// <summary>
    /// Registers a service's EF Core context on Azure SQL with transient-fault retries, and exposes it
    /// through the narrow interface the service's repositories depend on.
    /// </summary>
    public static IHostApplicationBuilder AddCatalogDbContext<TContextInterface, TContext>(
        this IHostApplicationBuilder builder,
        string connectionName = DefaultConnectionName)
        where TContextInterface : class
        where TContext : DbContext, TContextInterface
    {
        builder.AddSqlServerDbContext<TContext>(
            connectionName,
            configureDbContextOptions: options => options.UseSqlServer(sql =>
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)));

        builder.Services.AddScoped<TContextInterface>(provider => provider.GetRequiredService<TContext>());

        return builder;
    }
}

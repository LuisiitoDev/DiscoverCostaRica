using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class CatalogDbContextExtensions
{
    public const string DefaultConnectionName = "DiscoverCostaRica";

    /// <summary>Registers a service's EF Core context on Azure SQL with transient-fault retries.</summary>
    public static IHostApplicationBuilder AddCatalogDbContext<TContext>(
        this IHostApplicationBuilder builder,
        string connectionName = DefaultConnectionName)
        where TContext : DbContext
    {
        builder.AddSqlServerDbContext<TContext>(
            connectionName,
            configureDbContextOptions: options => options.UseSqlServer(sql =>
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)));

        return builder;
    }
}

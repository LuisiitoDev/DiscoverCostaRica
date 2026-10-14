using DiscoverCostaRica.Beaches.Infrastructure.Context;
using DiscoverCostaRica.Culture.Infrastructure.Context;
using DiscoverCostaRica.Geo.Infrastructure.Context;
using DiscoverCostaRica.Testing.Containers;
using DiscoverCostaRica.VolcanoService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.CharacterizationTests.Harness;

/// <summary>
/// Builds a catalog database the way production gets it: every service's EF migrations,
/// applied to one shared database (as built, see PLT-D2).
/// </summary>
public static class CatalogDatabase
{
    public static async Task<string> CreateAsync(SqlServerFixture sql, bool seeded, CancellationToken cancellationToken = default)
    {
        var connectionString = await sql.CreateDatabaseAsync(seeded ? "catalog" : "empty", cancellationToken);

        await MigrateAsync<GeoContext>(connectionString, o => new GeoContext(o), cancellationToken);
        await MigrateAsync<BeachContext>(connectionString, o => new BeachContext(o), cancellationToken);
        await MigrateAsync<CultureContext>(connectionString, o => new CultureContext(o), cancellationToken);
        await MigrateAsync<VolcanoContext>(connectionString, o => new VolcanoContext(o), cancellationToken);

        if (seeded)
            await SqlServerFixture.ExecuteAsync(connectionString, BaselineData.Sql, cancellationToken);

        return connectionString;
    }

    private static async Task MigrateAsync<TContext>(
        string connectionString,
        Func<DbContextOptions<TContext>, TContext> create,
        CancellationToken cancellationToken) where TContext : DbContext
    {
        var options = new DbContextOptionsBuilder<TContext>().UseSqlServer(connectionString).Options;
        await using var context = create(options);
        await context.Database.MigrateAsync(cancellationToken);
    }
}

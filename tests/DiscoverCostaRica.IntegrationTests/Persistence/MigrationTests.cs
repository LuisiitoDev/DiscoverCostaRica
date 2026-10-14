using DiscoverCostaRica.Beaches.Infrastructure.Context;
using DiscoverCostaRica.Culture.Infrastructure.Context;
using DiscoverCostaRica.Geo.Infrastructure.Context;
using DiscoverCostaRica.Testing.Containers;
using DiscoverCostaRica.Volcano.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.IntegrationTests.Persistence;

/// <summary>
/// Every service's migrations apply to an empty SQL Server and match its current model.
/// Guards package upgrades (EF Core 9 → 10) and model edits that forget a migration.
/// </summary>
public sealed class MigrationTests(SqlServerFixture sql)
{
    public static TheoryData<string> Contexts => [nameof(GeoContext), nameof(BeachContext), nameof(CultureContext), nameof(VolcanoContext)];

    [Theory]
    [MemberData(nameof(Contexts))]
    public async Task Migrations_Apply_AndLeaveNoPendingModelChanges(string contextName)
    {
        var ct = TestContext.Current.CancellationToken;
        var connectionString = await sql.CreateDatabaseAsync(contextName, ct);
        await using var context = Create(contextName, connectionString);

        await context.Database.MigrateAsync(ct);

        Assert.Empty(await context.Database.GetPendingMigrationsAsync(ct));
        Assert.False(context.Database.HasPendingModelChanges(), $"{contextName} model differs from its last migration; add a migration.");
    }

    private static DbContext Create(string name, string connectionString) => name switch
    {
        nameof(GeoContext) => new GeoContext(Options<GeoContext>(connectionString)),
        nameof(BeachContext) => new BeachContext(Options<BeachContext>(connectionString)),
        nameof(CultureContext) => new CultureContext(Options<CultureContext>(connectionString)),
        nameof(VolcanoContext) => new VolcanoContext(Options<VolcanoContext>(connectionString)),
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
    };

    private static DbContextOptions<T> Options<T>(string connectionString) where T : DbContext =>
        new DbContextOptionsBuilder<T>().UseSqlServer(connectionString).Options;
}

using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace DiscoverCostaRica.Testing.Containers;

/// <summary>
/// One SQL Server container per test assembly. Each test class asks for its own database,
/// so tests that mutate data never see each other.
/// </summary>
public sealed class SqlServerFixture : IAsyncLifetime
{
    private const string Image = "mcr.microsoft.com/mssql/server:2022-latest";

    private readonly MsSqlContainer _container = new MsSqlBuilder(Image).Build();

    public async ValueTask InitializeAsync() => await _container.StartAsync();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync();

    /// <summary>Creates an empty database and returns its connection string.</summary>
    public async Task<string> CreateDatabaseAsync(string name, CancellationToken cancellationToken = default)
    {
        var database = $"{name}_{Guid.NewGuid():N}";

        await using (var connection = new SqlConnection(_container.GetConnectionString()))
        {
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE [{database}]";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        return new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = database,
            TrustServerCertificate = true,
        }.ConnectionString;
    }

    /// <summary>Runs a T-SQL batch against the given database.</summary>
    public static async Task ExecuteAsync(string connectionString, string sql, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}

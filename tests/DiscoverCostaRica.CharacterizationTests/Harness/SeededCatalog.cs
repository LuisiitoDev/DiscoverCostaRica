using DiscoverCostaRica.Testing.Containers;

namespace DiscoverCostaRica.CharacterizationTests.Harness;

/// <summary>Read-only seeded catalog shared by the tests of one class. Tests that write data build their own.</summary>
public sealed class SeededCatalog(SqlServerFixture sql) : IAsyncLifetime
{
    private string? _connectionString;
    private GeoApiFactory? _geo;
    private BeachesApiFactory? _beaches;
    private CultureApiFactory? _culture;
    private VolcanoApiFactory? _volcano;

    public string ConnectionString => _connectionString ?? throw new InvalidOperationException("Not initialized.");

    public GeoApiFactory Geo => _geo ??= new GeoApiFactory(ConnectionString);

    public BeachesApiFactory Beaches => _beaches ??= new BeachesApiFactory(ConnectionString);

    public CultureApiFactory Culture => _culture ??= new CultureApiFactory(ConnectionString);

    public VolcanoApiFactory Volcano => _volcano ??= new VolcanoApiFactory(ConnectionString, Geo);

    public async ValueTask InitializeAsync() =>
        _connectionString = await CatalogDatabase.CreateAsync(sql, seeded: true, TestContext.Current.CancellationToken);

    public async ValueTask DisposeAsync()
    {
        if (_volcano is not null) await _volcano.DisposeAsync();
        if (_culture is not null) await _culture.DisposeAsync();
        if (_beaches is not null) await _beaches.DisposeAsync();
        if (_geo is not null) await _geo.DisposeAsync();
    }
}

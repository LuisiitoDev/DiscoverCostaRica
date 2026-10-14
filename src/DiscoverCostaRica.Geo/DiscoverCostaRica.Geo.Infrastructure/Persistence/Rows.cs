namespace DiscoverCostaRica.Geo.Infrastructure.Persistence;

// Persistence shapes of the Geo tables. EF Core cannot key an entity on value objects, so the
// domain entities stay pure and these rows exist only inside Infrastructure (research.md R3).

internal sealed class ProvinceRow
{
    public int Id { get; set; }

    public required string Name { get; set; }
}

internal sealed class CantonRow
{
    public int Id { get; set; }

    public int ProvinceId { get; set; }

    public required string Name { get; set; }
}

internal sealed class DistrictRow
{
    public int Id { get; set; }

    public int CantonId { get; set; }

    public int CantonProvinceId { get; set; }

    public required string Name { get; set; }
}

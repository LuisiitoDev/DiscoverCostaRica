namespace DiscoverCostaRica.Volcano.Domain;

public sealed class Volcano
{
    private Volcano(int id, string name, string description, LocationCode location)
    {
        Id = id;
        Name = name;
        Description = description;
        Location = location;
    }

    public int Id { get; }

    public string Name { get; }

    public string Description { get; }

    public LocationCode Location { get; }

    public static Volcano Create(int id, string name, string description, LocationCode location)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentNullException.ThrowIfNull(location);
        return new Volcano(id, name, description, location);
    }
}

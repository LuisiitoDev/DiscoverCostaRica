namespace DiscoverCostaRica.Beaches.Domain;

public sealed class Beach
{
    public const int MaxNameLength = 1000;
    public const int MaxDescriptionLength = 10000;

    private Beach(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public int Id { get; }

    public string Name { get; }

    public string Description { get; }

    public static Beach Create(int id, string name, string description)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, MaxNameLength, nameof(name));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(description.Length, MaxDescriptionLength, nameof(description));
        return new Beach(id, name, description);
    }
}

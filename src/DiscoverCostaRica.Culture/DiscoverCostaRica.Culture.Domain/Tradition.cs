namespace DiscoverCostaRica.Culture.Domain;

public sealed class Tradition
{
    private Tradition(int id, string name, string description, string? imageUrl)
    {
        Id = id;
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
    }

    public int Id { get; }

    public string Name { get; }

    public string Description { get; }

    /// <summary>Absolute URL of a picture, as curated; optional.</summary>
    public string? ImageUrl { get; }

    public static Tradition Create(int id, string name, string description, string? imageUrl = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        return new Tradition(id, name, description, imageUrl);
    }
}

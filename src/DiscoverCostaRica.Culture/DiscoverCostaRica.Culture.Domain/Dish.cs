namespace DiscoverCostaRica.Culture.Domain;

/// <summary>A typical Costa Rican dish with how to make it.</summary>
public sealed class Dish
{
    private Dish(int id, string name, string description, string ingredients, string preparation, string? imageUrl)
    {
        Id = id;
        Name = name;
        Description = description;
        Ingredients = ingredients;
        Preparation = preparation;
        ImageUrl = imageUrl;
    }

    public int Id { get; }

    public string Name { get; }

    public string Description { get; }

    public string Ingredients { get; }

    public string Preparation { get; }

    /// <summary>Absolute URL of a picture, as curated; optional.</summary>
    public string? ImageUrl { get; }

    public static Dish Create(int id, string name, string description, string ingredients, string preparation, string? imageUrl = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(ingredients);
        ArgumentException.ThrowIfNullOrWhiteSpace(preparation);
        return new Dish(id, name, description, ingredients, preparation, imageUrl);
    }
}

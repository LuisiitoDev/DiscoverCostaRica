namespace DiscoverCostaRica.Culture.Domain.Models;

public class DishModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Preparation { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

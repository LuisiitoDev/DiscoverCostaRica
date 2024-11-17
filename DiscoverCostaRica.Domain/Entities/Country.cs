namespace DiscoverCostaRica.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CountryCode { get; set; }
    public int Population { get; set; }
    public string Location { get; set; }
    public Province Capital { get; set; }
}

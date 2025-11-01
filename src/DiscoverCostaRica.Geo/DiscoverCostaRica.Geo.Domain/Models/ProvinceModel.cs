namespace DiscoverCostaRica.Geo.Domain.Models;

public class ProvinceModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required ICollection<CantonModel> Cantons { get; set; }
}

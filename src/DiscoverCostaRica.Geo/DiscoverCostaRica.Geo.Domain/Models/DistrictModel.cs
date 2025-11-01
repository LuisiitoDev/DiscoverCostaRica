namespace DiscoverCostaRica.Geo.Domain.Models;

public class DistrictModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int CantonId { get; set; }
    public required CantonModel Canton { get; set; }
}

namespace DiscoverCostaRica.Geo.Domain.Models;

public class CantonModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int ProvinceId { get; set; }
    public required ProvinceModel Province { get; set; }
    public required ICollection<DistrictModel> Districts { get; set; }
}

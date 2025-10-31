namespace DiscoverCostaRica.VolcanoService.Domain.Models;

public class VolcanoModel
{
    public int Id { get; set; }
    public required int ProvinceId { get; set; }
    public required int CantonId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}

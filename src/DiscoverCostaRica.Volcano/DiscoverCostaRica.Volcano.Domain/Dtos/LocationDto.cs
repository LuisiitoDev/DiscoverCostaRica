namespace DiscoverCostaRica.Volcano.Domain.Dtos;

public record LocationDto
{
    public ProvinceDto? Province { get; set; }
    public CantonDto? Canton { get; set; }
    public DistrictDto? District { get; set; }
}

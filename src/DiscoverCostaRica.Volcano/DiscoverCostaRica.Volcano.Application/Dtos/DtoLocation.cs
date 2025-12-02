using DiscoverCostaRica.Shared.Dtos;

namespace DiscoverCostaRica.Volcano.Application.Dtos;

public record class DtoLocation
{
    public DtoProvince? Province { get; set; }
    public DtoCanton? Canton { get; set; }
    public DtoDistrict? District { get; set; }
}

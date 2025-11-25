namespace DiscoverCostaRica.Volcano.Application.Dtos;

public record LocationDto
{
    public required string Province { get; set; }
    public required string Canton { get; set; }
    public string? District { get; set; }
}

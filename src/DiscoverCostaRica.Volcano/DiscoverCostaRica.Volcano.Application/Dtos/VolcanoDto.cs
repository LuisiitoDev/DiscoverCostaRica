namespace DiscoverCostaRica.VolcanoService.Application.Dtos;

public class VolcanoDto
{
    public int Id { get; set; }
    public required string Province { get; set; }
    public required string Canton { get; set; }
    public string? District { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}

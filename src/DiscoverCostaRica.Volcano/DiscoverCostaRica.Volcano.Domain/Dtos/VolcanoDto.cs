namespace DiscoverCostaRica.Volcano.Domain.Dtos;

public class VolcanoDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required LocationDto Location { get; set; }
}

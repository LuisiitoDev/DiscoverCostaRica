namespace DiscoverCostaRica.Volcano.Application.Dtos;

public record DtoVolcano
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required DtoLocation Location { get; set; }
}

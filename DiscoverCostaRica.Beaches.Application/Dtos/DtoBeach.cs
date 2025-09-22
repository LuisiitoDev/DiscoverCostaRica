namespace DiscoverCostaRica.Beaches.Application.Dtos;

public record DtoBeach
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}

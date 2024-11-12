namespace DiscoverCostaRica.Domain.Entities;

public class District
{
    public short Id { get; set; }
    public string Name { get; set; }
    public short CantonId { get; set; }
    public Canton Canton { get; set; }
}
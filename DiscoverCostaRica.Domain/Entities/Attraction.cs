namespace DiscoverCostaRica.Domain.Entities;
public class Attraction
{
    public int Id { get; set;}
    public string Name { get; set;}
    public short ProviceId { get; set; }
    public virtual Province Province { get; set; }
}
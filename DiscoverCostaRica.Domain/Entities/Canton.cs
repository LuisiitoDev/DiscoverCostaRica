namespace DiscoverCostaRica.Domain.Entities;

public class Canton
{
    public short Id { get; set; }
    public string Name { get; set; }
    public ICollection<District> Districts { get; set; }
    public virtual District District { get; set; }
}
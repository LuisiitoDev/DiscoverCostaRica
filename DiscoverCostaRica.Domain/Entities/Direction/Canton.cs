namespace DiscoverCostaRica.Domain.Entities.Direction;

public class Canton 
{
    public short Id { get; set; }
    public string Name { get; set; }
    public ICollection<District> Districts { get; set; }
}
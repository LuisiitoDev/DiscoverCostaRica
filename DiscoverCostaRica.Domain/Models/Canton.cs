namespace DiscoverCostaRica.Domain.Entities;

public class Canton
{
    public short Id { get; set; }
    public string Name { get; set; }
    public short ProvinceId { get; set; }
    public Province Province { get; set; }
    public ICollection<District> Districts { get; set; }
}
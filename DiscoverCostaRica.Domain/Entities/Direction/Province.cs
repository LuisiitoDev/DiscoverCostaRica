namespace DiscoverCostaRica.Domain.Entities.Direction;

public class Province {
    public short Id { get; set; }
    public string Name { get; set; }
    public ICollection<Canton> Cantons { get; set; }
    public ProvinceDetail ProvinceDetail { get; set; }
}
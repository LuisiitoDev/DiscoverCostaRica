namespace DiscoverCostaRica.Domain.Entities;

public class Province {
    public short Id { get; set; }
    public string Name { get; set; }
    public ICollection<Canton> Cantons { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; }
}
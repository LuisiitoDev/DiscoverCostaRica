namespace DiscoverCostaRica.Domain.Entities;

public class Province {
    public short Id { get; set; }
    public string Name { get; set; }
    public ICollection<Canton> Cantons { get; set; }
    public virtual Canton Canton { get; set; }
}
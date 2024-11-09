namespace DiscoverCostaRica.Domain.Entities.Direction;

public class ProvinceDetail
{
    public short Id { get; set; }
    public short ProvinceId { get; set; }
    public string History { get; set; }
    public string Map { get; set; }
    public virtual Province Province { get; set; }
}
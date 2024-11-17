namespace DiscoverCostaRica.Api.Models.Dto;

public class DtoCountry
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CountryCode { get; set; }
    public int Population { get; set; }
    public string Location { get; set; }
    public DtoProvince Capital { get; set; }
}

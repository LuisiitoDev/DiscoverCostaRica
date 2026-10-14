using DiscoverCostaRica.Geo.Domain;

namespace DiscoverCostaRica.Geo.Application.UseCases;

/// <summary>A province, canton or district as the use cases return it: the code within its parent, and the name.</summary>
public sealed record DivisionResponse(int Id, string Name)
{
    public static DivisionResponse From(Province province) => new(province.Code.Value, province.Name);

    public static DivisionResponse From(Canton canton) => new(canton.Code.Canton, canton.Name);

    public static DivisionResponse From(District district) => new(district.Code.District, district.Name);
}

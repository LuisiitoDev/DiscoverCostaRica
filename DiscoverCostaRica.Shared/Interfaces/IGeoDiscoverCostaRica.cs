using DiscoverCostaRica.Shared.Dtos;
using DiscoverCostaRica.Shared.Responses;
using Refit;

namespace DiscoverCostaRica.Shared.Interfaces;

public interface IGeoDiscoverCostaRica
{
    [Get("/provinces")]
    Task<Result<DtoProvince>> GetProvinces(CancellationToken cancellationToken);

    [Get($"/cantons/{{provinceId}}")]
    Task<Result<DtoProvince>> GetCantons(int provinceId, CancellationToken cancellationToken);

    [Get($"/districts/{{cantonId}}")]
    Task<Result<DtoProvince>> GetDistricts(int cantonId, CancellationToken cancellationToken);

    [Get($"/provinces/{{provinceId}}")]
    Task<Result<DtoProvince>> GetProvince(int provinceId, CancellationToken cancellationToken);

    [Get($"/cantons/{{provinceId}}/{{cantonId}}")]
    Task<Result<DtoCanton>> GetCanton(int provinceId, int cantonId, CancellationToken cancellationToken);

    [Get($"/districts/{{cantonId}}/{{districtId}}")]
    Task<Result<DtoDistrict>> GetDistrict(int cantonId, int districtId, CancellationToken cancellationToken);
}

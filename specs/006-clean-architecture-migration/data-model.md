# Data Model: Clean Architecture Migration

The model has three levels. **Entities** (Domain) hold the invariants. **Request/Response
models** (Application) cross the use-case boundary. **Ports** (Application) are implemented
by Infrastructure.

## 1. Building blocks (`BuildingBlocks.Application`, BCL only)

```csharp
public enum ErrorType { NotFound, Validation, Unavailable, Unexpected }

public sealed record Error(ErrorType Type, string Message)
{
    public static Error NotFound(string message) => new(ErrorType.NotFound, message);
    public static Error Validation(string message) => new(ErrorType.Validation, message);
    public static Error Unavailable(string message) => new(ErrorType.Unavailable, message);
}

public sealed class Result<T>          // a class, so default(Result<T>) cannot pose as a success
{
    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess => Error is null;   // [MemberNotNullWhen] on Value / Error
    // private ctors; Success / Failure factories; implicit operators from T and from Error; Match
}

public interface IUseCase<in TRequest, TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}
```

The presenter (`BuildingBlocks.Web`) maps outcomes to the v1 envelope:

| Outcome | HTTP | Body |
|---|---|---|
| Success | 200 | `{ "statusCode": 200, "value": … }` |
| `NotFound` | 404 | `{ "statusCode": 404, "message": "…" }` |
| `Validation` | 400 | `{ "statusCode": 400, "message": "…" }` |
| `Unavailable` | 503 | ProblemDetails |
| `Unexpected` | 500 | empty (as built) |

## 2. Geo (Entities)

| Entity / VO | Fields | Invariants |
|---|---|---|
| `ProvinceCode` (VO) | `int Value` | 1 ≤ value ≤ 7 |
| `CantonCode` (VO) | `ProvinceCode Province`, `int Canton` | canton > 0 |
| `DistrictCode` (VO) | `CantonCode Canton`, `int District` | district > 0 |
| `Province` | `ProvinceCode Code`, `string Name` | name not blank, ≤ 100 |
| `Canton` | `CantonCode Code`, `string Name` | belongs to exactly one province (encoded in the code) |
| `District` | `DistrictCode Code`, `string Name` | belongs to exactly one canton |

Persistence (Infrastructure only): tables `geo.Province (Id)`, `geo.Canton (ProvinceId, Id)`,
`geo.District (CantonProvinceId, CantonId, Id)`. The value objects are mapped as complex
types or keys. Navigation properties are **not** part of the entities.

**Ports**

```csharp
public interface IGeoReadRepository
{
    Task<IReadOnlyList<Province>> GetProvincesAsync(CancellationToken ct);
    Task<Province?> GetProvinceAsync(ProvinceCode code, CancellationToken ct);
    Task<IReadOnlyList<Canton>> GetCantonsAsync(ProvinceCode province, CancellationToken ct);
    Task<Canton?> GetCantonAsync(CantonCode code, CancellationToken ct);
    Task<IReadOnlyList<District>> GetDistrictsAsync(CantonCode canton, CancellationToken ct);
    Task<IReadOnlyList<District>> GetDistrictsByCantonIdOnlyAsync(int cantonId, CancellationToken ct); // deprecated route only
    Task<District?> GetDistrictAsync(DistrictCode code, CancellationToken ct);
    Task<IReadOnlyDictionary<LocationKey, ResolvedNames>> ResolveAsync(IReadOnlyCollection<LocationKey> keys, CancellationToken ct);
}
```

**Use cases**: `GetProvinces`, `GetProvinceById`, `GetCantonsByProvince`, `GetCantonById`,
`GetDistrictsByCanton`, `GetDistrictById`, `ResolveLocations`. Responses: `ProvinceResponse(Id, Name)`,
`CantonResponse(Id, Name)`, `DistrictResponse(Id, Name)`. The JSON shapes are identical to v1.

## 3. Volcano

| Entity / VO | Fields | Invariants |
|---|---|---|
| `LocationCode` (VO) | `int ProvinceId`, `int CantonId`, `int? DistrictId` | ids > 0 |
| `Volcano` | `int Id`, `string Name`, `string Description`, `LocationCode Location` | name and description not blank |

**Ports**: `IVolcanoReadRepository` (all, by id, by province) and `IGeoLocationGateway`
(`ResolveAsync(IReadOnlyCollection<LocationCode>) → IReadOnlyDictionary<LocationCode, LocationResponse?>`).
The gateway implementation (Infrastructure) uses Refit with `Contracts.Geo` and
stale-on-error caching.

**Response**: `VolcanoResponse(Id, Name, Description, LocationResponse? Location)` with
`LocationResponse(NamedRef Province, NamedRef Canton, NamedRef? District)` and `NamedRef(Id, Name)`.
This is identical to the v1 `DtoVolcano` / `DtoLocation` JSON.

## 4. Beaches

`Beach(int Id, string Name ≤ 1000, string Description ≤ 10000)`. Port: `IBeachReadRepository`.
Use case: `GetBeaches`. Cached with a 24 h TTL.

## 5. Culture

`Tradition(Id, Name, Description, ImageUrl?)` and `Dish(Id, Name, Description, Ingredients, Preparation, ImageUrl?)`.
Port: `ICultureReadRepository`. Use cases: `GetTraditions`, `GetDishes`.

## 6. Contracts.Geo (BCL only, versioned with Geo v1)

```csharp
public sealed record LocationCodeDto(int ProvinceId, int CantonId, int? DistrictId);
public sealed record NamedRefDto(int Id, string Name);
public sealed record ResolvedLocationDto(NamedRefDto? Province, NamedRefDto? Canton, NamedRefDto? District);
public sealed record ResolveLocationsRequest(IReadOnlyList<LocationCodeDto> Items);   // ≤ 200
public sealed record ResolveLocationsResponse(IReadOnlyList<ResolvedLocationDto> Items); // same order; unknown levels are null
```

## 7. Cache keys and TTL

| Key | Owner | TTL | Tags |
|---|---|---|---|
| `geo:provinces` | Geo | 24 h | `geo` |
| `beach:beaches` | Beaches | 24 h | `beach` |
| `culture:dishes` / `culture:traditions` | Culture | 24 h | `culture` |
| `volcano:volcanoes` | Volcano | 1 h | `volcano` |
| `volcano:location:{p}:{c}:{d?}` | Volcano | 24 h, stale-on-error | `geo-names` |

Key names change from the as-built `Geo.Provinces` etc. The old keys simply expire unused,
so no migration is needed.

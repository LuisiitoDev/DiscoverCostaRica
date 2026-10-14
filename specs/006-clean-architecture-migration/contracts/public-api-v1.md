# Contract: Public API v1 (frozen)

Reverse-engineered from the as-built services. **Status**: frozen. Changes are limited to
the fixes marked 🔧 and the additive routes marked ➕ (FR-004).

## Conventions

- Base path: `/api/v{version}/{group}`. Only `v1` is mapped. Responses carry the
  `api-supported-versions: 1.0, 2.0` header.
- Auth: `Authorization: Bearer <Entra token>` with the app role listed per group. No token
  → 401. Wrong role → 403.
- JSON: camelCase. Null properties are omitted. Enums are serialized as strings.
- Envelope:

```json
{ "statusCode": 200, "value": <payload> }        // success
{ "statusCode": 404, "message": "<text>" }        // not found
{ "statusCode": 400, "message": "<text>" }        // validation
```

- A 500 from the presenter has an empty body. An unhandled exception gives 500
  ProblemDetails: `{ "title": "An unexpected error occurred", "status": 500, "detail": "…" }`
  (🔧 typo "ocurred" fixed in the title).

## Geo — role `Geo.Read`

| Method | Route | 200 `value` | 404 message |
|---|---|---|---|
| GET | `/api/v1/geo/provinces` | `[{id,name}]` | No provinces found. Please check later for updates. |
| GET | `/api/v1/geo/provinces/{provinceId}` | `{id,name}` | No provice was found for {id} (as built; 🔧GEO-D4 → "province") |
| GET | `/api/v1/geo/cantons/{provinceId}` | `[{id,name}]` | No cantons found. Please check later for updates. |
| GET | `/api/v1/geo/cantons/{provinceId}/{cantonId}` | `{id,name}` | No canton was found for {cantonId} |
| GET | `/api/v1/geo/districts/{cantonId}[?provinceId=]` ⚠️deprecated | `[{id,name}]` | No districts found. Please check later for updates. |
| GET | `/api/v1/geo/districts/{cantonId}/{districtId}[?provinceId=]` ⚠️deprecated | `{id,name}` | No district was found for {districtId} |
| GET | ➕ `/api/v1/geo/provinces/{provinceId}/cantons` | `[{id,name}]` | same as cantons |
| GET | ➕ `/api/v1/geo/provinces/{provinceId}/cantons/{cantonId}` | `{id,name}` | same |
| GET | ➕ `/api/v1/geo/provinces/{provinceId}/cantons/{cantonId}/districts` 🔧GEO-D2 | `[{id,name}]` | same |
| GET | ➕ `/api/v1/geo/provinces/{provinceId}/cantons/{cantonId}/districts/{districtId}` 🔧GEO-D3 | `{id,name}` | same |
| POST | ➕ `/api/v1/geo/locations:resolve` (internal) | see `geo-locations-resolve.md` | — |

Deprecated routes respond with `Deprecation: true` and
`Link: </api/v1/geo/provinces/{p}/cantons/{c}/districts>; rel="successor-version"`.

## Beaches — role `Beaches.Read`

| Method | Route | 200 `value` | 404 message |
|---|---|---|---|
| GET | `/api/v1/beaches/` | `[{id,name,description}]` | No beaches found. Please check later for updates. |

## Culture — role `Culture.Read`

| Method | Route | 200 `value` | 404 message |
|---|---|---|---|
| GET | `/api/v1/traditions/tradition` | `[{id,name,description,imageUrl}]` | No traditions found. Please check later for updates. |
| GET | `/api/v1/traditions/dish` | `[{id,name,description,ingredients,preparation,imageUrl}]` | No dishes found. Please check later for updates. |

## Volcano — role `Volcano.Read`

Shape `Volcano`:

```json
{ "id": 1, "name": "Arenal", "description": "…",
  "location": { "province": {"id":2,"name":"Alajuela"},
                "canton":   {"id":10,"name":"San Carlos"},
                "district": {"id":7,"name":"La Fortuna"} } }
```

| Method | Route | 200 `value` | 404 message |
|---|---|---|---|
| GET 🔧VOL-D2 | `/api/v1/volcanoes/` | `[Volcano]` | Volcanos were not found |
| GET 🔧VOL-D1/D5 | `/api/v1/volcanoes/{id}` | `Volcano` | Volcano was not found |
| GET 🔧VOL-D1/D5 | `/api/v1/volcanoes/province/{provinceId}` | `[Volcano]` | Volcanos were not found |

`location` is omitted when Geo was never reachable for that code (spec 006 US3 #5).
`location.district` is omitted when the volcano has no district.

## Gateway (public) 🔧BCH-D1, VOL-D7, PLT-D1

| Public path | Upstream |
|---|---|
| `/provinces`, `/provinces/{**}` | Geo `/api/v1/geo/provinces/{**}` |
| `/cantons/{**}`, legacy `/canton/{**}` | Geo `/api/v1/geo/cantons/{**}` |
| `/districts/{**}` | Geo `/api/v1/geo/districts/{**}` |
| `/beaches` | Beaches `/api/v1/beaches/` |
| `/tradition`, `/dish` | Culture `/api/v1/traditions/{tradition\|dish}` |
| `/volcanoes`, `/volcanoes/{**}`, legacy `/volcano/{**}` | Volcano `/api/v1/volcanoes/{**}` |
| `/province/{id}` (legacy) | Volcano `/api/v1/volcanoes/province/{id}` |
| anything else | **404** at the gateway |

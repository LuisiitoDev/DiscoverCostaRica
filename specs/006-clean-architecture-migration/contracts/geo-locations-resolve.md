# Contract: Geo — Resolve Locations (internal, v1)

**Consumer**: Volcano (and future Beaches location linking)  **Provider**: Geo

**Package**: `DiscoverCostaRica.Contracts.Geo`  **Auth**: app role `Geo.Read`
(client credentials)

## Request

`POST /api/v1/geo/locations:resolve`

```json
{ "items": [
    { "provinceId": 2, "cantonId": 10, "districtId": 7 },
    { "provinceId": 5, "cantonId": 1 }
] }
```

- 1 ≤ `items.length` ≤ 200. Otherwise 400 `{ "statusCode": 400, "message": "items must contain between 1 and 200 entries" }`.
- Duplicate items are allowed, and each one is resolved.

## Response 200

Same order and same length as the request. Unknown levels are `null` (omitted in JSON).
An item whose province is unknown resolves to `{}`.

```json
{ "statusCode": 200,
  "value": { "items": [
    { "province": {"id":2,"name":"Alajuela"}, "canton": {"id":10,"name":"San Carlos"}, "district": {"id":7,"name":"La Fortuna"} },
    { "province": {"id":5,"name":"Guanacaste"}, "canton": {"id":1,"name":"Liberia"} }
  ] } }
```

## Non-functional

- One SQL round trip per request (a set-based query with a table-valued parameter or `IN` on
  composite keys).
- p95 < 100 ms for 200 items.
- Idempotent and side-effect free. POST is used only for the body size.

## Consumer guarantees (Volcano)

- Call once per uncached volcano query, with deduplicated codes.
- Cache each resolved code for 24 h. On a Geo failure (timeout, 5xx, circuit open), serve the
  stale entry and otherwise omit `location`.

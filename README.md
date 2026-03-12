# Cerbi.Signatures.Api

Developer helper methods for API request events in the Cerbi governance ecosystem.

## Purpose

Provides strongly-typed signature methods that produce `GovernedEvent` objects for API request/response scenarios. These events are **not** written to any logger directly — they are structured event objects ready for consumption by any logging adapter.

## Signatures

| Method | Event Name | Description |
|--------|-----------|-------------|
| `ApiEvents.RequestStarted(...)` | `Api.RequestStarted` | API request started |
| `ApiEvents.RequestCompleted(...)` | `Api.RequestCompleted` | API request completed |
| `ApiEvents.RequestFailed(...)` | `Api.RequestFailed` | API request failed |

## Usage

```csharp
using Cerbi.Signatures.Api;

var evt = ApiEvents.RequestStarted(
    method: "GET",
    route: "/api/users",
    requestId: "req-123",
    tenantId: "tenant-abc",
    correlationId: "corr-456"
);

// evt is a GovernedEvent — pass it to your logger adapter
```

## Dependencies

- [Cerbi.Governance.Schemas](https://github.com/Zeroshi/Cerbi.Governance.Schemas)

## Build

```bash
dotnet build
dotnet test
```

## License

MIT

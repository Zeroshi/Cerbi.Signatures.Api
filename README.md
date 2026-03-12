# Cerbi.Signatures.Api

[![NuGet](https://img.shields.io/nuget/v/Cerbi.Signatures.Api)](https://www.nuget.org/packages/Cerbi.Signatures.Api)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Typed event factories for API lifecycle governed log events.** Provides strongly-typed methods for creating `GovernedEvent` instances for HTTP request tracking — request started, completed, and failed.

## Why This Package?

API observability is critical for performance monitoring, debugging, and compliance. `Cerbi.Signatures.Api` ensures every API lifecycle event has **consistent field names**, **compile-time type safety**, and **automatic governance validation**. Track request flows from start to finish with fields that map directly to CerbiShield governance profiles.

## Installation

```shell
dotnet add package Cerbi.Signatures.Api
```

This package depends on [`Cerbi.Governance.Schemas`](https://github.com/Zeroshi/Cerbi.Governance.Schemas), which is installed automatically as a transitive dependency.

## API Reference

All methods are on the static `ApiEvents` class and return a `GovernedEvent` with `Category = "Api"`.

### `ApiEvents.RequestStarted`

Records the start of an API request.

```csharp
using Cerbi.Signatures.Api;

var evt = ApiEvents.RequestStarted(
    method: "POST",
    route: "/api/orders",
    requestId: "req-abc-123",
    tenantId: "tenant-abc",
    correlationId: "corr-200");
```

**Properties dictionary:**
| Key | Value | Description |
|---|---|---|
| `method` | `"POST"` | HTTP method |
| `route` | `"/api/orders"` | API route or path |
| `requestId` | `"req-abc-123"` | Unique request identifier |
| `tenantId` | `"tenant-abc"` | Tenant context |
| `correlationId` | `"corr-200"` | Distributed trace ID |

### `ApiEvents.RequestCompleted`

Records successful completion of an API request.

```csharp
var evt = ApiEvents.RequestCompleted(
    method: "POST",
    route: "/api/orders",
    requestId: "req-abc-123",
    tenantId: "tenant-abc",
    correlationId: "corr-200",
    statusCode: 201,
    durationMs: 142.5);
```

**Properties dictionary:**
| Key | Value | Description |
|---|---|---|
| `method` | `"POST"` | HTTP method |
| `route` | `"/api/orders"` | API route or path |
| `requestId` | `"req-abc-123"` | Unique request identifier |
| `tenantId` | `"tenant-abc"` | Tenant context |
| `correlationId` | `"corr-200"` | Distributed trace ID |
| `statusCode` | `201` | HTTP response status code |
| `durationMs` | `142.5` | Request duration in milliseconds |

### `ApiEvents.RequestFailed`

Records a failed API request.

```csharp
var evt = ApiEvents.RequestFailed(
    method: "POST",
    route: "/api/orders",
    requestId: "req-abc-123",
    tenantId: "tenant-abc",
    correlationId: "corr-200",
    statusCode: 500,
    reason: "Database connection timeout");
```

**Properties dictionary:**
| Key | Value | Description |
|---|---|---|
| `method` | `"POST"` | HTTP method |
| `route` | `"/api/orders"` | API route or path |
| `requestId` | `"req-abc-123"` | Unique request identifier |
| `tenantId` | `"tenant-abc"` | Tenant context |
| `correlationId` | `"corr-200"` | Distributed trace ID |
| `statusCode` | `500` | HTTP response status code |
| `reason` | `"Database connection timeout"` | Failure reason |

## Full Request Lifecycle Example

Use all three methods together to track a complete API request:

```csharp
using Cerbi.Signatures.Api;
using System.Diagnostics;

public async Task<IActionResult> CreateOrder(OrderRequest request)
{
    var requestId = Guid.NewGuid().ToString();
    var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
    var sw = Stopwatch.StartNew();

    // Log request start
    logger.LogGovernedEvent(ApiEvents.RequestStarted(
        "POST", "/api/orders", requestId, tenantId, correlationId));

    try
    {
        var order = await _orderService.CreateAsync(request);
        sw.Stop();

        // Log successful completion
        logger.LogGovernedEvent(ApiEvents.RequestCompleted(
            "POST", "/api/orders", requestId, tenantId, correlationId,
            statusCode: 201, durationMs: sw.Elapsed.TotalMilliseconds));

        return Created($"/api/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        sw.Stop();

        // Log failure
        logger.LogGovernedEvent(ApiEvents.RequestFailed(
            "POST", "/api/orders", requestId, tenantId, correlationId,
            statusCode: 500, reason: ex.Message));

        throw;
    }
}
```

## Supported Loggers

`ApiEvents` produces `GovernedEvent` objects compatible with **every** Cerbi logger governance plugin:

| Logger | Cerbi Plugin | Extraction Method |
|---|---|---|
| **CerbiStream** | [`Cerbi-CerbiStream`](https://github.com/Zeroshi/Cerbi-CerbiStream) | Native — `GovernedEvent` is the primary type |
| **Serilog** | [`Cerbi.Serilog.GovernanceAnalyzer`](https://github.com/Zeroshi/Cerbi.Serilog.GovernanceAnalyzer) | `SerilogEventAdapter.ToDictionary()` extracts `LogEvent.Properties` |
| **MEL** | [`Cerbi.MEL.Governance`](https://github.com/Zeroshi/Cerbi.MEL.Governance) | `CerbiGovernanceLogger.ExtractFields<TState>()` extracts key-value pairs |
| **NLog** | [`Cerbi.NLog.GovernanceAnalyzer`](https://github.com/Zeroshi/Cerbi.NLog.GovernanceAnalyzer) | `GovernanceConfigLoader` maps event properties |

### Example: Using with CerbiStream

```csharp
using CerbiStream;
using Cerbi.Signatures.Api;

var evt = ApiEvents.RequestStarted("GET", "/api/users", "req-1", "tenant-1", "corr-1");
logger.LogGovernedEvent(evt);
```

### Example: Using with Serilog

```csharp
using Serilog;
using Cerbi.Signatures.Api;

var evt = ApiEvents.RequestCompleted("GET", "/api/users", "req-1", "tenant-1", "corr-1", 200, 45.2);
Log.ForContext("GovernedEvent", evt, destructureObjects: true)
   .Information("Request completed: {Method} {Route} {StatusCode} in {Duration}ms",
       "GET", "/api/users", 200, 45.2);
```

### Example: Using with MEL

```csharp
using Microsoft.Extensions.Logging;
using Cerbi.Signatures.Api;

var evt = ApiEvents.RequestFailed("POST", "/api/payments", "req-2", "tenant-1", "corr-2", 503, "Timeout");
logger.LogError("Request failed: {@ApiEvent}", evt.Properties);
```

## CerbiShield Dashboard Integration

API lifecycle events provide rich operational and compliance data on the CerbiShield Dashboard:

```
ApiEvents.RequestStarted()     ──▶  GovernedEvent  ──▶  Logger Plugin
ApiEvents.RequestCompleted()   ──▶  GovernedEvent  ──▶  Logger Plugin
ApiEvents.RequestFailed()      ──▶  GovernedEvent  ──▶  Logger Plugin
                                                          │
                                                    Validate + Score
                                                          │
                                                    ScoreShipper
                                                          │
                                                Azure Service Bus
                                                          │
                                               ScoringApi → Aggregator
                                                          │
                                       ┌──────────────────▼──────────────────┐
                                       │      CerbiShield Dashboard          │
                                       │                                     │
                                       │  🌐 API request volume & trends    │
                                       │  ⚡ Response time tracking         │
                                       │  ❌ Error rate by route/method     │
                                       │  📊 Status code distribution       │
                                       │  🎯 Missing field violations       │
                                       │  📈 Governance compliance trends   │
                                       └─────────────────────────────────────┘
```

### Dashboard Views for API Events

| Dashboard Feature | What You See |
|---|---|
| **Overview → Events Today** | API event volume contributing to total governed events |
| **Overview → Governance Score** | API events scored for field completeness and compliance |
| **Violations → Top Rules** | API rules ranked by violation count (e.g., `API-001: Missing requestId`) |
| **Violations → Trend** | API compliance trend over time |
| **Reporting → By App** | Per-application API governance breakdown |
| **Health → Scoring Throughput** | API event processing rate |

### Governance Profile Example

```json
{
  "name": "api-logging-standards",
  "appName": "my-api",
  "version": "1.0.0",
  "requiredFields": ["method", "route", "requestId", "tenantId", "correlationId"],
  "disallowedFields": ["requestBody", "responseBody", "authToken"],
  "fieldSeverities": {
    "method": "Error",
    "route": "Error",
    "requestId": "Error",
    "tenantId": "Error",
    "correlationId": "Warn",
    "statusCode": "Warn",
    "durationMs": "Info"
  },
  "fieldTypes": {
    "method": "String",
    "route": "String",
    "requestId": "String",
    "statusCode": "Int",
    "durationMs": "Decimal"
  }
}
```

## Related Packages

| Package | Purpose |
|---|---|
| [`Cerbi.Governance.Schemas`](https://github.com/Zeroshi/Cerbi.Governance.Schemas) | Core types — `GovernedEvent`, `GovernedEventBuilder`, profile definitions |
| [`Cerbi.Signatures.Security`](https://github.com/Zeroshi/Cerbi.Signatures.Security) | Typed factories for security events (login failures, permission denied) |
| [`Cerbi.Signatures.Audit`](https://github.com/Zeroshi/Cerbi.Signatures.Audit) | Typed factories for audit events (entity CRUD) |

## License

MIT

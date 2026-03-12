using Xunit;
using Cerbi.Signatures.Api;

namespace Cerbi.Signatures.Api.Tests;

public class ApiEventsTests
{
    [Fact]
    public void RequestStarted_SetsEventName()
    {
        var evt = ApiEvents.RequestStarted("GET", "/api/users", "req-1", "tenant-1", "corr-1");

        Assert.Equal("Api.RequestStarted", evt.EventName);
    }

    [Fact]
    public void RequestStarted_SetsCategoryToApi()
    {
        var evt = ApiEvents.RequestStarted("GET", "/api/users", "req-1", "tenant-1", "corr-1");

        Assert.Equal("Api", evt.Category);
    }

    [Fact]
    public void RequestStarted_SetsMessage()
    {
        var evt = ApiEvents.RequestStarted("GET", "/api/users", "req-1", "tenant-1", "corr-1");

        Assert.Equal("GET /api/users request started", evt.Message);
    }

    [Fact]
    public void RequestStarted_ContainsAllRequiredProperties()
    {
        var evt = ApiEvents.RequestStarted("GET", "/api/users", "req-1", "tenant-1", "corr-1");

        Assert.Equal("GET", evt.Properties["method"]);
        Assert.Equal("/api/users", evt.Properties["route"]);
        Assert.Equal("req-1", evt.Properties["requestId"]);
        Assert.Equal("tenant-1", evt.Properties["tenantId"]);
        Assert.Equal("corr-1", evt.Properties["correlationId"]);
    }

    [Fact]
    public void RequestCompleted_SetsEventName()
    {
        var evt = ApiEvents.RequestCompleted("POST", "/api/orders", 201, 150, "tenant-1", "corr-2");

        Assert.Equal("Api.RequestCompleted", evt.EventName);
    }

    [Fact]
    public void RequestCompleted_ContainsStatusCodeAndDuration()
    {
        var evt = ApiEvents.RequestCompleted("POST", "/api/orders", 201, 150, "tenant-1", "corr-2");

        Assert.Equal(201, evt.Properties["statusCode"]);
        Assert.Equal(150L, evt.Properties["durationMs"]);
    }

    [Fact]
    public void RequestFailed_SetsEventName()
    {
        var evt = ApiEvents.RequestFailed("DELETE", "/api/users/1", 500, "internal error", "tenant-1", "corr-3");

        Assert.Equal("Api.RequestFailed", evt.EventName);
    }

    [Fact]
    public void RequestFailed_ContainsAllRequiredProperties()
    {
        var evt = ApiEvents.RequestFailed("DELETE", "/api/users/1", 500, "internal error", "tenant-1", "corr-3");

        Assert.Equal("DELETE", evt.Properties["method"]);
        Assert.Equal("/api/users/1", evt.Properties["route"]);
        Assert.Equal(500, evt.Properties["statusCode"]);
        Assert.Equal("internal error", evt.Properties["reason"]);
        Assert.Equal("tenant-1", evt.Properties["tenantId"]);
        Assert.Equal("corr-3", evt.Properties["correlationId"]);
    }
}

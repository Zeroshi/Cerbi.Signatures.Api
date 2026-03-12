using Cerbi.Governance.Schemas.Events;

namespace Cerbi.Signatures.Api;

public static class ApiEvents
{
    public static GovernedEvent RequestStarted(
        string method,
        string route,
        string requestId,
        string tenantId,
        string correlationId)
    {
        return GovernedEventBuilder.Create(
            eventName: "Api.RequestStarted",
            category: "Api",
            message: $"{method} {route} request started",
            properties: new Dictionary<string, object?>
            {
                ["method"] = method,
                ["route"] = route,
                ["requestId"] = requestId,
                ["tenantId"] = tenantId,
                ["correlationId"] = correlationId
            });
    }

    public static GovernedEvent RequestCompleted(
        string method,
        string route,
        int statusCode,
        long durationMs,
        string tenantId,
        string correlationId)
    {
        return GovernedEventBuilder.Create(
            eventName: "Api.RequestCompleted",
            category: "Api",
            message: $"{method} {route} completed with {statusCode}",
            properties: new Dictionary<string, object?>
            {
                ["method"] = method,
                ["route"] = route,
                ["statusCode"] = statusCode,
                ["durationMs"] = durationMs,
                ["tenantId"] = tenantId,
                ["correlationId"] = correlationId
            });
    }

    public static GovernedEvent RequestFailed(
        string method,
        string route,
        int statusCode,
        string reason,
        string tenantId,
        string correlationId)
    {
        return GovernedEventBuilder.Create(
            eventName: "Api.RequestFailed",
            category: "Api",
            message: $"{method} {route} failed with {statusCode}",
            properties: new Dictionary<string, object?>
            {
                ["method"] = method,
                ["route"] = route,
                ["statusCode"] = statusCode,
                ["reason"] = reason,
                ["tenantId"] = tenantId,
                ["correlationId"] = correlationId
            });
    }
}

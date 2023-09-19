using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace PlatformPlatform.SharedKernel.ApiCore.Endpoints;

public static class TestEndpoints
{
    public static void MapTestEndpoints(this IEndpointRouteBuilder routes)
    {
        if (!bool.TryParse(Environment.GetEnvironmentVariable("TestEndpointsEnabled"), out _)) return;

        // Add a dummy endpoint that throws an InvalidOperationException for testing purposes.
        routes.MapGet("/throwException", _ => throw new InvalidOperationException("Dummy endpoint for testing."));
    }
}
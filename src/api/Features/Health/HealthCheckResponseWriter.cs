using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AGRC.api.Features.Health;
internal static class DetailedHealthCheckResponseWriter {
    public static Task WriteDetailsJson(HttpContext httpContext, HealthReport result) {
        httpContext.Response.ContentType = "application/json";

        var response = new {
            status = result.Status.ToString(),
            checks = result.Entries.Select(x => new {
                check = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description
            })
        };

        return JsonSerializer.SerializeAsync(httpContext.Response.Body, response, new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
    }
}
internal static class StartupHealthCheckResponseWriter {
    public static async Task WriteText(HttpContext httpContext, HealthReport _) {
        httpContext.Response.ContentType = "text/plain";

        await httpContext.Response.Body.WriteAsync(Encoding.ASCII.GetBytes("ok"));
    }
}

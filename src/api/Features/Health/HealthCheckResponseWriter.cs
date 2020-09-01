using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AGRC.api.Features.Health {
    internal static class HealthCheckResponseWriter {
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
}

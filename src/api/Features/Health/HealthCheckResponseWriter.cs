using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

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

            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
    }
}

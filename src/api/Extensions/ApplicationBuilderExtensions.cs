using api.mapserv.utah.gov.Features.Health;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace api.mapserv.utah.gov.Extensions {
    public static class ApplicationBuilderExtensions {
        public static void UseSwagger(this IApplicationBuilder app) {
            app.UseSwagger(c => { c.RouteTemplate = "openapi/{documentName}/api.json"; });

            app.UseSwaggerUI(c => {
                c.DocumentTitle = "AGRC WebAPI OpenAPI Documentation";
                c.RoutePrefix = "openapi";
                c.SwaggerEndpoint("/openapi/v1/api.json", "v1");
                c.SupportedSubmitMethods();
                c.EnableDeepLinking();
                c.DocExpansion(DocExpansion.List);
            });
        }

        public static void UseHealthChecks(this IApplicationBuilder app) {
            app.UseHealthChecks("/health/details", new HealthCheckOptions {
                ResponseWriter = HealthCheckResponseWriter.WriteDetailsJson
            });
            app.UseHealthChecks("/health");
        }
    }
}

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace api.OpenApi {
    public class TrimUrlOperationFilter : IDocumentFilter {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
            var paths = new OpenApiPaths();
            foreach (var item in swaggerDoc.Paths) {
                var parameters = new List<OpenApiParameter>();
                foreach(var parameter in item.Value.Operations[0].Parameters) {
                    if (parameter.Name == "version") {
                        continue;
                    }

                    parameters.Add(parameter);
                }

                item.Value.Operations[0].Parameters = parameters;

                paths.Add(item.Key.Remove(0, 15), item.Value);
            }

            swaggerDoc.Paths = paths;
        }
    }
}

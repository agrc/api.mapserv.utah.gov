using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace api.tests.Helpers;
public static class HttpContextHelpers {
    public static HttpContext CreateVersionedHttpContext(int version) {
        var apiVersion = new ApiVersion(version, 0);
        var httpContext = new DefaultHttpContext();

        httpContext.Features.Set<IApiVersioningFeature>(new ApiVersioningFeature(httpContext) {
            RequestedApiVersion = apiVersion
        });

        return httpContext;
    }
}

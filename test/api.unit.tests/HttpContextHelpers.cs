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
    public static HttpContext GenerateContextFor(string query, int version) {
        var context = CreateVersionedHttpContext(version);
        context.Request.QueryString = new QueryString(query);

        return context;
    }

    public static EndpointFilterInvocationContext GetEndpointContext(params object[] arguments) =>
      new DefaultEndpointFilterInvocationContext(new DefaultHttpContext(), arguments);
}

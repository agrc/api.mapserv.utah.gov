namespace ugrc.api.Middleware;

public class RequestLoggerMiddleware(RequestDelegate next, ILogger log, IBrowserKeyProvider browserProvider) {
    private readonly RequestDelegate _next = next;
    private readonly ILogger? _log = log?.ForContext<RequestLoggerMiddleware>();
    private readonly IBrowserKeyProvider _apiKeyProvider = browserProvider;

    public async Task InvokeAsync(HttpContext context) {
        var key = _apiKeyProvider.Get(context.Request);
        await _next(context);

        _log?.ForContext("key", key)
            .ForContext("endpoint", ParseEndpoint(context.Request.Path))
            .ForContext("result", context.Response.StatusCode)
            .Information("Analytics:request");
    }

    private static string ParseEndpoint(string? path) {
        if (string.IsNullOrEmpty(path)) {
            return "-";
        }

        if (path[7] != '/') {
            return "-";
        }

        path = path.ToLowerInvariant();

        try {
            var pathSegments = path.Split('/');
            var primary = pathSegments[3];

            var simple = primary switch {
                "search" => "search",
                "health" => "health",
                _ => null,
            };

            if (!string.IsNullOrEmpty(simple)) {
                return simple;
            }

            var secondary = string.Empty;

            if (pathSegments.Length > 4) {
                secondary = pathSegments[4];
            }

            return primary switch {
                "geocode" => secondary switch {
                    "reverse" => "reverse geocode",
                    "reversemilepost" => "reverse milepost",
                    "milepost" => "milepost",
                    _ => "geocode",
                },
                "info" => secondary switch {
                    "featureclassnames" => "table info",
                    "fieldnames" => "field info",
                    _ => "info",
                },
                _ => "unknown",
            };
        } catch {
            return "error";
        }
    }
}

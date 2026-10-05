using System.Diagnostics;

namespace ugrc.api.Middleware;

public class RequestLoggerMiddleware(RequestDelegate next, ILogger log, IBrowserKeyProvider browserProvider) {
    private readonly RequestDelegate _next = next;
    private readonly ILogger? _log = log?.ForContext<RequestLoggerMiddleware>();
    private readonly IBrowserKeyProvider _apiKeyProvider = browserProvider;

    public async Task InvokeAsync(HttpContext context) {
        var key = _apiKeyProvider.Get(context.Request);
        var start = Stopwatch.GetTimestamp();
        await _next(context);
        var duration = Stopwatch.GetElapsedTime(start);

        _log?.ForContext("key", key)
            .ForContext("endpoint", ParseEndpoint(context.Request.Path))
            .ForContext("version", ParseVersion(context.Request.Path))
            .ForContext("result", context.Response.StatusCode)
            .ForContext("duration", Math.Round(duration.TotalMilliseconds))
            .ForContext("referer", ParseReferer(context.Request.Headers.Referer.ToString()))
            .ForContext("origin", NullIfEmpty(context.Request.Headers.Origin.ToString()))
            .ForContext("userAgent", NullIfEmpty(context.Request.Headers.UserAgent.ToString()))
            .Information("Analytics:request");
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrEmpty(value) ? null : value;

    private static string? ParseVersion(string? path) {
        if (string.IsNullOrEmpty(path)) {
            return null;
        }

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length < 2 || !segments[0].Equals("api", StringComparison.OrdinalIgnoreCase)) {
            return null;
        }

        var version = segments[1].ToLowerInvariant();

        return version.StartsWith('v') ? version : null;
    }

    private static string? ParseReferer(string? referer) {
        if (string.IsNullOrEmpty(referer)) {
            return null;
        }

        // drop the query string and fragment since they can contain tokens or personal information
        if (Uri.TryCreate(referer, UriKind.Absolute, out var uri)) {
            return uri.GetLeftPart(UriPartial.Path);
        }

        return referer;
    }

    private static string ParseEndpoint(string? path) {
        if (string.IsNullOrEmpty(path) || path.Length <= 1) {
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

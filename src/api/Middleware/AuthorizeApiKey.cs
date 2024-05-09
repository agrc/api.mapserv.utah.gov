using StackExchange.Redis;
using ugrc.api.Models.ResponseContracts;
using ugrc.api.Services;

namespace ugrc.api.Middleware;

public class AuthorizeApiKeyFilter(ILogger log, IBrowserKeyProvider browserProvider, IServerIpProvider serverIpProvider, IApiKeyRepository repo, Lazy<IConnectionMultiplexer> redis) : IEndpointFilter {
    private readonly ILogger? _log = log?.ForContext<AuthorizeApiKeyFilter>();
    private readonly IBrowserKeyProvider _apiKeyProvider = browserProvider;
    private readonly IServerIpProvider _serverIpProvider = serverIpProvider;
    private readonly IApiKeyRepository _repo = repo;
    private readonly IDatabase _db = redis.Value.GetDatabase();

    public virtual async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next) {
        var key = _apiKeyProvider.Get(context.HttpContext.Request);

        // key hasn't been created
        if (string.IsNullOrWhiteSpace(key)) {
            _log?.Debug("API key missing from request");

            return BadRequest("Your API key is missing from your request. " +
                "Add an `apikey={key}` to the request as a query string parameter."
            );
        }

        var apiKey = await _repo.GetKey(key);
        try {
            await _db.StringIncrementAsync("analytics:key-hit:" + key, flags: CommandFlags.FireAndForget);
        } catch { }

        // key hasn't been created
        if (apiKey == null) {
            _log?.ForContext("query", context.HttpContext.Request.Query)
                .Information("Unknown api key usage attempt for {key}", key);

            return BadRequest("Your API key does match the pattern created in the self service website. " +
                $"Check the referrer header on the request with the pattern for the api key `{key}`"
            );
        }

        _log?.Debug("Request with key: {apiKey}", apiKey.Key);

        if (apiKey.Flags["deleted"] || apiKey.Flags["disabled"]) {
            _log?.Information("Attempt to use deleted or disabled key {key}", apiKey);

            return BadRequest($"{key} is no longer active. It has been disabled or deleted by it's owner.");
        }

        if (apiKey.Elevated) {
            _log?.ForContext("headers", context.HttpContext.Request.Headers)
                .Information("Unrestricted key use {key} from {ip}", apiKey.Key, context.HttpContext.Request.Host);

            return await next(context);
        }

        if (apiKey.Flags["server"] == false) {
            if (apiKey.RegularExpression == null) {
                _log?.Warning("Key usage without regex pattern {key}", apiKey);

                return BadRequest("This api key has no regex pattern. This is likely a bug. " +
                    "Please contact the api owner to resolve this issue."
                );
            }

            var pattern = new Regex(apiKey.RegularExpression, RegexOptions.IgnoreCase);

            if (!context.HttpContext.Request.Headers.TryGetValue("Referrer", out var referrer)) {
                _log?.Debug("Missing referrer header");
                context.HttpContext.Request.Headers.TryGetValue("Referer", out referrer);
                _log?.Debug("Using common misspelling: {referrer}", referrer);
            }

            var hasOrigin = context.HttpContext.Request.Headers.Where(x => x.Key == "Origin").ToList();
            if (string.IsNullOrEmpty(referrer.ToString()) && hasOrigin.Count == 0) {
                _log?.Information("Browser key without referrer: {key}", apiKey);

                return BadRequest(
                    "The http referrer header is missing. Turn off any security solutions that may remove this " +
                    "header to use this service. If you are trying to test your query add the referrer header via a tool like postman " +
                    "or browse to api.mapserv.utah.gov and use the api explorer."
                );
            }

            var corsOriginHeader = hasOrigin.FirstOrDefault();
            var corsOriginValue = string.Empty;

            if (corsOriginHeader.Key != null) {
                corsOriginValue = corsOriginHeader.Value.SingleOrDefault() ?? string.Empty;
            }

            _log?.Debug("Request origin header: {origin}", corsOriginValue);

            try {
                var uri = new Uri(referrer.ToString());
                _log?.Debug("Referrer uri: {@uri}", uri);
            } catch {
                _log?.Information("Invalid referrer uri {referrer}", referrer);

                return BadRequest(
                    "The http referrer header is invalid. Turn off any security solutions that may remove this " +
                    "header to use this service. If you are trying to test your query add the referrer header via a tool like postman " +
                    "or browse to api.mapserv.utah.gov and use the api explorer."
                );
            }

            if (apiKey.Flags["production"] == false &&
                IsLocalDevelopment(new Uri(referrer.ToString()), corsOriginValue)) {
                return await next(context);
            }

            if (!ApiKeyPatternMatches(pattern, corsOriginValue, new Uri(referrer.ToString()))) {
                return BadRequest(
                    "Your API key does match the pattern created in the self service website. " +
                    $"Check the referrer header on the request with the pattern for the api key `{key}`"
                );
            }
        } else {
            var ip = apiKey.Pattern;
            var userHostAddress = _serverIpProvider.Get(context.HttpContext.Request);

            if (ip != userHostAddress) {
                _log?.Information("Invalid api key pattern match {ip} != {host} for {key}", ip, userHostAddress,
                                 apiKey);
                log?.Information("Http context connection info: {@request}", context.HttpContext.Connection);
                log?.Information("Request http context connection info: {@request}", context.HttpContext.Request.HttpContext.Connection);
                log?.Information("Request headers: {@headers}", context.HttpContext.Request.Headers);

                return BadRequest(
                    $"Your API key does match the pattern created in the self service website for key `{key}`. " +
                    $"The request is originating from `{userHostAddress}`"
                );
            }
        }

        return await next(context);
    }
    private static ApiResponseContract BadRequest(string message) => new() {
        Status = StatusCodes.Status400BadRequest,
        Message = message
    };
    private static bool ApiKeyPatternMatches(Regex pattern, string origin, Uri referrer) {
        var isOrigin = !string.IsNullOrEmpty(origin) && origin != "null";
        var isValidBasedOnReferrer = false;
        var isValidBasedOnOrigin = false;

        if (referrer is not null && pattern.IsMatch(referrer.AbsoluteUri)) {
            isValidBasedOnReferrer = true;
        }

        if (isOrigin) {
            var originUrl = new Uri(origin);
            if (pattern.IsMatch(originUrl.AbsoluteUri)) {
                isValidBasedOnOrigin = true;
            }
        }

        return isValidBasedOnOrigin || isValidBasedOnReferrer;
    }
    private static bool IsLocalDevelopment(Uri referrer, string origin) {
        var isOrigin = !string.IsNullOrEmpty(origin);
        var isLocalBasedOnReferrer = false;
        var isLocalBasedOnOrigin = false;

        if (referrer?.AbsoluteUri.StartsWith("http://localhost/", StringComparison.OrdinalIgnoreCase) == true) {
            isLocalBasedOnReferrer = true;
        }

        if (isOrigin && origin.StartsWith("http://localhost/", StringComparison.OrdinalIgnoreCase)) {
            isLocalBasedOnOrigin = true;
        }

        return isLocalBasedOnOrigin || isLocalBasedOnReferrer;
    }
}

using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ResponseContracts;
using api.mapserv.utah.gov.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace api.mapserv.utah.gov.Filters {
    public class AuthorizeApiKeyFromRequest : IAsyncResourceFilter {
        private const int BadRequest = (int)HttpStatusCode.BadRequest;
        private readonly IBrowserKeyProvider _apiKeyProvider;
        private readonly ILogger _log;
        private readonly IApiKeyRepository _repo;
        private readonly IServerIpProvider _serverIpProvider;

        public AuthorizeApiKeyFromRequest(IBrowserKeyProvider apiKeyProvider,
                                          IServerIpProvider serverIpProvider, IApiKeyRepository repo, ILogger log) {
            _apiKeyProvider = apiKeyProvider;
            _serverIpProvider = serverIpProvider;
            _repo = repo;
            _log = log?.ForContext<AuthorizeApiKeyFromRequest>();
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next) {
            var key = _apiKeyProvider.Get(context.HttpContext.Request);

            // key hasn't been created
            if (string.IsNullOrWhiteSpace(key)) {
                _log.Debug("API key missing from request");

                var missingResponse = new ApiResponseContract {
                    Status = BadRequest,
                    Message =
                        "Your API key is missing from your request. Add an `apikey={key}` to the request as a query string parameter."
                };

                context.Result = new BadRequestObjectResult(missingResponse);

                return;
            }

            var apiKey = await _repo.GetKey(key);

            var badKeyResponse = new ApiResponseContract {
                Status = BadRequest,
                Message = "Your API key does match the pattern created in the developer console. " +
                          $"Check the referrer header on the request with the pattern for the api key `{key}`"
            };

            // key hasn't been created
            if (apiKey == null) {
                _log.Information("Unknown API key usage attempt for {key}", context.HttpContext.Request.Query);

                context.Result = new BadRequestObjectResult(badKeyResponse);

                return;
            }

            // TODO make sure user has confirmed email address

            if (apiKey.Deleted || apiKey.Enabled == ApiKey.KeyStatus.Disabled) {
                _log.Information("Attempt to use deleted or disabled key {key}", apiKey);

                context.Result = new BadRequestObjectResult(new ApiResponseContract {
                    Status = BadRequest,
                    Message = $"{key} is no longer active. It has been disabled or deleted by it's owner."
                });

                return;
            }

            if (apiKey.Whitelisted) {
                _log.Information("Unrestricted key use {key} from {ip} with {headers}", apiKey.Key,
                                 context.HttpContext.Request.Host, context.HttpContext.Request.Headers);

                await next();

                return;
            }

            if (apiKey.Type == ApiKey.ApplicationType.Browser) {
                var pattern = new Regex(apiKey.RegexPattern, RegexOptions.IgnoreCase);

                if (!context.HttpContext.Request.Headers.TryGetValue("Referrer", out var referrer)) {
                    context.HttpContext.Request.Headers.TryGetValue("Referer", out referrer);
                }

                var hasOrigin = context.HttpContext.Request.Headers.Where(x => x.Key == "Origin").ToList();

                if (string.IsNullOrEmpty(referrer.ToString()) && !hasOrigin.Any()) {
                    _log.Information("API key usage without referrer header {key}", apiKey);

                    context.Result = new BadRequestObjectResult(new ApiResponseContract {
                        Status = BadRequest,
                        Message =
                            "The http referrer header is missing. Turn off any security solutions that may remove this " +
                            "header to use this service. If you are trying to test your query add the referrer header via a tool like postman " +
                            "or browse to api.mapserv.utah.gov and use the api explorer."
                    });

                    return;
                }

                var corsOriginHeader = hasOrigin.FirstOrDefault();
                var corsOriginValue = "";

                if (corsOriginHeader.Key != null) {
                    corsOriginValue = corsOriginHeader.Value.SingleOrDefault();
                }

                if (apiKey.Configuration == ApiKey.ApplicationStatus.Development &&
                    IsLocalDevelopment(new Uri(referrer.ToString()), corsOriginValue)) {
                    await next();

                    return;
                }

                if (!ApiKeyPatternMatches(pattern, corsOriginValue, new Uri(referrer.ToString()))) {
                    context.Result = new BadRequestObjectResult(badKeyResponse);

                    return;
                }
            } else {
                var ip = apiKey.Pattern;
                var userHostAddress = _serverIpProvider.Get(context.HttpContext.Request);

                if (ip != userHostAddress) {
                    _log.Information("Invalid api key pattern match {ip} != {host} for {key}", ip, userHostAddress,
                                     apiKey);

                    context.Result = new BadRequestObjectResult(new ApiResponseContract {
                        Status = BadRequest,
                        Message =
                            $"Your API key does match the pattern created in the developer console for key `{key}`. " +
                            $"The request is not originating from `{userHostAddress}`"
                    });

                    return;
                }
            }

            await next();
        }

        private static bool ApiKeyPatternMatches(Regex pattern, string origin, Uri referrer) {
            var isReferrer = !(referrer == null);
            var isOrigin = !string.IsNullOrEmpty(origin);
            var isValidBasedOnReferrer = false;
            var isValidBasedOnOrigin = false;

            if (isReferrer && pattern.IsMatch(referrer.AbsoluteUri)) {
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
            var isReferrer = !(referrer == null);
            var isOrigin = !string.IsNullOrEmpty(origin);
            var isLocalBasedOnReferrer = false;
            var isLocalBasedOnOrigin = false;

            if (isReferrer &&
                referrer.AbsoluteUri.StartsWith("http://localhost/", StringComparison.OrdinalIgnoreCase)) {
                isLocalBasedOnReferrer = true;
            }

            if (isOrigin && origin.StartsWith("http://localhost/", StringComparison.OrdinalIgnoreCase)) {
                isLocalBasedOnOrigin = true;
            }

            return isLocalBasedOnOrigin || isLocalBasedOnReferrer;
        }

        public class ServerIpProvider : IServerIpProvider {
            public string Get(HttpRequest request) => request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public class BrowserKeyProvider : IBrowserKeyProvider {
            public string Get(HttpRequest request) {
                try {
                    var key = request.Query["apikey"];

                    if (!string.IsNullOrEmpty(key)) {
                        return key;
                    }
                } catch {
                }

                try {
                    var formData = request.Form;

                    if (formData.ContainsKey("apikey") && formData.TryGetValue("apikey", out var apikey)) {
                        return apikey.ToString();
                    }
                } catch {
                }

                if (request.Path.Value.ToLower() == "/api/v1/geocode/ago/agrc-ago/geocodeserver" ||
                    request.Path.Value.ToLower() ==
                    "/api/v1/geocode/ago/agrc-ago/geocodeserver/findaddresscandidates") {
                    return "agrc-ago";
                }

                return null;
            }
        }
    }
}

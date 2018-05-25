using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Middleware
{
    public class AuthorizeApiKeyFromRequest
    {
        private const int BadRequest = (int) HttpStatusCode.BadRequest;
        private readonly RequestDelegate _next;
        private readonly IApiKeyRepository _repo;
        private readonly IBrowserKeyProvider _apiKeyProvider;
        private readonly IServerIpProvider _serverIpProvider;

        public AuthorizeApiKeyFromRequest(RequestDelegate next, IBrowserKeyProvider apiKeyProvider,
                                          IServerIpProvider serverIpProvider, IApiKeyRepository repo)
        {
            _next = next;
            _apiKeyProvider = apiKeyProvider;
            _serverIpProvider = serverIpProvider;
            _repo = repo;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = _apiKeyProvider.Get(context.Request);

            // key hasn't been created
            if (string.IsNullOrWhiteSpace(key))
            {
                var missingResponse = new ApiResponseContainer
                {
                    Status = BadRequest,
                    Message =
                        "Your API key is missing from your request. Add an `apikey={key}` to the request as a query string parameter."
                };

                context.Response.StatusCode = BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(missingResponse));

                return;
            }

            var apiKey = await _repo.GetKey(key);

            var badKeyResponse = new ApiResponseContainer
            {
                Status = BadRequest,
                Message = "Your API key does match the pattern created in the developer console. " +
                          $"Check the referrer header on the request with the pattern for the api key `{key}`"
            };

            // key hasn't been created
            if (apiKey == null)
            {
                context.Response.StatusCode = BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(badKeyResponse));

                return;
            }

            // TODO handle whitelist

            // TODO make sure user has confirmed email address

            if (apiKey.Deleted || apiKey.ApiKeyStatus == ApiKey.KeyStatus.Disabled)
            {
                context.Response.StatusCode = BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponseContainer
                {
                    Status = BadRequest,
                    Message = $"{key} is no longer active. It has been disabled or deleted by it's owner."
                }));

                return;
            }

            if (apiKey.Type == ApiKey.ApplicationType.Browser)
            {
                var pattern = new Regex(apiKey.RegexPattern, RegexOptions.IgnoreCase);

                if (!context.Request.Headers.TryGetValue("Referrer", out var referrer))
                {
                    context.Request.Headers.TryGetValue("Referer", out referrer);
                }

                var hasOrigin = context.Request.Headers.Where(x => x.Key == "Origin").ToList();

                if (string.IsNullOrEmpty(referrer.ToString()) && !hasOrigin.Any())
                {
                    context.Response.StatusCode = BadRequest;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponseContainer
                    {
                        Status = BadRequest,
                        Message =
                            "The http referrer header is missing. Turn off any security solutions that may remove this " +
                            "header to use this service. If you are trying to test your query add the referer header via a tool like postman " +
                            "or browse to api.mapserv.utah.gov and use the api explorer."
                    }));

                    return;
                }

                var corsOriginHeader = hasOrigin.FirstOrDefault();
                var corsOriginValue = "";

                if (corsOriginHeader.Key != null)
                {
                    corsOriginValue = corsOriginHeader.Value.SingleOrDefault();
                }

                if (apiKey.AppStatus == ApiKey.ApplicationStatus.Development &&
                    IsLocalDevelopment(new Uri(referrer.ToString()), corsOriginValue))
                {
                    await _next(context);

                    return;
                }

                if (!ApiKeyPatternMatches(pattern, corsOriginValue, new Uri(referrer.ToString())))
                {
                    context.Response.StatusCode = BadRequest;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(badKeyResponse));

                    return;
                }
            }
            else
            {
                var ip = apiKey.Pattern;
                var userHostAddress = _serverIpProvider.Get(context.Request);

                if (ip != userHostAddress)
                {
                    context.Response.StatusCode = BadRequest;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponseContainer
                    {
                        Status = BadRequest,
                        Message =
                            $"Your API key does match the pattern created in the developer console for key `{key}`. " +
                            $"The request is not originiating from `{userHostAddress}`"
                    }));

                    return;
                }
            }

            await _next(context);
        }

        private static bool ApiKeyPatternMatches(Regex pattern, string origin, Uri referrer)
        {
            var isReferrer = !(referrer == null);
            var isOrigin = !string.IsNullOrEmpty(origin);
            var isValidBasedOnReferrer = false;
            var isValidBasedOnOrigin = false;

            if (isReferrer && pattern.IsMatch(referrer.AbsoluteUri))
            {
                isValidBasedOnReferrer = true;
            }

            if (isOrigin)
            {
                var originUrl = new Uri(origin);
                if (pattern.IsMatch(originUrl.AbsoluteUri))
                {
                    isValidBasedOnOrigin = true;
                }
            }

            return isValidBasedOnOrigin || isValidBasedOnReferrer;
        }

        private static bool IsLocalDevelopment(Uri referrer, string origin)
        {
            var isReferrer = !(referrer == null);
            var isOrigin = !string.IsNullOrEmpty(origin);
            var isLocalBasedOnReferrer = false;
            var isLocalBasedOnOrigin = false;

            if (isReferrer && referrer.AbsoluteUri.StartsWith("http://localhost/"))
            {
                isLocalBasedOnReferrer = true;
            }

            if (isOrigin && origin.StartsWith("http://localhost/"))
            {
                isLocalBasedOnOrigin = true;
            }

            return isLocalBasedOnOrigin || isLocalBasedOnReferrer;
        }

        public class ServerIpProvider : IServerIpProvider
        {
            public string Get(HttpRequest request)
            {
                return request.HttpContext.Connection.RemoteIpAddress.ToString();
            }
        }

        public class BrowserKeyProvider : IBrowserKeyProvider
        {
            public string Get(HttpRequest request)
            {
                try
                {
                    var key = request.Query["apikey"];

                    if (!string.IsNullOrEmpty(key))
                    {
                        return key;
                    }
                }
                catch
                {
                }

                try
                {
                    var formData = request.Form;

                    if (formData.ContainsKey("apikey") && formData.TryGetValue("apikey", out StringValues apikey))
                    {
                        return apikey.ToString();
                    }
                }
                catch
                {
                }

                if (request.Path.Value.ToLower() == "/api/v1/geocode/ago/agrc-ago/geocodeserver" ||
                    request.Path.Value.ToLower() ==
                    "/api/v1/geocode/ago/agrc-ago/geocodeserver/findaddresscandidates")
                {
                    return "agrc-ago";
                }

                return null;
            }
        }
    }
}

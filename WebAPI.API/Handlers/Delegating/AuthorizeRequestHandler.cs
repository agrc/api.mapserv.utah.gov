using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using Ninject;
using Raven.Client.Documents;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Common.Models.Raven.Whitelist;
using WebAPI.Common.Providers;
using WebAPI.Domain;

namespace WebAPI.API.Handlers.Delegating
{
    public class AuthorizeRequestHandler : DelegatingHandler
    {
        private const string Origin = "Origin";
        private HttpResponseMessage _invalidResponse;

        [Inject]
        public IDocumentStore DocumentStore { get; set; }

        [Inject]
        public IpProvider IpProvider { get; set; }

        [Inject]
        public ApiKeyProvider ApiKeyProvider { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Properties.Any())
            {
                //properties is null under test need to add basic configuration
                request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            }

            if (DocumentStore == null || DocumentStore.WasDisposed)
            {
                DocumentStore = request.GetDependencyScope().GetService(typeof (IDocumentStore)) as IDocumentStore;
            }

            var apikey = await ApiKeyProvider.GetApiFromRequestAsync(request);

            if (string.IsNullOrWhiteSpace(apikey))
            {
                var response = request.CreateResponse(HttpStatusCode.BadRequest,
                    new ResultContainer
                    {
                        Status = (int) HttpStatusCode.BadRequest,
                        Message = "Missing API key."
                    }, new MediaTypeHeaderValue("application/json"));

                return response;
            }

            using (var s = DocumentStore.OpenSession())
            {
                _invalidResponse = request.CreateResponse(HttpStatusCode.BadRequest,
                    new ResultContainer
                    {
                        Status = (int) HttpStatusCode.BadRequest,
                        Message = "Invalid API key."
                    }, new MediaTypeHeaderValue("application/json"));

                var key = s.Query<ApiKey, IndexApiKey>()
                    .Customize(c => c.WaitForNonStaleResults())
                    .Include(x => x.AccountId)
                    .SingleOrDefault(x => x.Key == apikey);

                if (key == null)
                {
                    return _invalidResponse;
                }

                var validateUser = true;

                var isWhitelisted = s.Query<WhitelistContainer>()
                    .SingleOrDefault(x => x.Items.Any(y => y.Key == key.Key));

                if (isWhitelisted != null)
                {
                    return await base.SendAsync(request, cancellationToken);
                }

                if (validateUser)
                {
                    var user = s.Load<Account>(key.AccountId);

                    if (user == null || !user.Confirmation.Confirmed)
                    {
                        return request.CreateResponse(HttpStatusCode.BadRequest,
                            new ResultContainer
                            {
                                Status = (int) HttpStatusCode.BadRequest,
                                Message =
                                    "Invalid key owner. Key does not belong to user or user has not been confirmed. " +
                                    "Browse to your profile and click the confirm button next to your email. " +
                                    "Follow the instructions in your inbox."
                            },
                            new MediaTypeHeaderValue("application/json"));
                    }

                    if (key.Deleted || key.ApiKeyStatus == ApiKey.KeyStatus.Disabled)
                    {
                        return request.CreateResponse(HttpStatusCode.BadRequest,
                            new ResultContainer
                            {
                                Status = (int) HttpStatusCode.BadRequest,
                                Message = "Key no longer exists or has been deactivated."
                            },
                            new MediaTypeHeaderValue("application/json"));
                    }
                }

                var response = await ValidateRequest(key, request, cancellationToken);

                if (response != null)
                {
                    return response;
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<HttpResponseMessage> ValidateRequest(ApiKey key, HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (key.Type == ApiKey.ApplicationType.Browser)
            {
                var pattern = new Regex(key.RegexPattern, RegexOptions.IgnoreCase);

                var referrer = request.Headers.Referrer;
                var hasOrigin = request.Headers.Where(x => x.Key == Origin).ToList();

                if (referrer == null && !hasOrigin.Any())
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest,
                        new ResultContainer
                        {
                            Status = (int) HttpStatusCode.BadRequest,
                            Message = "Referrer http header is missing. " +
                                      "Turn off any security solutions that hide this header to use this service."
                        },
                        new MediaTypeHeaderValue("application/json"));
                }

                if (!referrer.IsAbsoluteUri)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest,
                        new ResultContainer
                        {
                            Status = (int)HttpStatusCode.BadRequest,
                            Message = "Referrer http header is invalid. The value must be a vaid URI. e.g. http://my.url.com"
                        },
                        new MediaTypeHeaderValue("application/json"));
                }

                var corsOriginHeader = hasOrigin.FirstOrDefault();
                var corsOriginValue = "";

                if (corsOriginHeader.Key != null)
                {
                    corsOriginValue = corsOriginHeader.Value.SingleOrDefault();
                }

                if (key.AppStatus == ApiKey.ApplicationStatus.Development &&
                    IsLocalDevelopment(referrer, corsOriginValue))
                {
                    return await base.SendAsync(request, cancellationToken);
                }

                if (!ApiKeyPatternMatches(pattern, corsOriginValue, referrer))
                {
                    return _invalidResponse;
                }
            }
            else
            {
                var ip = key.Pattern;
                var userHostAddress = IpProvider.GetIp(request);

                if (ip != userHostAddress)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest,
                        new ResultContainer
                        {
                            Status = (int) HttpStatusCode.BadRequest,
                            Message = string.Format("Invalid API key. The IP you provided does not match what the API " + 
                                                    "is receiving. Double check your API key or create a new one using `{0}` " + 
                                                    "as the pattern.", userHostAddress)
                        },
                        new MediaTypeHeaderValue("application/json"));
                }
            }

            return null;
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

        private static bool IsLocalDevelopment(Uri referrer, string orign)
        {
            var isReferrer = !(referrer == null);
            var isOrigin = !string.IsNullOrEmpty(orign);
            var isLocalBasedOnReferrer = false;
            var isLocalBasedOnOrigin = false;

            if (isReferrer && referrer.AbsoluteUri.StartsWith("http://localhost/", StringComparison.OrdinalIgnoreCase))
            {
                isLocalBasedOnReferrer = true;
            }

            if (isOrigin && orign.StartsWith("http://localhost/", StringComparison.OrdinalIgnoreCase))
            {
                isLocalBasedOnOrigin = true;
            }

            return isLocalBasedOnOrigin || isLocalBasedOnReferrer;
        }
    }
}

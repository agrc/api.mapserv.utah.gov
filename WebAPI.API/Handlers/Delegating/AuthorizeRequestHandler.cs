using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Hosting;
using Ninject;
using Raven.Client;
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

        [Inject]
        public IDocumentStore DocumentStore { get; set; }

        [Inject]
        public IpProvider IpProvider { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
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

            var apikey = FindApikey(request);

            if (string.IsNullOrWhiteSpace(apikey))
            {
                var response = request.CreateResponse(HttpStatusCode.BadRequest,
                                                      new ResultContainer
                                                          {
                                                              Status = (int) HttpStatusCode.BadRequest,
                                                              Message = "Missing API key."
                                                          }, new MediaTypeHeaderValue("application/json"));

                return Task.Factory.StartNew(() => response);
            }

            using (var s = DocumentStore.OpenSession())
            {
                var invalidResponse = request.CreateResponse(HttpStatusCode.BadRequest,
                                                             new ResultContainer
                                                                 {
                                                                     Status = (int) HttpStatusCode.BadRequest,
                                                                     Message = "Invalid API key."
                                                                 }, new MediaTypeHeaderValue("application/json"));

                var key = s.Query<ApiKey, IndexApiKey>()
                           .Customize(c => c.WaitForNonStaleResultsAsOfNow())
                           .Include(x => x.AccountId)
                           .SingleOrDefault(x => x.Key == apikey);

                if (key == null)
                {
                    return Task.Factory.StartNew(() => invalidResponse);
                }

                var isWhitelisted = s.Query<WhitelistContainer>()
                                     .Customize(c => c.WaitForNonStaleResultsAsOfNow())
                                     .SingleOrDefault(x => x.Items.Any(y => y.Key == key.Key));

                if (isWhitelisted != null)
                {
                    return base.SendAsync(request, cancellationToken);
                }

                var user = s.Load<Account>(key.AccountId);

                if (user == null || !user.Confirmation.Confirmed)
                {
                    return Task.Factory.StartNew(() => request.CreateResponse(HttpStatusCode.BadRequest,
                                                                              new ResultContainer
                                                                                  {
                                                                                      Status =
                                                                                          (int)
                                                                                          HttpStatusCode.BadRequest,
                                                                                      Message =
                                                                                          "Invalid key owner. Key does not belong to user or user has not been confirmed. Browse to your profile and click the confirm button next to your email. Follow the instructions in your inbox."
                                                                                  },
                                                                              new MediaTypeHeaderValue(
                                                                                  "application/json")));
                }

                if (key.Deleted || key.ApiKeyStatus == ApiKey.KeyStatus.Disabled)
                {
                    return Task.Factory.StartNew(() => request.CreateResponse(HttpStatusCode.BadRequest,
                                                                              new ResultContainer
                                                                                  {
                                                                                      Status =
                                                                                          (int)
                                                                                          HttpStatusCode.BadRequest,
                                                                                      Message =
                                                                                          "Key no longer exists or has been deactivated."
                                                                                  },
                                                                              new MediaTypeHeaderValue(
                                                                                  "application/json")));
                }

                if (key.Type == ApiKey.ApplicationType.Browser)
                {
                    var pattern = new Regex(key.RegexPattern, RegexOptions.IgnoreCase);

                    var referrer = request.Headers.Referrer;
                    var hasOrigin = request.Headers.Where(x => x.Key == Origin).ToList();

                    if (referrer == null && !hasOrigin.Any())
                    {
                        return Task.Factory.StartNew(() => request.CreateResponse(HttpStatusCode.BadRequest,
                                                                                  new ResultContainer
                                                                                      {
                                                                                          Status =
                                                                                              (int)
                                                                                              HttpStatusCode.BadRequest,
                                                                                          Message =
                                                                                              "Referrer http header is missing. Turn off any security solutions that hide this header to use this service."
                                                                                      },
                                                                                  new MediaTypeHeaderValue(
                                                                                      "application/json")));
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
                        return base.SendAsync(request, cancellationToken);
                    }

                    if (!ApiKeyPatternMatches(pattern, corsOriginValue, referrer))
                    {
                        return Task.Factory.StartNew(() => invalidResponse);
                    }
                }
                else
                {
                    var ip = key.Pattern;
                    var userHostAddress = IpProvider.GetIp(request);

                    if (ip != userHostAddress)
                    {
                        return Task.Factory.StartNew(() => request.CreateResponse(HttpStatusCode.BadRequest,
                                                                                  new ResultContainer
                                                                                      {
                                                                                          Status =
                                                                                              (int)
                                                                                              HttpStatusCode.BadRequest,
                                                                                          Message =
                                                                                              string.Format(
                                                                                                  "Invalid API key. Pattern does not match {0}.",
                                                                                                  userHostAddress)
                                                                                      },
                                                                                  new MediaTypeHeaderValue(
                                                                                      "application/json")));
                    }
                }
            }

            return base.SendAsync(request, cancellationToken);
        }

        private static bool ApiKeyPatternMatches(Regex pattern, string orign, Uri referrer)
        {
            var isReferrer = !(referrer == null);
            var isOrigin = !string.IsNullOrEmpty(orign);
            var isValidBasedOnReferrer = false;
            var isValidBasedOnOrigin = false;

            if (isReferrer && pattern.IsMatch(referrer.AbsoluteUri))
            {
                isValidBasedOnReferrer = true;
            }

            if (isOrigin)
            {
                var originUrl = new Uri(orign);
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

            if (isReferrer && referrer.AbsoluteUri.StartsWith("http://localhost/"))
            {
                isLocalBasedOnReferrer = true;
            }

            if (isOrigin && orign.StartsWith("http://localhost/"))
            {
                isLocalBasedOnOrigin = true;
            }

            return isLocalBasedOnOrigin || isLocalBasedOnReferrer;
        }

        private static string FindApikey(HttpRequestMessage request)
        {
            try
            {
                var key = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("apikey");

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
                return request.Content.ReadAsFormDataAsync().Result.Get("apikey");
            }
            catch
            {
            }

            return null;
        }
    }
}
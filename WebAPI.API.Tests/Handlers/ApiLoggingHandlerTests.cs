using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WebAPI.API.Handlers.Delegating;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Common.Providers;
using WebAPI.Common.Tests;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Tests.Handlers
{
    [TestFixture]
    public class ApiLoggingHandlerTests : RavenEmbeddableTest
    {
        public override void SetUp()
        {
            base.SetUp();

            using (var s = DocumentStore.OpenSession())
            {
                s.Store(new Account
                    {
                        FirstName = "test",
                        LastName = "account",
                        Confirmation = new EmailConfirmation("confirm")
                            {
                                Confirmed = true
                            }
                    }, "testaccount");

                s.Store(new ApiKey("key")
                    {
                        AccountId = "testaccount",
                        CreatedAtTicks = 634940675825121039,
                        ApiKeyStatus = ApiKey.KeyStatus.Active,
                        Type = ApiKey.ApplicationType.Browser,
                        AppStatus = ApiKey.ApplicationStatus.Production,
                        Pattern = "*",
                        RegexPattern = null,
                        IsMachineName = false,
                        Deleted = false
                    }, "testkey");

                s.SaveChanges();
            }
        }

        [Test, Explicit("we switched to redis and it's not as easy to test as the raven in memory")]
        public void LogsCorrectAmountOfRequests()
        {
            var routeDataMoq = new Mock<RouteDataProvider>();
            routeDataMoq.Setup(x => x.GetRouteData(It.IsAny<HttpRequestMessage>()))
                        .Returns(new RouteData
                            {
                                Controller = "GEOCODE",
                                Action = "GET"
                            });

            var content =
                new ObjectContent<ResultContainer<GeocodeAddressResult>>(new ResultContainer<GeocodeAddressResult>
                    {
                        Result = new GeocodeAddressResult
                            {
                                InputAddress = "tESTING",
                                Score = 100
                            }
                    }, new JsonMediaTypeFormatter());
            content.Headers.Add("X-Type", typeof(ResultContainer<GeocodeAddressResult>).ToString());

            var contentMoq = new Mock<HttpContentProvider>();
            contentMoq.Setup(x => x.GetResponseContent(It.IsAny<HttpResponseMessage>()))
                      .Returns(content);

            var handler = new ApiLoggingHandler
                {
                    InnerHandler = new TestHandler((r, c) => TestHandler.Return200()),
                    RouteDataProvider = routeDataMoq.Object,
                    HttpContentProvider = contentMoq.Object,
                    ApiKeyProvider = new ApiKeyProvider()
                };

            var client = new HttpClient(handler);
            const int requests = 10;

            for (var i = 0; i < requests; i++)
            {
                var result = client.GetAsync("http://api.mapserv.utah.gov/api/v1/Geocode/326 east south temple/84111?apiKey=key")
                                   .Result;

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }

            int count;
            using (var s = DocumentStore.OpenSession())
            {
                count = s.Query<GeocodeStreetZoneUsage>()
                         .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                         .Count();
            }
            Assert.That(count, Is.EqualTo(requests));
        }
    }
}
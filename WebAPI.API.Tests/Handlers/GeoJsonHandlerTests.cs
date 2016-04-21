using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Moq;
using NUnit.Framework;
using WebAPI.API.Handlers.Delegating;
using WebAPI.Common.Providers;
using WebAPI.Common.Tests;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Tests.Handlers
{
    [TestFixture]
    public class GeoJsonHandlerTests
    {
        [Test]
        public void Test()
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
                                Location = new Location {X = 1, Y = 2},
                                Score = 100
                            }
                    }, new JsonMediaTypeFormatter());

            content.Headers.Add("X-Format", "geojson");
            content.Headers.Add("X-Type", typeof(GeocodeAddressResult).ToString());

            var contentMoq = new Mock<HttpContentProvider>();
            contentMoq.Setup(x => x.GetRequestContent(It.IsAny<HttpRequestMessage>()))
                      .Returns(content);

            var handler = new GeoJsonHandler
                {
                    InnerHandler = new TestHandler((r, c) => TestHandler.Return200()),
                    HttpContentProvider = contentMoq.Object
                };

            var client = new HttpClient(handler);
            const int requests = 10;

            for (var i = 0; i < requests; i++)
            {
                var result =
                    client.GetAsync("http://mapserv.utah.gov/beta/WebAPI/api/v1/Geocode/2236 atkin ave/84109?apiKey=key")
                          .Result;

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
        }
    }
}
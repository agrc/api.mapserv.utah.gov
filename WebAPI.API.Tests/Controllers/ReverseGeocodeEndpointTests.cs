using System.Diagnostics;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Tests.Controllers
{
    [TestFixture]
    public class ReverseGeocodeEndpointTests : GeocodeTestsBase
    {
        [Test]
        public void RouteWorks()
        {
            //arrange
            var client = new HttpClient();

            //act
            var response =
                client.GetAsync(
                    "http://webapi/api/v1/Geocode/reverse/1/2?apiKey=AGRC-Explorer")
                      .Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void ReverseLocationWorksInDefaultProjection()
        {
            //arrange
            var client = new HttpClient();
            
            //act
            var response =
                client.GetAsync(
                    "http://webapi/api/v1/Geocode/reverse/430196.865872593/4506863.8943712?apiKey=AGRC-Explorer")
                      .Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            var result = response.Content.ReadAsAsync<ResultContainer<ReverseGeocodeResult>>().Result;

            //assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result, Is.Not.Null, "response is null");
            Assert.That(result.Result, Is.Not.Null, "result is null");
            Assert.That(result.Result.Address, Is.Not.Null, "address object is null");
            Assert.That(result.Result.Address.Street.ToLowerInvariant(), Is.EqualTo("2241 E ATKIN AVE".ToLowerInvariant()), "address is wrong");
        }
    }
}
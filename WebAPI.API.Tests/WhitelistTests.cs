using System.Net;
using System.Net.Http;
using NUnit.Framework;
using WebAPI.API.Models;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Tests
{
    [TestFixture]
    public class WhitelistTests
    {
        [Test]
        public void NonWhitelistKeyGetsBlocked()
        {
            var client = new HttpClient();

            var response =
                client.GetAsync(
                    "http://webapi/api/v1/Geocode/2241 atkin ave/84109?apiKey=AGRC-ApiExplorer2").Result;
            var result = response.Content.ReadAsStringAsync().Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(response.StatusCode == HttpStatusCode.BadRequest);
            Assert.That(result, Is.EqualTo("{\"status\":400,\"message\":\"Invalid API key.\"}"));
        }

        [Test]
        public void WhitelistKeyDoesWhatItWants()
        {
            var client = new HttpClient();

            var response =
                client.GetAsync(
                    "http://webapi/api/v1/Geocode/326 east south temple/84111?apiKey=AGRC-Explorer").Result;
            var result = response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>().Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result.MatchAddress.ToLowerInvariant(), Is.EqualTo("326 E South Temple St, salt lake city".ToLowerInvariant()));
            Assert.That(result.Result.Locator.ToLowerInvariant(), Is.EqualTo("addresspoints.addressgrid").IgnoreCase);
        }
    }
}
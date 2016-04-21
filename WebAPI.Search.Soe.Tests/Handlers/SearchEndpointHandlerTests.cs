using ESRI.ArcGIS.SOESupport;
using NUnit.Framework;
using WebAPI.Search.Soe.Endpoints;

namespace WebAPI.Search.Soe.Tests.Handlers
{
    [TestFixture]
    public class SearchEndpointHandlerTests
    {
        [Test]
        public void ReturnsErrorIfLayerIsNotFormattedCorrectly()
        {
            var json = new JsonObject("{\"featureClass\": \"sgid\", \"returnValues\": \"a,b\", \"predicate\": \"age > 10\", \"point\": \"123, 234\" }");
            string responseProperties;

            var response = SearchEndpoint.Handler(null, json, null, null, out responseProperties);
            var result = System.Text.Encoding.UTF8.GetString(response);

            Assert.That(result, Is.EqualTo("{\"error\":{\"code\":400,\"message\":\"sgid was not found in our database. A valid example would be SGID10.BOUNDARIES.Counties.\"}}"));
        }

        [Test]
        public void ReturnsErrorIfBadInputInFeatureClass()
        {
            var json = new JsonObject("{\"featureClass\": \"sgid10.boundaries.counties; drop table yes--\", \"returnValues\": \"a,b\", \"predicate\": \"age > 10\", \"point\": \"123, 234\" }");
            string responseProperties;

            var response = SearchEndpoint.Handler(null, json, null, null, out responseProperties);
            var result = System.Text.Encoding.UTF8.GetString(response);

            Assert.That(result, Is.EqualTo("{\"error\":{\"code\":400,\"message\":\"Input appears to be unsafe. That is all we know.\"}}"));
        }

        [Test]
        public void ReturnsErrorIfBadInputInReturnValues()
        {
            var json = new JsonObject("{\"featureClass\": \"sgid10.boundaries.counties\", \"returnValues\": \"a,b; drop table yes--\", \"predicate\": \"age > 10\", \"point\": \"123, 234\" }");
            string responseProperties;

            var response = SearchEndpoint.Handler(null, json, null, null, out responseProperties);
            var result = System.Text.Encoding.UTF8.GetString(response);

            Assert.That(result, Is.EqualTo("{\"error\":{\"code\":400,\"message\":\"Input appears to be unsafe. That is all we know.\"}}"));
        }

        [Test]
        public void ReturnsErrorIfBadInputInPredicate()
        {
            var json = new JsonObject("{\"featureClass\": \"sgid10.boundaries.counties\", \"returnValues\": \"a,b\", \"predicate\": \"age > 10; drop table yes--\", \"point\": \"123, 234\" }");
            string responseProperties;

            var response = SearchEndpoint.Handler(null, json, null, null, out responseProperties);
            var result = System.Text.Encoding.UTF8.GetString(response);

            Assert.That(result, Is.EqualTo("{\"error\":{\"code\":400,\"message\":\"Input appears to be unsafe. That is all we know.\"}}"));
        } 
    }
}
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using WebAPI.API.Commands.Spatial;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class CreateFeatureFromCommandTests
    {
        [Test]
        public void CanCreateGeoJsonFeatureFromGeocodeResponse()
        {
            var objectToSerialize = new ResultContainer<GeocodeAddressResult>
                {
                    Result = new GeocodeAddressResult
                        {
                            Candidates = new Candidate[0],
                            InputAddress = "Input Address",
                            Location = new Location
                                {
                                    X = 1,
                                    Y = 1
                                },
                            Locator = "Centerlines",
                            MatchAddress = "Matched Address",
                            Score = 100,
                            Wkid = 26912
                        },
                        Status = 200
                };

            var command = new CreateFeatureFromCommand(objectToSerialize);
            command.Run();

            Assert.That(command.Result, Is.Not.Null, "result shoult not be null");

            var result = command.Result.Result;

            Assert.That(result, Is.Not.Null, "result shoult not be null");
            
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new StringEnumConverter());

            var output = JsonConvert.SerializeObject(result);
            dynamic d = JObject.Parse(output);

            //http://www.geojson.org/geojson-spec.html#feature-objects
            Assert.That(d.geometry, Is.Not.Null, "A feature object must have a member with the name 'geometry'.");
            Assert.That(d.properties, Is.Not.Null, "A feature object must have a member with the name 'properties'.");
            Assert.That(output.Contains("\"type\":\"Point\""), Is.True, "A geometry is a GeoJSON object where the type member's value is one of the following strings: 'Point', 'MultiPoint', 'LineString', 'MultiLineString', 'Polygon', 'MultiPolygon', or 'GeometryCollection'.");
            Assert.That(d.geometry.coordinates, Is.Not.Null, "A GeoJSON geometry object of any type other than 'GeometryCollection' must have a member with the name 'coordinates'.");
            Assert.That(d.geometry.coordinates.Count, Is.EqualTo(2), "For type 'Point', the 'coordinates' member must be a single position");
        }
    }
}
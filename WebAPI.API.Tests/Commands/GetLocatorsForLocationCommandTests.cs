using NUnit.Framework;
using WebAPI.API.Commands.Geocode;
using WebAPI.Common.Executors;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class GetLocatorsForLocationCommandTests
    {
        [Test]
        public void ReverseGeocodingOnlyWorksOnCerterlines()
        {
            var result = CommandExecutor.ExecuteCommand(new GetLocatorsForLocationCommand(new Location())
                {
                    Host = "test"
                });
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Centerlines.StatewideRoads"));
            Assert.That(result.Url.ToLowerInvariant(), Is.EqualTo("http://test/arcgis/rest/services/geolocators/roads_addresssystem_street/geocodeserver/reversegeocode?location={0},{1}&distance={2}&outsr={3}&f=json"));
        }
    }
}
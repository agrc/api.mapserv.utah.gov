using System.Configuration;
using WebAPI.Common.Abstractions;
using WebAPI.Domain;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Commands.Geocode
{
    public class GetLocatorsForLocationCommand : Command<LocatorDetails>
    {
        public GetLocatorsForLocationCommand(Location location)
        {
            Location = location;
            Host = ConfigurationManager.AppSettings["gis_server_host"];
        }

        public Location Location { get; set; }
        public string Host { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Location: {1}, Host: {2}", "GetLocatorsForLocationCommand", Location, Host);
        }

        protected override void Execute()
        {
            Result =
                new LocatorDetails
                    {
                        Url = Host +
                            "/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/GeocodeServer/reverseGeocode?location={0},{1}&distance={2}&outSR={3}&f=json",
                        Name = "Centerlines.StatewideRoads"
                    };
        }
    }
}
using System.Collections.ObjectModel;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.SecretOptions;
using Microsoft.Extensions.Options;

namespace api.mapserv.utah.gov.Commands
{
    public class GetLocatorsForReverseLookupCommand : Command<ReadOnlyCollection<LocatorProperties>>
    {
        private readonly string Host;

        public GetLocatorsForReverseLookupCommand(IOptions<GisServerConfiguration> options)
        {
            Host = options.Value.Host;
        }

        public override string ToString() => "GetLocatorsForReverseLookupCommand";

        protected override void Execute()
        {
            Result = new ReadOnlyCollection<LocatorProperties>(new[] { new LocatorProperties {
                    Url = $"http://{Host}/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/GeocodeServer/reverseGeocode?location={{0}},{{1}}&distance={{2}}&outSR={{3}}&f=json",
                Name = "Centerlines.StatewideRoads"
            }});
        }
    }
}

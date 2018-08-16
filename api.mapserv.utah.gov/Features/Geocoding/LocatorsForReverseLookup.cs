using System.Collections.Generic;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.SecretOptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace api.mapserv.utah.gov.Features.Geocoding
{
    public class LocatorsForReverseLookup
    {
        public class Command : IRequest<IEnumerable<LocatorProperties>>
        {
        }

        public class Handler : RequestHandler<Command, IEnumerable<LocatorProperties>>
        {
            private readonly string _host;

            public Handler(IOptions<GisServerConfiguration> options)
            {
                _host = options.Value.ToString();
            }

            protected override IEnumerable<LocatorProperties> Handle(Command request)
            {
                return new[] { 
                    new LocatorProperties {
                        Url = $"{_host}/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/GeocodeServer/reverseGeocode?location={{0}},{{1}}&distance={{2}}&outSR={{3}}&f=json",
                        Name = "Centerlines.StatewideRoads"
                    }
                };
            }
        }
    }
}

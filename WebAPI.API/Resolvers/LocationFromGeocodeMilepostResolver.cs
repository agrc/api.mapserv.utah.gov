using AutoMapper;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.ArcServerResponse.Soe;

namespace WebAPI.API.Resolvers
{
    public class LocationFromGeocodeMilepostResolver : ValueResolver<GeocodeMilepostResponse, Location>
    {
        protected override Location ResolveCore(GeocodeMilepostResponse source)
        {
            return new Location
                {
                    X = source.UTM_X,
                    Y = source.UTM_Y
                };
        }
    }
}
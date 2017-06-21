using System;
using AutoMapper;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.ArcServerResponse.Soe;

namespace WebAPI.API.Resolvers
{
    public class LocationFromGeocodeMilepostResolver : IValueResolver<GeocodeMilepostResponse, RouteMilepostResult, Location>
    {
        public Location Resolve(GeocodeMilepostResponse source, RouteMilepostResult destination, Location destMember, ResolutionContext context)
        {
            return new Location
            {
                X = source.UTM_X,
                Y = source.UTM_Y
            };
        }
    }
}
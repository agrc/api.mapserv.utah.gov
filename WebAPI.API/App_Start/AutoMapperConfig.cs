using AutoMapper;
using WebAPI.API.Resolvers;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.ArcServerResponse.Soe;

namespace WebAPI.API
{
    public class AutoMapperConfig
    {
        public static void RegisterMaps()
        {
            Mapper.CreateMap<ReverseGeocodeResponse, ReverseGeocodeResult>()
                  .ForMember(dest => dest.InputLocation, opt => opt.MapFrom(src => src.Location));
            
            Mapper.CreateMap<GeocodeMilepostResponse, RouteMilepostResult>()
                  .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Geocoder))
                  .ForMember(dest => dest.MatchRoute, opt => opt.MapFrom(src => src.MatchAddress))
                  .ForMember(d => d.InputRouteMilePost, src => src.Ignore())
                  .ForMember(dest => dest.Location, opt => opt.ResolveUsing<LocationFromGeocodeMilepostResolver>());

            Mapper.AssertConfigurationIsValid();
        }
    }
}
using api.mapserv.utah.gov.Models.RequestOptions;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public interface IHasGeocodingOptions {
        GeocodingOptions Options { get; }
    }
}

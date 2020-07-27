using AGRC.api.Models.RequestOptions;

namespace AGRC.api.Features.Geocoding {
    public interface IHasGeocodingOptions {
        GeocodingOptions Options { get; }
    }
}

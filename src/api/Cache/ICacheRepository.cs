using ugrc.api.Models.Linkables;

namespace ugrc.api.Cache;
public interface ICacheRepository {
    Task<IReadOnlyCollection<GridLinkable>> FindGridsForPlaceAsync(string placeName);
    Task<IReadOnlyCollection<GridLinkable>> FindGridsForZipCodeAsync(string zipCode);
}

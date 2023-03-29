using AGRC.api.Models.Linkables;

namespace AGRC.api.Cache;
public interface ICacheRepository {
    Task<IReadOnlyCollection<GridLinkable>> FindGridsForPlaceAsync(string placeName);
    Task<IReadOnlyCollection<GridLinkable>> FindGridsForZipCodeAsync(string zipCode);
}

using ugrc.api.Features.Geocoding;
using ugrc.api.Models.Linkables;

namespace ugrc.api.Cache;
public interface IStaticCache {
    Dictionary<int, PoBoxAddress> PoBoxes { get; }
    IReadOnlyCollection<int> PoBoxZipCodesWithExclusions { get; }
    Dictionary<int, PoBoxAddressCorrection> PoBoxExclusions { get; }
    Dictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; }
}

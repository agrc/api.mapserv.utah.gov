using AGRC.api.Features.Geocoding;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Cache;
public interface IStaticCache {
    Dictionary<int, PoBoxAddress> PoBoxes { get; }
    IReadOnlyCollection<int> PoBoxZipCodesWithExclusions { get; }
    Dictionary<int, PoBoxAddressCorrection> PoBoxExclusions { get; }
    Dictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; }
}

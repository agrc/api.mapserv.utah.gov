using System.Collections.Generic;
using System.Threading.Tasks;
using AGRC.api.Models;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Cache {
    public interface ILookupCache {
        IDictionary<string, List<GridLinkable>> PlaceGrids { get; set; }
        IDictionary<int, PoBoxAddress> PoBoxes { get; set; }
        IReadOnlyCollection<int> PoBoxZipCodesWithExclusions { get; set; }
        IDictionary<int, PoBoxAddressCorrection> PoBoxExclusions { get; set; }
        IDictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; set; }
        IDictionary<string, List<GridLinkable>> ZipCodesGrids { get; set; }
        Task InitializeAsync();
    }
}

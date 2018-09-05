using System.Collections.Generic;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Linkables;

namespace api.mapserv.utah.gov.Cache {
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

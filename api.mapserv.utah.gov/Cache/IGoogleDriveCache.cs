using System.Collections.Generic;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Cache
{
    public interface IGoogleDriveCache
    {
        Dictionary<string, List<GridLinkable>> PlaceGrids { get; set; }
        Dictionary<int, PoBoxAddress> PoBoxes { get; set; }
        IEnumerable<int> PoBoxZipCodesWithExclusions { get; set; }
        Dictionary<int, PoBoxAddressCorrection> PoBoxExclusions { get; set; }
        Dictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; set; }
        Dictionary<string, List<GridLinkable>> ZipCodesGrids { get; set; }
    }
}
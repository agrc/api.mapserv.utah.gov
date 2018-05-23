using System.Collections.Generic;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Cache
{
    public interface IGoogleDriveCache
    {
        Dictionary<string, List<GridLinkable>> PlaceGrids { get; set; }
        Dictionary<int, PoBoxAddress> PoBoxes { get; set; }
        IEnumerable<PoBoxAddressCorrection> PoBoxExclusions { get; set; }
        Dictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; set; }
        Dictionary<string, List<GridLinkable>> ZipCodesGrids { get; set; }
    }
}
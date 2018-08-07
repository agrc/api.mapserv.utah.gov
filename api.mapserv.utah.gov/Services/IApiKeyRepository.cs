using System.Collections.Generic;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Services
{
    public interface IApiKeyRepository
    {
        Task<ApiKey> GetKey(string key);
    }

    public interface ICacheRepository
    {
        Task<IEnumerable<PlaceGridLink>> GetPlaceNames();
        Task<IEnumerable<ZipGridLink>> GetZipCodes();
    }

    public interface ILookupCache 
    {
        Dictionary<string, List<GridLinkable>> PlaceGrids { get; set; }
        Dictionary<int, PoBoxAddress> PoBoxes { get; set; }
        IEnumerable<int> PoBoxZipCodesWithExclusions { get; set; }
        Dictionary<int, PoBoxAddressCorrection> PoBoxExclusions { get; set; }
        Dictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; set; }
        Dictionary<string, List<GridLinkable>> ZipCodesGrids { get; set; }
        Task InitializeAsync();
    }
}
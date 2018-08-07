using System.Collections.Generic;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Cache
{
    public interface ICacheRepository
    {
        Task<IEnumerable<PlaceGridLink>> GetPlaceNames();
        Task<IEnumerable<ZipGridLink>> GetZipCodes();
        Task<IEnumerable<UspsDeliveryPointLink>> GetDeliveryPoints();
        Task<IDictionary<int, PoBoxAddress>> GetPoBoxes();
        Task<IEnumerable<PoBoxAddressCorrection>> GetCorrections();
    }
}

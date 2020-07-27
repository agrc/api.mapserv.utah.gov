using System.Collections.Generic;
using System.Threading.Tasks;
using AGRC.api.Models;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Cache {
    public interface ICacheRepository {
        Task<IEnumerable<PlaceGridLink>> GetPlaceNames();
        Task<IEnumerable<ZipGridLink>> GetZipCodes();
        Task<IEnumerable<UspsDeliveryPointLink>> GetDeliveryPoints();
        Task<IDictionary<int, PoBoxAddress>> GetPoBoxes();
        Task<IEnumerable<PoBoxAddressCorrection>> GetCorrections();
    }
}

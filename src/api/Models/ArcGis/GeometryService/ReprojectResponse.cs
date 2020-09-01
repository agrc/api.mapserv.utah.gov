using System.Collections.Generic;

namespace AGRC.api.Models.ArcGis {
    public class ReprojectResponse<T> : RestErrorable {
        public IReadOnlyCollection<T> Geometries { get; set; }
    }
}

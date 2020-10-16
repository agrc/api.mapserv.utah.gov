using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace AGRC.api.Features.Searching {
    public class SearchResponseContract {
        public Geometry Geometry { get; set; }

        public IDictionary<string, object> Attributes { get; set; }
    }
}

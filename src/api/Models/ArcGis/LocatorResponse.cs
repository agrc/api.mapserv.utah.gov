using System.Collections.Generic;

namespace AGRC.api.Models.ArcGis {
    public class LocatorResponse : RestErrorable {
        public IList<Candidate> Candidates { get; set; }
    }
}

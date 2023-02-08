using System.Collections.Generic;

namespace AGRC.api.Models.ArcGis {
    public class LocatorResponse : RestErrorable {
        public List<Candidate> Candidates { get; set; }
    }
}

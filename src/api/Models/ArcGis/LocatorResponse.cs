using System.Collections.Generic;

namespace api.mapserv.utah.gov.Models.ArcGis {
    public class LocatorResponse : RestErrorable {
        public IList<Candidate> Candidates { get; set; }
    }
}

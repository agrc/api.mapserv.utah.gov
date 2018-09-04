using System.Collections.Generic;
using api.mapserv.utah.gov.Models.ArcGis;

namespace api.mapserv.utah.gov.Models.ArcGis {
    public class LocatorResponse : RestErrorable {
        public IList<Candidate> Candidates { get; set; }
    }
}

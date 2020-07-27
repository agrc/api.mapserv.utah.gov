using System.Collections.Generic;

namespace AGRC.api.Models.ArcGis
{
    public class RestEndpointError {
        public int Code { get; set; }
        public string Message { get; set; }
        public IReadOnlyCollection<object> Details { get; set; }
    }
}

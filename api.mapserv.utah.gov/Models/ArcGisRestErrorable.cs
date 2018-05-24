using System.Collections.Generic;

namespace api.mapserv.utah.gov.Models
{
    public class RestEndpointError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IEnumerable<object> Details { get; set; }
    }

    public abstract class ArcGisRestErrorable
    {
        public virtual RestEndpointError Error { get; set; }

        public virtual bool IsSuccessful => Error == null;
    }
}

using System.Collections.Generic;

namespace api.mapserv.utah.gov.Models.ArcGis
{
    public abstract class RestErrorable
    {
        public virtual RestEndpointError Error { get; set; }

        public virtual bool IsSuccessful
        {
            get { return Error == null; }
        }
    }

    public class RestEndpointError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IEnumerable<object> Details { get; set; }
    }
}

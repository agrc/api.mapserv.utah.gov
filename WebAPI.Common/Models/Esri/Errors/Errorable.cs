using System.Collections.Generic;

namespace WebAPI.Common.Models.Esri.Errors
{
    public class RestEndpointError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<object> Details { get; set; }
    }

    public abstract class Errorable
    {
        public virtual RestEndpointError Error { get; set; }

        public virtual bool IsSuccessful
        {
            get { return Error == null; }
        }

    }

}
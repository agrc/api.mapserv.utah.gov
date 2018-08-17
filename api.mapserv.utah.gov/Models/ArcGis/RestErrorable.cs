namespace api.mapserv.utah.gov.Models.ArcGis {
    public abstract class RestErrorable {
        public virtual RestEndpointError Error { get; set; }
        public virtual bool IsSuccessful => Error == null;
    }
}
